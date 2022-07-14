using System;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class SliderCell : Cell<SliderCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private RectTransform _containerTrans;
        [SerializeField] private RectTransform _topTrans;

        public CellIcon icon;
        public CellTexts cellTexts;
        public InputField valueField;
        public Slider slider;

        protected override void SetModel(SliderCellModel model)
        {
            // Slider
            slider.minValue = model.MinValue;
            slider.maxValue = model.MaxValue;
            
            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            // Texts
            if (!model.UseSubText)
                model.CellTexts.SubText = null;
            cellTexts.Setup(model.CellTexts);

            var value = Mathf.Clamp(model.Value, model.MinValue, model.MaxValue);

            // TextField
            var valueTextFormat = string.IsNullOrEmpty(model.ValueTextFormat) ? "F2" : model.ValueTextFormat;
            valueField.onValueChanged.RemoveAllListeners();
            valueField.gameObject.SetActive(model.ShowValueText);
            valueField.SetTextWithoutNotify(value.ToString(valueTextFormat));
            valueField.keyboardType = TouchScreenKeyboardType.DecimalPad;
            valueField.onEndEdit.AddListener(x =>
            {
                if (float.TryParse(x, out var v))
                {
                    v = Mathf.Clamp(v, model.MinValue, model.MaxValue);
                    slider.SetValueWithoutNotify(v);
                    model.Value = v;
                    valueField.SetTextWithoutNotify(v.ToString(valueTextFormat));
                    model.InvokeValueChanged(v);
                }
            });

            // Slider
            slider.onValueChanged.RemoveAllListeners();
            slider.SetValueWithoutNotify(value);
            slider.onValueChanged.AddListener(x =>
            {
                valueField.SetTextWithoutNotify(x.ToString(valueTextFormat));
                model.Value = x;
                model.InvokeValueChanged(x);
            });

            // Height
            var height = ((RectTransform)cellTexts.transform).rect.height + 36;
            _topTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            height += 65; // Bottom is fixed size.
            _containerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _layoutElement.preferredHeight = height;
        }
    }

    public sealed class SliderCellModel : CellModel
    {
        public SliderCellModel(bool useSubText, float minValue, float maxValue)
        {
            UseSubText = useSubText;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public CellIconModel Icon { get; } = new CellIconModel();

        public bool UseSubText { get; }

        public CellTextsModel CellTexts { get; } = new CellTextsModel();

        public bool ShowValueText { get; set; } = true;

        public float Value { get; set; }

        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public string ValueTextFormat { get; set; }

        public event Action<float> ValueChanged;

        internal void InvokeValueChanged(float value)
        {
            ValueChanged?.Invoke(value);
        }
    }
}
