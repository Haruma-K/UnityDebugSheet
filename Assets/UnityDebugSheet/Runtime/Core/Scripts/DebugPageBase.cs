using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityDebugSheet.Runtime.Foundation.ObjectPooling;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityDebugSheet.Runtime.Foundation.TinyRecyclerView;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public abstract class DebugPageBase : Page, IRecyclerViewCellProvider, IRecyclerViewDataProvider
    {
        private readonly List<CellModel> _dataList = new List<CellModel>();
        private readonly List<int> _itemIds = new List<int>();

        private readonly Dictionary<int, string> _objIdToPrefabKeyMap = new Dictionary<int, string>();
        private readonly List<string> _prefabKeys = new List<string>();

        private readonly Dictionary<string, ObjectPool<GameObject>> _prefabPools =
            new Dictionary<string, ObjectPool<GameObject>>();

        private int _itemId;
        private string _overrideTitle;
        private GameObject _poolRoot;
        private PrefabContainer _prefabContainer;
        private RecyclerView _recyclerView;

        protected abstract string Title { get; }

        public IReadOnlyList<CellModel> DataList => _dataList;
        public IReadOnlyList<int> ItemIds => _itemIds;
        public IReadOnlyList<string> PrefabKeys => _prefabKeys;

        private RecyclerView RecyclerView
        {
            get
            {
                if (_recyclerView == null)
                {
                    _recyclerView = GetComponentInChildren<RecyclerView>();
                    _recyclerView.DataCount = 0;
                    _recyclerView.CellProvider = this;
                    _recyclerView.DataProvider = this;
                }

                return _recyclerView;
            }
        }

        protected virtual void Awake()
        {
            _poolRoot = new GameObject("PoolRoot");
            _poolRoot.transform.SetParent(transform);
            _prefabContainer = GetComponent<PrefabContainer>();
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
            foreach (var pool in _prefabPools.Values)
                pool.Clear();
        }

        GameObject IRecyclerViewCellProvider.GetCell(int dataIndex)
        {
            var prefabKey = _prefabKeys[dataIndex];
            if (!_prefabPools.TryGetValue(prefabKey, out var pool))
            {
                pool = new ObjectPool<GameObject>(() =>
                {
                    var prefab = _prefabContainer.GetPrefab(prefabKey);
                    return Instantiate(prefab);
                });
                _prefabPools.Add(prefabKey, pool);
            }

            var obj = pool.Use();
            _objIdToPrefabKeyMap[obj.GetInstanceID()] = prefabKey;
            obj.SetActive(true);
            return obj;
        }

        void IRecyclerViewCellProvider.ReleaseCell(GameObject obj)
        {
            var prefabKey = _objIdToPrefabKeyMap[obj.GetInstanceID()];
            var pool = _prefabPools[prefabKey];
            pool.Release(obj);
            obj.SetActive(false);
            obj.transform.SetParent(_poolRoot.transform);
        }

        void IRecyclerViewDataProvider.SetupCell(int dataIndex, GameObject cell)
        {
            var data = _dataList[dataIndex];
            cell.GetComponent<ICell>().Setup(data);
        }

        /// <summary>
        ///     Add a item.
        /// </summary>
        /// <remarks>
        ///     You need to call <see cref="Reload" /> after this.
        /// </remarks>
        /// <param name="prefabKey"></param>
        /// <param name="model"></param>
        /// <returns>Item ID</returns>
        public int AddItem(string prefabKey, CellModel model)
        {
            var itemId = _itemId++;
            _prefabKeys.Add(prefabKey);
            _dataList.Add(model);
            _itemIds.Add(itemId);
            RecyclerView.DataCount++;
            return itemId;
        }

        /// <summary>
        ///     Insert a item at <see cref="index" />.
        /// </summary>
        /// <remarks>
        ///     You need to call <see cref="Reload" /> after this.
        /// </remarks>
        /// <param name="prefabKey"></param>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public int InsertItem(string prefabKey, CellModel model, int index)
        {
            var itemId = _itemId++;
            _prefabKeys.Insert(index, prefabKey);
            _dataList.Insert(index, model);
            _itemIds.Insert(index, itemId);
            RecyclerView.DataCount++;
            return itemId;
        }

        /// <summary>
        ///     Remove a item.
        /// </summary>
        /// <remarks>
        ///     You need to call <see cref="Reload" /> after this.
        /// </remarks>
        /// <param name="itemId"></param>
        public void RemoveItem(int itemId)
        {
            var index = _itemIds.IndexOf(itemId);
            _dataList.RemoveAt(index);
            _prefabKeys.RemoveAt(index);
            _itemIds.RemoveAt(index);
            RecyclerView.DataCount--;
        }

        /// <summary>
        ///     Remove all items.
        /// </summary>
        public void ClearItems()
        {
            _dataList.Clear();
            _prefabKeys.Clear();
            _itemIds.Clear();
            RecyclerView.DataCount = 0;
        }

        /// <summary>
        ///     Delete and re-generate cells.
        /// </summary>
        public void Reload()
        {
            _recyclerView.Reload();
        }

        /// <summary>
        ///     Update the data of the displayed cells.
        /// </summary>
        public void RefreshData()
        {
            _recyclerView.RefreshData();
        }

        /// <summary>
        ///     Update only a data of specified index.
        /// </summary>
        /// <param name="dataIndex"></param>
        public void RefreshDataAt(int dataIndex)
        {
            _recyclerView.RefreshDataAt(dataIndex);
        }

        public void SetTitle(string title)
        {
            _overrideTitle = title;
        }

        public string GetTitle()
        {
            return string.IsNullOrEmpty(_overrideTitle) ? Title : _overrideTitle;
        }

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

        public int AddPageLinkButton<TPage>(string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, Action<TPage> onLoad = null,
            int index = -1) where TPage : DebugPageBase
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(textModel, iconModel, onLoad, index);
        }

        public int AddPageLinkButton<TPage>(CellTextsModel textModel, CellIconModel iconModel = null,
            Action<TPage> onLoad = null, int index = -1) where TPage : DebugPageBase
        {
            var useSubText = textModel != null && !string.IsNullOrEmpty(textModel.SubText);
            var buttonModel = new ButtonCellModel(useSubText);
            if (textModel != null)
            {
                buttonModel.CellTexts.Text = textModel.Text;
                buttonModel.CellTexts.TextColor = textModel.TextColor;
                buttonModel.CellTexts.SubText = textModel.SubText;
                buttonModel.CellTexts.SubTextColor = textModel.SubTextColor;
            }
            else
            {
                buttonModel.CellTexts.Text = nameof(TPage);
            }

            if (iconModel != null)
            {
                buttonModel.Icon.Sprite = iconModel.Sprite;
                buttonModel.Icon.Color = iconModel.Color;
            }

            buttonModel.Clicked += () => DebugSheet.Of(transform).PushPage(true, onLoad: onLoad);
            buttonModel.ShowArrow = true;
            return AddButton(buttonModel, index);
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
