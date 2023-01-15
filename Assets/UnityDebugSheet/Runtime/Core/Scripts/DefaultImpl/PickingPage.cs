using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#else
using System.Collections;
#endif

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl
{
    public sealed class PickingPage : DefaultDebugPageBase
    {
        private readonly List<PickerOptionCellModel> _optionDataList = new List<PickerOptionCellModel>();

        public int ActiveIndex { get; private set; }
        public IReadOnlyList<PickerOptionCellModel> OptionDataList => _optionDataList;

        protected override string Title => "Select";

        public event Action<int> ValueChanged;

        private IReadOnlyList<string> _options;

        public void Setup(IReadOnlyList<string> options, int selectedIndex)
        {
            _options = options;
            ActiveIndex = selectedIndex;

            ClearItems();
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            _optionDataList.Clear();

            var index = 0;
            foreach (var option in _options)
            {
                var indexCache = index;
                var isOn = index == ActiveIndex;
                var pickerOptionModel = new PickerOptionCellModel(false);
                pickerOptionModel.CellTexts.Text = option;
                pickerOptionModel.IsOn = isOn;
                pickerOptionModel.Toggled += x =>
                {
                    // User cannot make the value false because the picker always must have the selected option.
                    // So if false is set by user operation, set true immediately.
                    if (!x)
                    {
                        pickerOptionModel.IsOn = true;
                        RefreshData();
                        return;
                    }

                    OnSelectionChanged(indexCache);
                };
                AddPickerOption(pickerOptionModel);
                _optionDataList.Add(pickerOptionModel);
                index++;
            }

            Reload();

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

        private void OnSelectionChanged(int selectedIndex)
        {
            SetActiveIndex(selectedIndex);
            ValueChanged?.Invoke(selectedIndex);
            RefreshData();
        }

        public void SetActiveIndex(int index)
        {
            for (var i = 0; i < _optionDataList.Count; i++)
            {
                var data = _optionDataList[i];
                data.IsOn = index == i;
            }

            ActiveIndex = index;
        }
    }
}
