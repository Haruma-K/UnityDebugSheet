using System.Collections.Generic;
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
        private readonly List<string> _prefabKeys = new List<string>();

        private readonly Dictionary<string, ObjectPool<GameObject>> _prefabPools =
            new Dictionary<string, ObjectPool<GameObject>>();

        private readonly Dictionary<int, string> _objIdToPrefabKeyMap = new Dictionary<int, string>();

        private int _itemId;
        private GameObject _poolRoot;
        private RecyclerView _recyclerView;
        private PrefabContainer _prefabContainer;
        private string _overrideTitle;

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
    }
}
