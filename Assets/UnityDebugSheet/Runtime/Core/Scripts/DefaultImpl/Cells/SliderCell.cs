using System;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class SliderCell : Cell<SliderCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private RectTransform _contents;
        [SerializeField] private RectTransform _containerTrans;
        [SerializeField] private RectTransform _topTrans;
        [SerializeField] private CanvasGroup _contentsCanvasGroup;

        public CellIcon icon;
        public CellTexts cellTexts;
        public InputField valueField;
        public Slider slider;

        protected override void SetModel(SliderCellModel model)
        {
            _contentsCanvasGroup.alpha = model.Interactable ? 1.0f : 0.3f;
            
            // Cleanup
            valueField.onValueChanged.RemoveAllListeners();
            slider.onValueChanged.RemoveAllListeners();

            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            // Texts
            cellTexts.Setup(model.CellTexts);

            var value = Mathf.Clamp(model.Value, model.MinValue, model.MaxValue);

            // TextField
            var valueTextFormat = string.IsNullOrEmpty(model.ValueTextFormat) ? "F2" : model.ValueTextFormat;
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
            slider.interactable = model.Interactable;
            slider.minValue = model.MinValue;
            slider.maxValue = model.MaxValue;
            slider.SetValueWithoutNotify(value);
            slider.wholeNumbers = model.WholeNumbers;
            slider.onValueChanged.AddListener(x =>
            {
                valueField.SetTextWithoutNotify(x.ToString(valueTextFormat));
                model.Value = x;
                model.InvokeValueChanged(x);
            });

            // Height
            //var height = ((RectTransform)cellTexts.transform).rect.height + 36;
            var height = model.UseSubTextOrIcon ? 68 : 42; // Texts
            height += 36; // Padding
            _topTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            height += 65; // Bottom is fixed size.
            _containerTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            height += 1; // Border
            _contents.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _layoutElement.preferredHeight = height; // Set the preferred height for the recycler view.
        }
    }

    public sealed class SliderCellModel : CellModel
    {
        public SliderCellModel(bool useSubTextOrIcon, float minValue, float maxValue)
        {
            UseSubTextOrIcon = useSubTextOrIcon;
            MinValue = minValue;
            MaxValue = maxValue;
        }

        public CellIconModel Icon { get; } = new CellIconModel();

        public bool UseSubTextOrIcon { get; }

        public CellTextsModel CellTexts { get; } = new CellTextsModel();

        public bool ShowValueText { get; set; } = true;

        public float Value { get; set; }

        public float MinValue { get; set; }

        public float MaxValue { get; set; }

        public string ValueTextFormat { get; set; }

        public bool WholeNumbers { get; set; }

        public bool Interactable { get; set; } = true;

        public event Action<float> ValueChanged;

        internal void InvokeValueChanged(float value)
        {
            ValueChanged?.Invoke(value);
        }
    }
}
