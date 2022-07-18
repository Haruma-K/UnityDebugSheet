using System;
using System.Collections.Generic;
using System.Linq;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl
{
    public sealed class MultiPickingPage : DebugPageBase
    {
        private readonly List<int> _activeOptionIndices = new List<int>();

        public IReadOnlyList<int> ActiveOptionIndices => _activeOptionIndices;

        protected override string Title => "Select";
        public event Action<(int index, bool isActive)> OptionActiveStateChanged;

        public void Setup(IReadOnlyList<string> options, IReadOnlyList<int> activeOptionIndices)
        {
            ClearItems();
            _activeOptionIndices.Clear();
            _activeOptionIndices.AddRange(activeOptionIndices);

            for (var i = 0; i < options.Count; i++)
            {
                var option = options[i];
                var index = i;
                var isActive = activeOptionIndices.Contains(index);

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
