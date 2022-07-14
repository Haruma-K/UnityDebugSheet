using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl
{
    public abstract class DefaultDebugPageBase : DebugPageBase
    {
        public int AddLabel(LabelCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.LabelCell, model, index);
        }

        public int AddButton(ButtonCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.ButtonCell, model, index);
        }

        public int AddSwitch(SwitchCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.SwitchCell, model, index);
        }

        public int AddSlider(SliderCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.SliderCell, model, index);
        }

        public int AddPicker(PickerCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.PickerCell, model, index);
        }

        public int AddEnumPicker(EnumPickerCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.EnumPickerCell, model, index);
        }

        public int AddMultiPicker(MultiPickerCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.MultiPickerCell, model, index);
        }

        public int AddEnumMultiPicker(EnumMultiPickerCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.EnumMultiPickerCell, model, index);
        }

        public int AddPickerOption(PickerOptionCellModel model, int index = -1)
        {
            return AddSeparatedItem(AssetKeys.PickerOption, model, index);
        }

        /// <summary>
        ///     Add a item. Also add a separator before if it is not the first item.
        /// </summary>
        /// <param name="prefabKey"></param>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns>Item ID</returns>
        protected int AddSeparatedItem(string prefabKey, CellModel model, int index = -1)
        {
            var isLastItem = index == -1 || index == DataList.Count - 1;

            return isLastItem
                ? AddItem(prefabKey, model)
                : InsertItem(prefabKey, model, index);
        }
    }
}
