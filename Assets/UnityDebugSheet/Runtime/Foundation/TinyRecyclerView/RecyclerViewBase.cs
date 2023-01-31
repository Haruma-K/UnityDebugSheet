using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Foundation.TinyRecyclerView
{
    [RequireComponent(typeof(ScrollRect))]
    public abstract class RecyclerViewBase : MonoBehaviour
    {
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private ScrollDirection _scrollDirection;

        private CellManager _cellManager;

        public abstract LayoutGroup LayoutGroup { get; set; }

        public IRecyclerViewCellProvider CellProvider { get; set; }
        public IRecyclerViewDataProvider DataProvider { get; set; }
        public int BeforePadding { get; set; }
        public int AfterPadding { get; set; }
        public int DataCount { get; set; }

        public abstract float Spacing { get; }

        public ScrollRect ScrollRect
        {
            get => _scrollRect;
            set => _scrollRect = value;
        }

        public ScrollDirection ScrollDirection
        {
            get => _scrollDirection;
            set => _scrollDirection = value;
        }

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
            var content = _scrollRect.content;
            _cellManager = new CellManager(content);

            BeforePadding += _scrollDirection == ScrollDirection.Horizontal
                ? LayoutGroup.padding.left
                : LayoutGroup.padding.top;
            AfterPadding += _scrollDirection == ScrollDirection.Horizontal
                ? LayoutGroup.padding.right
                : LayoutGroup.padding.bottom;

            OnAwake();

            Assert.IsTrue(_scrollRect.content.childCount == 0);
        }

        private void OnEnable()
        {
            _scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        }

        private void OnDisable()
        {
            _scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }

        protected virtual void OnValidate()
        {
        }

        protected virtual void OnAwake()
        {
        }

        protected abstract float CalculateBeforeInvisibleAreaSize(float viewportPosMin, out int cellCount);

        protected abstract float CalculateAfterInvisibleAreaSize(int startDataIndex);

        protected abstract float GetCellSize(int dataIndex);

        /// <summary>
        ///     Get cell count in a cell group.
        ///     In the grid layout, cells with the same position belong to the same cell group.
        /// </summary>
        /// <returns></returns>
        protected abstract int GetCellCountInGroup();

        protected virtual GameObject CreateCell(int dataIndex)
        {
            var obj = CellProvider.GetCell(dataIndex);
            DataProvider.SetupCell(dataIndex, obj);
            _cellManager.AddCell(dataIndex, obj.transform);
            return obj;
        }

        public void UpdateContents()
        {
            if (CellProvider == null || DataProvider == null)
                return;

            var isHorizontal = _scrollDirection == ScrollDirection.Horizontal;
            var rectTrans = (RectTransform)transform;
            var viewportSize = isHorizontal ? rectTrans.rect.width : rectTrans.rect.height;
            var viewportPosMin = isHorizontal
                ? -_scrollRect.content.anchoredPosition.x
                : _scrollRect.content.anchoredPosition.y;
            var viewportPosMax = viewportPosMin + viewportSize;

            // Calculate the invisible area size before the viewport.
            var beforeInvisibleAreaSize = CalculateBeforeInvisibleAreaSize(viewportPosMin, out var beforeCellCount);
            var position = beforeInvisibleAreaSize;
            var dataIndex = beforeCellCount;

            // Create the visible cells.
            var visibleDataIndices = new List<int>();
            var cellCountInGroup = GetCellCountInGroup();
            var cellIndexInGroup = 0;
            var spacing = Spacing;
            while (dataIndex < DataCount)
            {
                if (!_cellManager.Contains(dataIndex))
                    CreateCell(dataIndex);

                visibleDataIndices.Add(dataIndex);

                cellIndexInGroup++;
                if (cellIndexInGroup == cellCountInGroup)
                {
                    // Calculate the next cell position.
                    position += GetCellSize(dataIndex) + spacing;

                    // If the next cell is out of the drawing area, break.
                    if (position > viewportPosMax)
                        break;

                    cellIndexInGroup = 0;
                }

                dataIndex++;
            }

            // Calculate the invisible area size after the viewport.
            var afterInvisibleAreaSize = CalculateAfterInvisibleAreaSize(dataIndex);

            // Remove all invisible cells.
            foreach (var i in _cellManager.DataIndices.ToArray())
                if (!visibleDataIndices.Contains(i))
                {
                    var cell = _cellManager.GetCell(i);
                    _cellManager.RemoveCell(i);
                    CellProvider.ReleaseCell(cell.gameObject);
                }

            // Apply the invisible area size calculated above.
            ApplyInvisibleAreaSize((int)beforeInvisibleAreaSize, (int)afterInvisibleAreaSize);
        }

        /// <summary>
        ///     Update only the data of the displayed cells.
        /// </summary>
        public void RefreshData()
        {
            foreach (var dataIndex in _cellManager.DataIndices)
            {
                var cellTrans = _cellManager.GetCell(dataIndex);
                DataProvider.SetupCell(dataIndex, cellTrans.gameObject);
            }
        }

        /// <summary>
        ///     Update only a data of specified index.
        /// </summary>
        /// <param name="dataIndex"></param>
        public void RefreshDataAt(int dataIndex)
        {
            if (!_cellManager.Contains(dataIndex))
                return;

            var cellTrans = _cellManager.GetCell(dataIndex);
            DataProvider.SetupCell(dataIndex, cellTrans.gameObject);
        }

        /// <summary>
        ///     Delete and re-generate cells.
        /// </summary>
        public void Reload()
        {
            Cleanup();
            UpdateContents();
        }

        /// <summary>
        ///     Get the instance of the cell at the specified data index.
        /// </summary>
        /// <param name="dataIndex"></param>
        /// <returns>GameObject if exits, null if not.</returns>
        public GameObject GetCellIfExists(int dataIndex)
        {
            if (!_cellManager.Contains(dataIndex))
                return null;

            return _cellManager.GetCell(dataIndex).gameObject;
        }

        protected virtual void Cleanup()
        {
            foreach (var dataIndex in _cellManager.DataIndices)
            {
                var cellTrans = _cellManager.GetCell(dataIndex);
                CellProvider.ReleaseCell(cellTrans.gameObject);
            }

            _cellManager.ClearCells();
        }

        public void SetNormalizedPosition(float normalizedPosition)
        {
            var pos = _scrollRect.normalizedPosition;
            if (_scrollDirection == ScrollDirection.Horizontal)
                pos.x = normalizedPosition;
            else
                pos.y = 1.0f - normalizedPosition;
            _scrollRect.normalizedPosition = pos;
        }

        private void OnScrollValueChanged(Vector2 _)
        {
            UpdateContents();
        }

        private void ApplyInvisibleAreaSize(int beforeSize, int afterSize)
        {
            if (_scrollDirection == ScrollDirection.Horizontal)
            {
                LayoutGroup.padding.left = beforeSize;
                LayoutGroup.padding.right = afterSize;
            }
            else
            {
                LayoutGroup.padding.top = beforeSize;
                LayoutGroup.padding.bottom = afterSize;
            }
        }

        private sealed class CellManager
        {
            private readonly List<(int dataIndex, Transform trans)> _children =
                new List<(int dataIndex, Transform trans)>();

            private readonly List<int> _dataIndices = new List<int>();

            private readonly Transform _parent;

            public CellManager(Transform parent)
            {
                _parent = parent;
            }

            public IReadOnlyList<int> DataIndices => _dataIndices;

            public bool Contains(int dataIndex)
            {
                return _dataIndices.Contains(dataIndex);
            }

            public Transform GetCell(int dataIndex)
            {
                var childIndex = _dataIndices.IndexOf(dataIndex);
                return _children[childIndex].trans;
            }

            public void AddCell(int dataIndex, Transform trans)
            {
                if (Contains(dataIndex))
                    throw new InvalidOperationException($"Data Index {dataIndex} is already added.");

                var childIndex = 0;
                for (var i = 0; i < _children.Count; i++)
                {
                    var child = _children[i];

                    if (child.dataIndex < dataIndex)
                        childIndex++;
                    else
                        break;
                }

                trans.SetParent(_parent, false);
                trans.SetSiblingIndex(childIndex);
                _dataIndices.Insert(childIndex, dataIndex);
                _children.Insert(childIndex, (dataIndex, trans));
            }

            public void RemoveCell(int dataIndex)
            {
                var childIndex = _dataIndices.IndexOf(dataIndex);
                _children.RemoveAt(childIndex);
                _dataIndices.RemoveAt(childIndex);
            }

            public void ClearCells()
            {
                _children.Clear();
                _dataIndices.Clear();
            }
        }
    }
}
