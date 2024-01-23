using System;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class InputFieldCell : Cell<InputFieldCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private RectTransform _contents;
        [SerializeField] private CanvasGroup _contentsCanvasGroup;

        public CellIcon icon;
        public CellTexts cellTexts;
        public InputField inputField;
        public Text placeholder;

        protected override void SetModel(InputFieldCellModel model)
        {
            _contentsCanvasGroup.alpha = model.Interactable ? 1.0f : 0.3f;

            // Cleanup
            inputField.onValueChanged.RemoveAllListeners();
            
            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            //Texts
            cellTexts.Setup(model.CellTexts);

            // PlaceHolder
            placeholder.text = model.Placeholder;

            // InputField
            // ContentType must be set before the value because it seems to reset the value if in the Edit Mode.
            inputField.contentType = model.ContentType;
            inputField.interactable = model.Interactable;
            inputField.SetTextWithoutNotify(model.Value);

            inputField.onValueChanged.AddListener(x =>
            {
                model.Value = x;
                model.InvokeValueChanged(x);
            });

            // Height
            var height = model.UseSubTextOrIcon ? 68 : 42; // Texts
            height += 36; // Padding
            height += 1; // Border
            _contents.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _layoutElement.preferredHeight = height; // Set the preferred height for the recycler view.
        }
    }

    public sealed class InputFieldCellModel : CellModel
    {
        public InputFieldCellModel(bool useSubTextOrIcon)
        {
            UseSubTextOrIcon = useSubTextOrIcon;
        }

        public CellIconModel Icon { get; } = new CellIconModel();

        public bool UseSubTextOrIcon { get; }

        public string Value { get; set; }

        public string Placeholder { get; set; }

        public bool Interactable { get; set; } = true;

        public InputField.ContentType ContentType { get; set; } = InputField.ContentType.Standard;

        public CellTextsModel CellTexts { get; } = new CellTextsModel();

        public event Action<string> ValueChanged;

        internal void InvokeValueChanged(string value)
        {
            ValueChanged?.Invoke(value);
        }
    }
}
