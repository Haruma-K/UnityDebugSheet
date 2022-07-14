using System;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class PickerOptionCell : Cell<PickerOptionCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;

        public CellIcon icon;
        public CellTexts cellTexts;
        public Toggle toggle;

        protected override void SetModel(PickerOptionCellModel model)
        {
            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            //Texts
            if (!model.UseSubText)
                model.CellTexts.SubText = null;
            cellTexts.Setup(model.CellTexts);

            // Toggle
            toggle.SetIsOnWithoutNotify(model.IsOn);
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(x =>
            {
                model.IsOn = x;
                model.InvokeToggled(x);
            });

            // Height
            var height = ((RectTransform)cellTexts.transform).rect.height + 36;
            _layoutElement.preferredHeight = height;
        }
    }

    public sealed class PickerOptionCellModel : CellModel
    {
        public PickerOptionCellModel(bool useSubText)
        {
            UseSubText = useSubText;
        }

        public CellIconModel Icon { get; } = new CellIconModel();

        public bool UseSubText { get; }

        public bool IsOn { get; set; }

        public CellTextsModel CellTexts { get; } = new CellTextsModel();

        public event Action<bool> Toggled;

        internal void InvokeToggled(bool isOn)
        {
            Toggled?.Invoke(isOn);
        }
    }
}
