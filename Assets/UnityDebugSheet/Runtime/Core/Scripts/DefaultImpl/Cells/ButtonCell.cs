using System;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class ButtonCell : Cell<ButtonCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;

        public CellIcon icon;
        public CellTexts cellTexts;
        public Image arrow;
        public Button button;

        protected override void SetModel(ButtonCellModel model)
        {
            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            //Texts
            if (!model.UseSubText)
                model.CellTexts.SubText = null;
            cellTexts.Setup(model.CellTexts);

            // Arrow
            arrow.gameObject.SetActive(model.ShowArrow);

            // Button
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(model.InvokeClicked);

            // Height
            var height = ((RectTransform)cellTexts.transform).rect.height + 36;
            _layoutElement.preferredHeight = height;
        }
    }

    public sealed class ButtonCellModel : CellModel
    {
        public ButtonCellModel(bool useSubText)
        {
            UseSubText = useSubText;
        }

        public CellIconModel Icon { get; } = new CellIconModel();

        public bool UseSubText { get; }

        public CellTextsModel CellTexts { get; } = new CellTextsModel();

        public bool ShowArrow { get; set; }

        public event Action Clicked;

        internal void InvokeClicked()
        {
            Clicked?.Invoke();
        }
    }
}
