using System;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class SwitchCell : Cell<SwitchCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;

        public CellIcon icon;
        public CellTexts cellTexts;
        public Toggle toggle;

        protected override void SetModel(SwitchCellModel model)
        {
            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            //Texts
            if (!model.UseSubText)
                model.CellTexts.SubText = null;
            cellTexts.Setup(model.CellTexts);

            // Toggle
            toggle.SetIsOnWithoutNotify(model.Value);
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(x =>
            {
                model.Value = x;
                model.InvokeToggled(x);
            });

            // Height
            var height = ((RectTransform)cellTexts.transform).rect.height + 36;
            _layoutElement.preferredHeight = height;
        }
    }

    public sealed class SwitchCellModel : CellModel
    {
        public SwitchCellModel(bool useSubText)
        {
            UseSubText = useSubText;
        }

        public CellIconModel Icon { get; } = new CellIconModel();

        public bool UseSubText { get; }

        public bool Value { get; set; }

        public CellTextsModel CellTexts { get; } = new CellTextsModel();

        public event Action<bool> ValueChanged;

        internal void InvokeToggled(bool isOn)
        {
            ValueChanged?.Invoke(isOn);
        }
    }
}
