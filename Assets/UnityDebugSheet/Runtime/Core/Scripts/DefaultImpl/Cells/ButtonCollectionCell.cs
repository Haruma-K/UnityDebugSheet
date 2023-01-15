using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class ButtonCollectionCell : Cell<ButtonCollectionCellModel>
    {
        [SerializeField] private LayoutElement _layoutElement;
        [SerializeField] private RectTransform _contents;
        [SerializeField] private CanvasGroup _contentsCanvasGroup;
        [SerializeField] private GridLayoutGroup _gridLayoutGroup;

        public CollectionButton buttonPrefab;

        protected override void SetModel(ButtonCollectionCellModel model)
        {
            //_contentsCanvasGroup.alpha = model.Interactable ? 1.0f : 0.3f;

            // Remove all buttons
            while (_gridLayoutGroup.transform.childCount > 0)
            {
                var child = _gridLayoutGroup.transform.GetChild(0);
                child.SetParent(null);
                Destroy(child.gameObject);
            }

            // Buttons
            foreach (var buttonModel in model.Buttons)
            {
                var button = Instantiate(buttonPrefab, _gridLayoutGroup.transform);
                button.Setup(buttonModel);
            }

            // Height
            var buttonCount = model.Buttons.Count;
            var rowCount = Mathf.CeilToInt(buttonCount / (float) _gridLayoutGroup.constraintCount);
            var height = _gridLayoutGroup.padding.top + _gridLayoutGroup.padding.bottom;
            height += (int)(rowCount * _gridLayoutGroup.cellSize.y);
            height += (int)(_gridLayoutGroup.spacing.y * (rowCount - 1));
            height += 1; // Border
            _contents.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            _layoutElement.preferredHeight = height; // Set the preferred height for the recycler view.
        }
    }

    public sealed class ButtonCollectionCellModel : CellModel
    {
        public List<CollectionButtonModel> Buttons { get; } = new List<CollectionButtonModel>();
    }
}
