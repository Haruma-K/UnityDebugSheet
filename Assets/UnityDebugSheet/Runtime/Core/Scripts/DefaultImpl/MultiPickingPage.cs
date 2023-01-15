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
    public sealed class MultiPickingPage : DefaultDebugPageBase
    {
        private readonly List<int> _activeOptionIndices = new List<int>();

        private IReadOnlyList<string> _options;

        public IReadOnlyList<int> ActiveOptionIndices => _activeOptionIndices;

        protected override string Title => "Select";
        public event Action<(int index, bool isActive)> OptionActiveStateChanged;

        public void Setup(IReadOnlyList<string> options, IReadOnlyList<int> activeOptionIndices)
        {
            _options = options;
            _activeOptionIndices.Clear();
            _activeOptionIndices.AddRange(activeOptionIndices);
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            ClearItems();
            for (var i = 0; i < _options.Count; i++)
            {
                var option = _options[i];
                var index = i;
                var isActive = _activeOptionIndices.Contains(index);

                var pickerOptionData = new PickerOptionCellModel(false);
                pickerOptionData.CellTexts.Text = option;
                pickerOptionData.IsOn = isActive;
                pickerOptionData.Toggled += x =>
                {
                    SetOptionState(index, x);
                    OptionActiveStateChanged?.Invoke((index, x));
                };
                AddPickerOption(pickerOptionData);
            }

            Reload();

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

        public void SetOptionState(int optionIndex, bool isActive)
        {
            if (isActive)
                _activeOptionIndices.Add(optionIndex);
            else
                _activeOptionIndices.Remove(optionIndex);
        }
    }
}
