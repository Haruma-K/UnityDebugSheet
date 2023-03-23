using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public abstract class DefaultDebugPageBase : DebugPageBase
    {
        public int AddLabel(string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var labelCellModel = new LabelCellModel(useSubTextOrIcon);
            labelCellModel.CellTexts.Text = text;
            labelCellModel.CellTexts.SubText = subText;
            if (textColor != null) labelCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) labelCellModel.CellTexts.SubTextColor = subTextColor.Value;
            labelCellModel.Icon.Sprite = icon;
            if (iconColor != null) labelCellModel.Icon.Color = iconColor.Value;

            return AddLabel(labelCellModel, priority);
        }

        public int AddLabel(LabelCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.LabelCell, model, priority);
        }

        public int AddButton(string text, string subText = null, Color? textColor = null, Color? subTextColor = null,
            Sprite icon = null, Color? iconColor = null, bool showAllow = false, Action clicked = null,
            int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var buttonCellModel = new ButtonCellModel(useSubTextOrIcon);
            buttonCellModel.CellTexts.Text = text;
            buttonCellModel.CellTexts.SubText = subText;
            if (textColor != null) buttonCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) buttonCellModel.CellTexts.SubTextColor = subTextColor.Value;
            buttonCellModel.Icon.Sprite = icon;
            if (iconColor != null) buttonCellModel.Icon.Color = iconColor.Value;
            buttonCellModel.ShowArrow = showAllow;
            if (clicked != null) buttonCellModel.Clicked += clicked;

            return AddButton(buttonCellModel, priority);
        }

        public int AddButton(ButtonCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.ButtonCell, model, priority);
        }

        public int AddButtonCollection(ButtonCollectionCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.ButtonCollectionCell, model, priority);
        }

        public int AddSwitch(bool value, string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, Action<bool> valueChanged = null,
            int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var switchCellModel = new SwitchCellModel(useSubTextOrIcon);
            switchCellModel.CellTexts.Text = text;
            switchCellModel.CellTexts.SubText = subText;
            if (textColor != null) switchCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) switchCellModel.CellTexts.SubTextColor = subTextColor.Value;
            switchCellModel.Icon.Sprite = icon;
            if (iconColor != null) switchCellModel.Icon.Color = iconColor.Value;
            switchCellModel.Value = value;
            if (valueChanged != null) switchCellModel.ValueChanged += valueChanged;

            return AddSwitch(switchCellModel, priority);
        }

        public int AddSwitch(SwitchCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.SwitchCell, model, priority);
        }

        public int AddInputField(string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string value = null,
            string placeholder = null, InputField.ContentType contentType = InputField.ContentType.Standard,
            Action<string> valueChanged = null, int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var inputFieldCellModel = new InputFieldCellModel(useSubTextOrIcon);
            inputFieldCellModel.CellTexts.Text = text;
            inputFieldCellModel.CellTexts.SubText = subText;
            if (textColor != null) inputFieldCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) inputFieldCellModel.CellTexts.SubTextColor = subTextColor.Value;
            inputFieldCellModel.Icon.Sprite = icon;
            if (iconColor != null) inputFieldCellModel.Icon.Color = iconColor.Value;
            inputFieldCellModel.Placeholder = placeholder;
            inputFieldCellModel.Value = value;
            inputFieldCellModel.ContentType = contentType;
            if (valueChanged != null) inputFieldCellModel.ValueChanged += valueChanged;

            return AddInputField(inputFieldCellModel, priority);
        }

        public int AddInputField(InputFieldCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.InputFieldCell, model, priority);
        }

        public int AddSlider(float value, float minValue, float maxValue, string text, string subText = null,
            Color? textColor = null, Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            bool showValueText = true, string valueTextFormat = null, bool wholeNumbers = false,
            Action<float> valueChanged = null,
            int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var sliderCellModel = new SliderCellModel(useSubTextOrIcon, minValue, maxValue);
            sliderCellModel.CellTexts.Text = text;
            sliderCellModel.CellTexts.SubText = subText;
            if (textColor != null) sliderCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) sliderCellModel.CellTexts.SubTextColor = subTextColor.Value;
            sliderCellModel.Icon.Sprite = icon;
            if (iconColor != null) sliderCellModel.Icon.Color = iconColor.Value;
            sliderCellModel.Value = value;
            sliderCellModel.ShowValueText = showValueText;
            sliderCellModel.ValueTextFormat = valueTextFormat;
            sliderCellModel.WholeNumbers = wholeNumbers;
            if (valueChanged != null) sliderCellModel.ValueChanged += valueChanged;

            return AddSlider(sliderCellModel, priority);
        }

        public int AddSlider(SliderCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.SliderCell, model, priority);
        }

        public int AddPicker(IEnumerable<string> options, int activeOptionIndex, string text, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            Action<int> activeOptionChanged = null, Action clicked = null, Action confirmed = null,
            int priority = 0)
        {
            var pickerCellModel = new PickerCellModel();
            pickerCellModel.Text = text;
            if (textColor != null) pickerCellModel.TextColor = textColor.Value;
            if (subTextColor != null) pickerCellModel.SubTextColor = subTextColor.Value;
            pickerCellModel.Icon.Sprite = icon;
            if (iconColor != null) pickerCellModel.Icon.Color = iconColor.Value;
            pickerCellModel.SetOptions(options, activeOptionIndex);
            if (activeOptionChanged != null) pickerCellModel.ActiveOptionChanged += activeOptionChanged;
            if (clicked != null) pickerCellModel.Clicked += clicked;
            if (confirmed != null) pickerCellModel.Confirmed += confirmed;

            return AddPicker(pickerCellModel, priority);
        }

        public int AddPicker(PickerCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.PickerCell, model, priority);
        }

        public int AddEnumPicker(Enum activeValue, string text, Color? textColor = null, Color? subTextColor = null,
            Sprite icon = null, Color? iconColor = null, Action<Enum> activeValueChanged = null, Action clicked = null,
            Action confirmed = null, int priority = 0)
        {
            var pickerCellModel = new EnumPickerCellModel(activeValue);
            pickerCellModel.Text = text;
            if (textColor != null) pickerCellModel.TextColor = textColor.Value;
            if (subTextColor != null) pickerCellModel.SubTextColor = subTextColor.Value;
            pickerCellModel.Icon.Sprite = icon;
            if (iconColor != null) pickerCellModel.Icon.Color = iconColor.Value;
            if (activeValueChanged != null) pickerCellModel.ActiveValueChanged += activeValueChanged;
            if (clicked != null) pickerCellModel.Clicked += clicked;
            if (confirmed != null) pickerCellModel.Confirmed += confirmed;

            return AddEnumPicker(pickerCellModel, priority);
        }

        public int AddEnumPicker(EnumPickerCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.EnumPickerCell, model, priority);
        }

        public int AddMultiPicker(IEnumerable<string> options, IEnumerable<int> activeOptionIndices, string text,
            Color? textColor = null, Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            Action<int, bool> optionStateChanged = null, Action clicked = null, Action confirmed = null,
            int priority = 0)
        {
            var pickerCellModel = new MultiPickerCellModel();
            pickerCellModel.Text = text;
            if (textColor != null) pickerCellModel.TextColor = textColor.Value;
            if (subTextColor != null) pickerCellModel.SubTextColor = subTextColor.Value;
            pickerCellModel.Icon.Sprite = icon;
            if (iconColor != null) pickerCellModel.Icon.Color = iconColor.Value;
            pickerCellModel.SetOptions(options, activeOptionIndices);
            if (optionStateChanged != null) pickerCellModel.OptionStateChanged += optionStateChanged;
            if (clicked != null) pickerCellModel.Clicked += clicked;
            if (confirmed != null) pickerCellModel.Confirmed += confirmed;

            return AddMultiPicker(pickerCellModel, priority);
        }

        public int AddMultiPicker(MultiPickerCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.MultiPickerCell, model, priority);
        }

        public int AddEnumMultiPicker(Enum activeValue, string text, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            Action<Enum> activeValueChanged = null, Action clicked = null, Action confirmed = null,
            int priority = 0)
        {
            var pickerCellModel = new EnumMultiPickerCellModel(activeValue);
            pickerCellModel.Text = text;
            if (textColor != null) pickerCellModel.TextColor = textColor.Value;
            if (subTextColor != null) pickerCellModel.SubTextColor = subTextColor.Value;
            pickerCellModel.Icon.Sprite = icon;
            if (iconColor != null) pickerCellModel.Icon.Color = iconColor.Value;
            if (activeValueChanged != null) pickerCellModel.ActiveValueChanged += activeValueChanged;
            if (clicked != null) pickerCellModel.Clicked += clicked;
            if (confirmed != null) pickerCellModel.Confirmed += confirmed;

            return AddEnumMultiPicker(pickerCellModel, priority);
        }

        public int AddEnumMultiPicker(EnumMultiPickerCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.EnumMultiPickerCell, model, priority);
        }

        public int AddPickerOption(bool isOn, string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, Action<bool> toggled = null,
            int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var pickerOptionModel = new PickerOptionCellModel(useSubTextOrIcon);
            pickerOptionModel.IsOn = isOn;
            pickerOptionModel.CellTexts.Text = text;
            pickerOptionModel.CellTexts.SubText = subText;
            if (textColor != null) pickerOptionModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) pickerOptionModel.CellTexts.SubTextColor = subTextColor.Value;
            pickerOptionModel.Icon.Sprite = icon;
            if (iconColor != null) pickerOptionModel.Icon.Color = iconColor.Value;
            if (toggled != null) pickerOptionModel.Toggled += toggled;

            return AddPickerOption(pickerOptionModel, priority);
        }

        public int AddSearchField(string placeholder = null, Action<string> valueChanged = null,
            Action<string> submitted = null, int priority = 0)
        {
            var searchFieldCellModel = new SearchFieldCellModel();
            if (!string.IsNullOrEmpty(placeholder)) searchFieldCellModel.Placeholder = placeholder;
            if (valueChanged != null) searchFieldCellModel.ValueChanged += valueChanged;
            if (submitted != null) searchFieldCellModel.Submitted += submitted;

            return AddSearchField(searchFieldCellModel, priority);
        }

        public int AddSearchField(SearchFieldCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.SearchFieldCell, model, priority);
        }

        public int AddPickerOption(PickerOptionCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.PickerOption, model, priority);
        }

        public int AddPageLinkButton(string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string titleOverride = null,
            Action<(string pageId, DebugPage page)> onLoad = null, string pageId = null, int priority = 0)
        {
            return AddPageLinkButton<DebugPage>(text, subText, textColor, subTextColor, icon, iconColor,
                titleOverride, onLoad, pageId, priority);
        }

        public int AddPageLinkButton(Type pageType, string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string titleOverride = null,
            Action<(string pageId, DebugPageBase page)> onLoad = null, string pageId = null, int priority = 0)
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(pageType, textModel, iconModel, titleOverride, onLoad, pageId, priority);
        }

        public int AddPageLinkButton<TPage>(string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string titleOverride = null,
            Action<(string pageId, TPage page)> onLoad = null, string pageId = null, int priority = 0)
            where TPage : DebugPageBase
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(textModel, iconModel, titleOverride, onLoad, pageId, priority);
        }

        public int AddPageLinkButton(CellTextsModel textModel, CellIconModel iconModel = null,
            string titleOverride = null, Action<(string pageId, DebugPage page)> onLoad = null, string pageId = null,
            int priority = 0)
        {
            return AddPageLinkButton<DebugPage>(textModel, iconModel, titleOverride, onLoad, pageId, priority);
        }

        public int AddPageLinkButton(Type pageType, CellTextsModel textModel, CellIconModel iconModel = null,
            string titleOverride = null, Action<(string pageId, DebugPageBase page)> onLoad = null,
            string pageId = null, int priority = 0)
        {
            return AddPageLinkButton(pageType, null, textModel, iconModel, titleOverride, onLoad, pageId, priority);
        }

        public int AddPageLinkButton<TPage>(CellTextsModel textModel, CellIconModel iconModel = null,
            string titleOverride = null, Action<(string pageId, TPage page)> onLoad = null, string pageId = null,
            int priority = 0)
            where TPage : DebugPageBase
        {
            return AddPageLinkButton(null, textModel, iconModel, titleOverride, onLoad, pageId, priority);
        }

        public int AddPageLinkButton(Type pageType, DebugPageBase prefab, string text, string subText = null,
            Color? textColor = null, Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            string titleOverride = null, Action<(string pageId, DebugPageBase page)> onLoad = null,
            string pageId = null, int priority = 0)
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(pageType, prefab, textModel, iconModel, titleOverride, onLoad, pageId, priority);
        }

        public int AddPageLinkButton<TPage>(TPage prefab, string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string titleOverride = null,
            Action<(string pageId, TPage page)> onLoad = null, string pageId = null, int priority = 0)
            where TPage : DebugPageBase
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(prefab, textModel, iconModel, titleOverride, onLoad, pageId, priority);
        }

        public int AddPageLinkButton(Type pageType, DebugPageBase prefab, CellTextsModel textModel,
            CellIconModel iconModel = null, string titleOverride = null,
            Action<(string pageId, DebugPageBase page)> onLoad = null,
            string pageId = null, int priority = 0)
        {
            var useSubText = textModel != null && !string.IsNullOrEmpty(textModel.SubText);
            var useIcon = iconModel != null && iconModel.Sprite != null;
            var useSubTextOrIcon = useSubText || useIcon;
            var buttonModel = new PageLinkButtonCellModel(useSubTextOrIcon);
            if (textModel != null)
            {
                buttonModel.CellTexts.Text = textModel.Text;
                buttonModel.CellTexts.TextColor = textModel.TextColor;
                buttonModel.CellTexts.SubText = textModel.SubText;
                buttonModel.CellTexts.SubTextColor = textModel.SubTextColor;
            }
            else
            {
                buttonModel.CellTexts.Text = pageType.Name;
            }

            if (iconModel != null)
            {
                buttonModel.Icon.Sprite = iconModel.Sprite;
                buttonModel.Icon.Color = iconModel.Color;
            }

            buttonModel.PageType = pageType;
            buttonModel.Prefab = prefab;
            buttonModel.PageTitleOverride = titleOverride;
            buttonModel.OnLoad += onLoad;
            buttonModel.ShowArrow = true;
            buttonModel.PageId = pageId;
            return AddPageLinkButton(buttonModel, priority);
        }

        public int AddPageLinkButton<TPage>(TPage prefab, CellTextsModel textModel, CellIconModel iconModel = null,
            string titleOverride = null, Action<(string pageId, TPage page)> onLoad = null, string pageId = null,
            int priority = 0)
            where TPage : DebugPageBase
        {
            var useSubText = textModel != null && !string.IsNullOrEmpty(textModel.SubText);
            var useIcon = iconModel != null && iconModel.Sprite != null;
            var useSubTextOrIcon = useSubText || useIcon;
            var buttonModel = new PageLinkButtonCellModel(useSubTextOrIcon);
            if (textModel != null)
            {
                buttonModel.CellTexts.Text = textModel.Text;
                buttonModel.CellTexts.TextColor = textModel.TextColor;
                buttonModel.CellTexts.SubText = textModel.SubText;
                buttonModel.CellTexts.SubTextColor = textModel.SubTextColor;
            }
            else
            {
                buttonModel.CellTexts.Text = nameof(TPage);
            }

            if (iconModel != null)
            {
                buttonModel.Icon.Sprite = iconModel.Sprite;
                buttonModel.Icon.Color = iconModel.Color;
            }

            buttonModel.PageType = typeof(TPage);
            buttonModel.Prefab = prefab;
            buttonModel.PageTitleOverride = titleOverride;
            buttonModel.OnLoad += x => onLoad?.Invoke((x.pageId, (TPage)x.page));
            buttonModel.ShowArrow = true;
            buttonModel.PageId = pageId;
            return AddPageLinkButton(buttonModel, priority);
        }

        public int AddPageLinkButton(PageLinkButtonCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.PageLinkButtonCell, model, priority);
        }
    }
}
