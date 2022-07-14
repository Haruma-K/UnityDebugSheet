using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Foundation.TinyRecyclerView
{
    public sealed class RecyclerView : RecyclerViewBase
    {
        [SerializeField] private HorizontalOrVerticalLayoutGroup _layoutGroup;

        private readonly Dictionary<int, float> _cellSizes = new Dictionary<int, float>();

        private float _cellSizeAverage;

        public override LayoutGroup LayoutGroup
        {
            get => _layoutGroup;
            set => _layoutGroup = value as HorizontalOrVerticalLayoutGroup;
        }

        public override float Spacing => _layoutGroup.spacing;

        protected override float CalculateBeforeInvisibleAreaSize(float viewportPosMin, out int cellCount)
        {
            cellCount = 0;
            var areaSize = (float)BeforePadding;
            while (cellCount < DataCount)
            {
                // Because the cell work with auto layout, create the cell.
                if (!_cellSizes.ContainsKey(cellCount))
                    CreateCell(cellCount);

                var cellSize = _cellSizes[cellCount];

                if (areaSize + cellSize + Spacing >= viewportPosMin)
                    break;

                areaSize += cellSize + Spacing;
                cellCount++;
            }

            return areaSize;
        }

        protected override float CalculateAfterInvisibleAreaSize(int startDataIndex)
        {
            var areaSize = (float)AfterPadding;
            for (var i = startDataIndex; i < DataCount; i++)
            {
                if (_cellSizes.TryGetValue(i, out var cellSize))
                    areaSize += cellSize;
                else
                    areaSize += _cellSizeAverage;

                areaSize += Spacing;
            }

            return areaSize;
        }

        protected override float GetCellSize(int dataIndex)
        {
            return _cellSizes[dataIndex];
        }

        protected override int GetCellCountInGroup()
        {
            return 1;
        }

        protected override GameObject CreateCell(int dataIndex)
        {
            var cell = base.CreateCell(dataIndex);
            var cellTrans = (RectTransform)cell.transform;
            var isHorizontal = ScrollDirection == ScrollDirection.Horizontal;
            var layoutElement = cell.GetComponent<ILayoutElement>();

            if (layoutElement == null)
                throw new Exception("RecyclerView cells must have the ILayoutElement.");

            var cellSize = isHorizontal
                ? LayoutUtility.GetPreferredWidth(cellTrans)
                : LayoutUtility.GetPreferredHeight(cellTrans);
            
            _cellSizes[dataIndex] = cellSize;
            _cellSizeAverage = _cellSizes.Values.Average();
            return cell;
        }

        protected override void Cleanup()
        {
            base.Cleanup();
            _cellSizes.Clear();
            _cellSizeAverage = 0.0f;
        }
    }
}
