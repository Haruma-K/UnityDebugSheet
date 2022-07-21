#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using System.Collections;
using Demo._99_Shared.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;

namespace Demo._02_DefaultCells.Scripts
{
    public sealed class DefaultCellsDemoDebugPage : DebugPageBase
    {
        protected override string Title => "DemoPage";

        public override IEnumerator Initialize()
        {
            AddDefaultCells();
            Reload();
            yield break;
        }

        public void AddDefaultCells()
        {
            // Labels
            var labelData1 = new LabelCellModel(false);
            labelData1.CellTexts.Text = "Text";
            AddLabel(labelData1);

            var labelData2 = new LabelCellModel(false);
            labelData2.Icon.Sprite = DemoSprites.Icon.Settings;
            labelData2.CellTexts.Text = "Label With Icon";
            AddLabel(labelData2);

            var labelData3 = new LabelCellModel(true);
            labelData3.Icon.Sprite = DemoSprites.Icon.Settings;
            labelData3.CellTexts.Text = "Label With Icon And SubText";
            labelData3.CellTexts.SubText = "This is the SubText.";
            AddLabel(labelData3);

            // Buttons
            var buttonData1 = new ButtonCellModel(false);
            buttonData1.CellTexts.Text = "Button";
            buttonData1.Clicked += () => Debug.Log("Clicked");
            AddButton(buttonData1);

            var buttonData2 = new ButtonCellModel(false);
            buttonData2.CellTexts.Text = "Button With Arrow";
            buttonData2.Clicked += () => Debug.Log("Clicked");
            buttonData2.ShowArrow = true;
            AddButton(buttonData2);

            var buttonData3 = new ButtonCellModel(true);
            buttonData3.CellTexts.Text = "Button With SubText";
            buttonData3.CellTexts.SubText = "This is the SubText.";
            buttonData3.Clicked += () => Debug.Log("Clicked");
            AddButton(buttonData3);

            var buttonData4 = new ButtonCellModel(true);
            buttonData4.CellTexts.Text = "Button With SubText And Arrow";
            buttonData4.CellTexts.SubText = "This is the SubText.";
            buttonData4.Clicked += () => Debug.Log("Clicked");
            buttonData4.ShowArrow = true;
            AddButton(buttonData4);

            var buttonData5 = new ButtonCellModel(true);
            buttonData5.Icon.Sprite = DemoSprites.Icon.Settings;
            buttonData5.CellTexts.Text = "Button With SubText And Icon";
            buttonData5.CellTexts.SubText = "This is the SubText.";
            buttonData5.Clicked += () => Debug.Log("Clicked");
            AddButton(buttonData5);

            var buttonData6 = new ButtonCellModel(true);
            buttonData6.Icon.Sprite = DemoSprites.Icon.Settings;
            buttonData6.CellTexts.Text = "Button With SubText And Arrow And Icon";
            buttonData6.CellTexts.SubText = "This is the SubText.";
            buttonData6.Clicked += () => Debug.Log("Clicked");
            buttonData6.ShowArrow = true;
            AddButton(buttonData6);

            // Switch
            var toggleData1 = new SwitchCellModel(false);
            toggleData1.CellTexts.Text = "Switch";
            toggleData1.ValueChanged += x => Debug.Log($"Changed: {x}");
            AddSwitch(toggleData1);

            var toggleData2 = new SwitchCellModel(true);
            toggleData2.CellTexts.Text = "Switch With SubText";
            toggleData2.CellTexts.SubText = "This is the SubText.";
            toggleData2.ValueChanged += x => Debug.Log($"Changed: {x}");
            AddSwitch(toggleData2);

            // Sliders
            var sliderData1 = new SliderCellModel(false, 0.0f, 1.0f);
            sliderData1.CellTexts.Text = "Slider";
            sliderData1.Value = 0.5f;
            sliderData1.ValueChanged += x => Debug.Log($"Value Changed: {x}");
            AddSlider(sliderData1);

            var sliderData2 = new SliderCellModel(false, 0.0f, 1.0f);
            sliderData2.Icon.Sprite = DemoSprites.Icon.Settings;
            sliderData2.CellTexts.Text = "Slider";
            sliderData2.Value = 0.5f;
            sliderData2.ValueChanged += x => Debug.Log($"Value Changed: {x}");
            AddSlider(sliderData2);

            var sliderData3 = new SliderCellModel(false, 0.0f, 1.0f);
            sliderData3.CellTexts.Text = "Slider Without Value Text";
            sliderData3.ShowValueText = false;
            sliderData3.Value = 0.5f;
            sliderData3.ValueChanged += x => Debug.Log($"Value Changed: {x}");
            AddSlider(sliderData3);

            var sliderData4 = new SliderCellModel(true, 0.0f, 1.0f);
            sliderData4.CellTexts.Text = "Slider With SubText";
            sliderData4.CellTexts.SubText = "This is SubText.";
            sliderData4.Icon.Sprite = DemoSprites.Icon.Settings;
            sliderData4.Value = 0.5f;
            sliderData4.ValueChanged += x => Debug.Log($"Value Changed: {x}");
            AddSlider(sliderData4);

            // Pickers
            var pickerData1 = new PickerCellModel();
            pickerData1.Text = "Picker";
            pickerData1.SetOptions(new[] { "Option01", "Option02", "Option03" }, 0);
            pickerData1.Clicked += () => Debug.Log("Clicked");
            pickerData1.Confirmed += () => Debug.Log("Picking Page Closed");
            pickerData1.ActiveOptionChanged += index => Debug.Log($"Selected Option Changed: {index}");
            AddPicker(pickerData1);

            var pickerData2 = new PickerCellModel();
            pickerData2.Text = "Picker With Icon";
            pickerData2.Icon.Sprite = DemoSprites.Icon.Settings;
            pickerData2.SetOptions(new[] { "Option01", "Option02", "Option03" }, 0);
            pickerData2.Clicked += () => Debug.Log("Clicked");
            pickerData2.Confirmed += () => Debug.Log("Picking Page Closed");
            pickerData2.ActiveOptionChanged += index => Debug.Log($"Selected Option Changed: {index}");
            AddPicker(pickerData2);

            var pickerData3 = new PickerCellModel();
            pickerData3.Text = "Picker With Initial Value";
            pickerData3.SetOptions(new[] { "Option01", "Option02", "Option03" }, 2);
            pickerData3.Clicked += () => Debug.Log("Clicked");
            pickerData3.Confirmed += () => Debug.Log("Picking Page Closed");
            pickerData3.ActiveOptionChanged += index => Debug.Log($"Selected Option Changed: {index}");
            AddPicker(pickerData3);

            // Multi Pickers
            var multiPickerData1 = new MultiPickerCellModel();
            multiPickerData1.Text = "Multi Picker";
            multiPickerData1.SetOptions(new[] { "Option01", "Option02", "Option03" }, new[] { 0 });
            multiPickerData1.Clicked += () => Debug.Log("Clicked");
            multiPickerData1.Confirmed += () => Debug.Log("Picking Page Closed");
            multiPickerData1.OptionStateChanged +=
                (index, isOn) => Debug.Log($"Selected Option Changed: {index} {isOn}");
            AddMultiPicker(multiPickerData1);

            var multiPickerData2 = new MultiPickerCellModel();
            multiPickerData2.Text = "Multi Picker With Icon";
            multiPickerData2.Icon.Sprite = DemoSprites.Icon.Settings;
            multiPickerData2.SetOptions(new[] { "Option01", "Option02", "Option03" }, new[] { 1 });
            multiPickerData2.Clicked += () => Debug.Log("Clicked");
            multiPickerData2.Confirmed += () => Debug.Log("Picking Page Closed");
            multiPickerData2.OptionStateChanged +=
                (index, isOn) => Debug.Log($"Selected Option Changed: {index} {isOn}");
            AddMultiPicker(multiPickerData2);

            // Enum Picker
            var enumPickerData1 = new EnumPickerCellModel(ExampleEnum.Two);
            enumPickerData1.Text = "Enum Picker";
            enumPickerData1.Clicked += () => Debug.Log("Clicked");
            enumPickerData1.Confirmed += () => Debug.Log("Picking Page Closed");
            enumPickerData1.ActiveValueChanged += value => Debug.Log($"Selected Option Changed: {(ExampleEnum)value}");
            AddEnumPicker(enumPickerData1);

            // Enum Multi Picker
            var enumMultiPickerData1 = new EnumMultiPickerCellModel(ExampleEnum.One | ExampleEnum.Three);
            enumMultiPickerData1.Text = "Enum Multi Picker";
            enumMultiPickerData1.Clicked += () => Debug.Log("Clicked");
            enumMultiPickerData1.Confirmed += () => Debug.Log("Picking Page Closed");
            enumMultiPickerData1.ActiveValueChanged +=
                value => Debug.Log($"Selected Option Changed: {(ExampleEnum)value}");
            AddEnumMultiPicker(enumMultiPickerData1);
        }

        [Flags]
        private enum ExampleEnum
        {
            One = 1,
            Two = 1 << 2,
            Three = 2 << 3
        }
    }
}
#endif
