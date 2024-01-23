using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class SearchFieldCell : Cell<SearchFieldCellModel>
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Text _placeholderText;

        protected override void SetModel(SearchFieldCellModel model)
        {
            // Cleanup
            _inputField.onValueChanged.RemoveAllListeners();
            _inputField.onEndEdit.RemoveAllListeners();
            
            _inputField.interactable = model.Interactable;
            _inputField.onValueChanged.AddListener(model.InvokeValueChanged);
            _inputField.onEndEdit.AddListener(model.InvokeSubmitted);

            _placeholderText.text = model.Placeholder;
        }
    }

    public sealed class SearchFieldCellModel : CellModel
    {
        public bool Interactable { get; set; } = true;

        public string Placeholder { get; set; } = "Search";

        public event Action<string> ValueChanged;

        public event Action<string> Submitted;

        internal void InvokeValueChanged(string searchText)
        {
            ValueChanged?.Invoke(searchText);
        }

        internal void InvokeSubmitted(string searchText)
        {
            Submitted?.Invoke(searchText);
        }
    }
}
