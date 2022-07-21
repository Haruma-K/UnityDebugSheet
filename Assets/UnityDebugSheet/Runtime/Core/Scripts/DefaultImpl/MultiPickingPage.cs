using System;
using System.Collections;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl
{
    public sealed class MultiPickingPage : DebugPageBase
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

        public override IEnumerator Initialize()
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

            yield break;
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
