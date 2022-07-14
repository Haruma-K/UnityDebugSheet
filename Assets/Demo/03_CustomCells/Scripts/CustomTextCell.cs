#if !EXCLUDE_UNITY_DEBUG_SHEET
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Demo._03_CustomCells.Scripts
{
    public sealed class CustomTextCell : Cell<CustomTextCellModel>
    {
        [SerializeField] private Text _text;
        [SerializeField] private LayoutElement _layoutElement;

        private const int Padding = 36;

        protected override void SetModel(CustomTextCellModel model)
        {
            _text.text = model.Text;
            _text.color = model.Color;
            _layoutElement.preferredHeight = _text.preferredHeight + Padding;
        }
    }

    public sealed class CustomTextCellModel : CellModel
    {
        public string Text { get; set; }
        
        public Color Color { get; set; } = Color.black;
    }
}
#endif
