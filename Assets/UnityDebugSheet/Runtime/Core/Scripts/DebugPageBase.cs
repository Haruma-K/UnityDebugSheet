using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityDebugSheet.Runtime.Foundation.ObjectPooling;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules;
using UnityDebugSheet.Runtime.Foundation.TinyRecyclerView;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public abstract class DebugPageBase : Page, IRecyclerViewCellProvider, IRecyclerViewDataProvider
    {
        private readonly PriorityList<int> _itemIds = new PriorityList<int>();
        private readonly Dictionary<int, int> _itemIdToDataIndexMap = new Dictionary<int, int>();
        private readonly List<ItemInfo> _itemInfos = new List<ItemInfo>();
        private readonly Dictionary<int, string> _objIdToPrefabKeyMap = new Dictionary<int, string>();

        private readonly Dictionary<string, ObjectPool<GameObject>> _prefabPools =
            new Dictionary<string, ObjectPool<GameObject>>();

        private bool _addedOrRemovedInThisFrame;

        private int _currentItemId;
        private string _overrideTitle;
        private GameObject _poolRoot;
        private PrefabContainer _prefabContainer;
        private RecyclerView _recyclerView;

        protected abstract string Title { get; }

        public IReadOnlyList<ItemInfo> ItemInfos => _itemInfos;

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
            // Add padding for the safe area.
            var canvasScaleFactor = GetComponentInParent<Canvas>().scaleFactor;
            _recyclerView.AfterPadding += (int)(Screen.safeArea.y / canvasScaleFactor);
        }

        protected virtual void LateUpdate()
        {
            if (_addedOrRemovedInThisFrame)
                Reload();

            _addedOrRemovedInThisFrame = false;
        }

        protected virtual void OnDestroy()
        {
            foreach (var pool in _prefabPools.Values)
                pool.Clear();
        }

        GameObject IRecyclerViewCellProvider.GetCell(int dataIndex)
        {
            var prefabKey = _itemInfos[dataIndex].PrefabKey;
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
            var data = _itemInfos[dataIndex].CellModel;
            cell.GetComponent<ICell>().Setup(data);
        }

        /// <summary>
        ///     Get the instance of the cell at the specified item id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns>GameObject if exits, null if not.</returns>
        public GameObject GetCellIfExists(int itemId)
        {
            if (!_itemIdToDataIndexMap.TryGetValue(itemId, out var dataIndex))
                return null;

            return RecyclerView.GetCellIfExists(dataIndex);
        }

        /// <summary>
        ///     Add a item.
        /// </summary>
        /// <param name="prefabKey"></param>
        /// <param name="model"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        public int AddItem(string prefabKey, CellModel model, int priority = 0)
        {
            var itemId = _currentItemId++;
            var node = _itemIds.Add(itemId, priority);
            var previousItemId = node.Previous?.Value.Item;
            var index = previousItemId.HasValue
                ? _itemInfos.FindIndex(x => x.ItemId == previousItemId.Value) + 1
                : 0;

            _itemInfos.Insert(index, new ItemInfo(itemId, prefabKey, model));
            _itemIdToDataIndexMap[itemId] = index;
            RecyclerView.DataCount++;
            _addedOrRemovedInThisFrame = true;
            return itemId;
        }

        /// <summary>
        ///     Remove a item.
        /// </summary>
        /// <param name="itemId"></param>
        public void RemoveItem(int itemId)
        {
            var info = _itemInfos.Find(x => x.ItemId == itemId);
            _itemIds.Remove(info.ItemId);
            _itemIdToDataIndexMap.Remove(info.ItemId);
            _itemInfos.Remove(info);
            RecyclerView.DataCount--;
            _addedOrRemovedInThisFrame = true;
        }

        /// <summary>
        ///     Remove all items.
        /// </summary>
        public void ClearItems()
        {
            _itemIds.Clear();
            _itemInfos.Clear();
            _itemIdToDataIndexMap.Clear();
            RecyclerView.DataCount = 0;
            _addedOrRemovedInThisFrame = true;
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

        public int AddLabel(string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var labelCellModel = new LabelCellModel(useSubTextOrIcon);
            labelCellModel.CellTexts.Text = text;
            labelCellModel.CellTexts.SubText = subText;
            if (textColor != null) labelCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) labelCellModel.CellTexts.SubTextColor = subTextColor.Value;
            labelCellModel.Icon.Sprite = icon;
            if (iconColor != null) labelCellModel.Icon.Color = iconColor.Value;

            return AddLabel(labelCellModel, priority);
        }

        public int AddLabel(LabelCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.LabelCell, model, priority);
        }

        public int AddButton(string text, string subText = null, Color? textColor = null, Color? subTextColor = null,
            Sprite icon = null, Color? iconColor = null, bool showAllow = false, Action clicked = null,
            int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var buttonCellModel = new ButtonCellModel(useSubTextOrIcon);
            buttonCellModel.CellTexts.Text = text;
            buttonCellModel.CellTexts.SubText = subText;
            if (textColor != null) buttonCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) buttonCellModel.CellTexts.SubTextColor = subTextColor.Value;
            buttonCellModel.Icon.Sprite = icon;
            if (iconColor != null) buttonCellModel.Icon.Color = iconColor.Value;
            buttonCellModel.ShowArrow = showAllow;
            if (clicked != null) buttonCellModel.Clicked += clicked;

            return AddButton(buttonCellModel, priority);
        }

        public int AddButton(ButtonCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.ButtonCell, model, priority);
        }

        public int AddSwitch(bool value, string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, Action<bool> valueChanged = null,
            int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var switchCellModel = new SwitchCellModel(useSubTextOrIcon);
            switchCellModel.CellTexts.Text = text;
            switchCellModel.CellTexts.SubText = subText;
            if (textColor != null) switchCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) switchCellModel.CellTexts.SubTextColor = subTextColor.Value;
            switchCellModel.Icon.Sprite = icon;
            if (iconColor != null) switchCellModel.Icon.Color = iconColor.Value;
            switchCellModel.Value = value;
            if (valueChanged != null) switchCellModel.ValueChanged += valueChanged;

            return AddSwitch(switchCellModel, priority);
        }

        public int AddSwitch(SwitchCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.SwitchCell, model, priority);
        }

        public int AddSlider(float value, float minValue, float maxValue, string text, string subText = null,
            Color? textColor = null, Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            bool showValueText = true, string valueTextFormat = null, Action<float> valueChanged = null,
            int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var sliderCellModel = new SliderCellModel(useSubTextOrIcon, minValue, maxValue);
            sliderCellModel.CellTexts.Text = text;
            sliderCellModel.CellTexts.SubText = subText;
            if (textColor != null) sliderCellModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) sliderCellModel.CellTexts.SubTextColor = subTextColor.Value;
            sliderCellModel.Icon.Sprite = icon;
            if (iconColor != null) sliderCellModel.Icon.Color = iconColor.Value;
            sliderCellModel.Value = value;
            sliderCellModel.ShowValueText = showValueText;
            sliderCellModel.ValueTextFormat = valueTextFormat;
            if (valueChanged != null) sliderCellModel.ValueChanged += valueChanged;

            return AddSlider(sliderCellModel, priority);
        }

        public int AddSlider(SliderCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.SliderCell, model, priority);
        }

        public int AddPicker(IEnumerable<string> options, int activeOptionIndex, string text, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            Action<int> activeOptionChanged = null, Action clicked = null, Action confirmed = null,
            int priority = 0)
        {
            var pickerCellModel = new PickerCellModel();
            pickerCellModel.Text = text;
            if (textColor != null) pickerCellModel.TextColor = textColor.Value;
            if (subTextColor != null) pickerCellModel.SubTextColor = subTextColor.Value;
            pickerCellModel.Icon.Sprite = icon;
            if (iconColor != null) pickerCellModel.Icon.Color = iconColor.Value;
            pickerCellModel.SetOptions(options, activeOptionIndex);
            if (activeOptionChanged != null) pickerCellModel.ActiveOptionChanged += activeOptionChanged;
            if (clicked != null) pickerCellModel.Clicked += clicked;
            if (confirmed != null) pickerCellModel.Confirmed += confirmed;

            return AddPicker(pickerCellModel, priority);
        }

        public int AddPicker(PickerCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.PickerCell, model, priority);
        }

        public int AddEnumPicker(Enum activeValue, string text, Color? textColor = null, Color? subTextColor = null,
            Sprite icon = null, Color? iconColor = null, Action<Enum> activeValueChanged = null, Action clicked = null,
            Action confirmed = null, int priority = 0)
        {
            var pickerCellModel = new EnumPickerCellModel(activeValue);
            pickerCellModel.Text = text;
            if (textColor != null) pickerCellModel.TextColor = textColor.Value;
            if (subTextColor != null) pickerCellModel.SubTextColor = subTextColor.Value;
            pickerCellModel.Icon.Sprite = icon;
            if (iconColor != null) pickerCellModel.Icon.Color = iconColor.Value;
            if (activeValueChanged != null) pickerCellModel.ActiveValueChanged += activeValueChanged;
            if (clicked != null) pickerCellModel.Clicked += clicked;
            if (confirmed != null) pickerCellModel.Confirmed += confirmed;

            return AddEnumPicker(pickerCellModel, priority);
        }

        public int AddEnumPicker(EnumPickerCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.EnumPickerCell, model, priority);
        }

        public int AddMultiPicker(IEnumerable<string> options, IEnumerable<int> activeOptionIndices, string text,
            Color? textColor = null, Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            Action<int, bool> optionStateChanged = null, Action clicked = null, Action confirmed = null,
            int priority = 0)
        {
            var pickerCellModel = new MultiPickerCellModel();
            pickerCellModel.Text = text;
            if (textColor != null) pickerCellModel.TextColor = textColor.Value;
            if (subTextColor != null) pickerCellModel.SubTextColor = subTextColor.Value;
            pickerCellModel.Icon.Sprite = icon;
            if (iconColor != null) pickerCellModel.Icon.Color = iconColor.Value;
            pickerCellModel.SetOptions(options, activeOptionIndices);
            if (optionStateChanged != null) pickerCellModel.OptionStateChanged += optionStateChanged;
            if (clicked != null) pickerCellModel.Clicked += clicked;
            if (confirmed != null) pickerCellModel.Confirmed += confirmed;

            return AddMultiPicker(pickerCellModel, priority);
        }

        public int AddMultiPicker(MultiPickerCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.MultiPickerCell, model, priority);
        }

        public int AddEnumMultiPicker(Enum activeValue, string text, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            Action<Enum> activeValueChanged = null, Action clicked = null, Action confirmed = null,
            int priority = 0)
        {
            var pickerCellModel = new EnumMultiPickerCellModel(activeValue);
            pickerCellModel.Text = text;
            if (textColor != null) pickerCellModel.TextColor = textColor.Value;
            if (subTextColor != null) pickerCellModel.SubTextColor = subTextColor.Value;
            pickerCellModel.Icon.Sprite = icon;
            if (iconColor != null) pickerCellModel.Icon.Color = iconColor.Value;
            if (activeValueChanged != null) pickerCellModel.ActiveValueChanged += activeValueChanged;
            if (clicked != null) pickerCellModel.Clicked += clicked;
            if (confirmed != null) pickerCellModel.Confirmed += confirmed;

            return AddEnumMultiPicker(pickerCellModel, priority);
        }

        public int AddEnumMultiPicker(EnumMultiPickerCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.EnumMultiPickerCell, model, priority);
        }

        public int AddPickerOption(bool isOn, string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, Action<bool> toggled = null,
            int priority = 0)
        {
            var useSubTextOrIcon = !string.IsNullOrEmpty(subText) || icon != null;
            var pickerOptionModel = new PickerOptionCellModel(useSubTextOrIcon);
            pickerOptionModel.IsOn = isOn;
            pickerOptionModel.CellTexts.Text = text;
            pickerOptionModel.CellTexts.SubText = subText;
            if (textColor != null) pickerOptionModel.CellTexts.TextColor = textColor.Value;
            if (subTextColor != null) pickerOptionModel.CellTexts.SubTextColor = subTextColor.Value;
            pickerOptionModel.Icon.Sprite = icon;
            if (iconColor != null) pickerOptionModel.Icon.Color = iconColor.Value;
            if (toggled != null) pickerOptionModel.Toggled += toggled;

            return AddPickerOption(pickerOptionModel, priority);
        }

        public int AddSearchField(string placeholder = null, Action<string> valueChanged = null,
            Action<string> submitted = null, int priority = 0)
        {
            var searchFieldCellModel = new SearchFieldCellModel();
            if (!string.IsNullOrEmpty(placeholder)) searchFieldCellModel.Placeholder = placeholder;
            if (valueChanged != null) searchFieldCellModel.ValueChanged += valueChanged;
            if (submitted != null) searchFieldCellModel.Submitted += submitted;

            return AddSearchField(searchFieldCellModel, priority);
        }

        public int AddSearchField(SearchFieldCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.SearchFieldCell, model, priority);
        }

        public int AddPickerOption(PickerOptionCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.PickerOption, model, priority);
        }

        public int AddPageLinkButton(string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string titleOverride = null,
            Action<DebugPageBase> onLoad = null, int priority = 0)
        {
            return AddPageLinkButton<DebugPage>(text, subText, textColor, subTextColor, icon, iconColor,
                titleOverride, onLoad, priority);
        }

        public int AddPageLinkButton(Type pageType, string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string titleOverride = null,
            Action<DebugPageBase> onLoad = null, int priority = 0)
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(pageType, textModel, iconModel, titleOverride, onLoad, priority);
        }

        public int AddPageLinkButton<TPage>(string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string titleOverride = null,
            Action<TPage> onLoad = null, int priority = 0) where TPage : DebugPageBase
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(textModel, iconModel, titleOverride, onLoad, priority);
        }

        public int AddPageLinkButton(CellTextsModel textModel, CellIconModel iconModel = null,
            string titleOverride = null, Action<DebugPageBase> onLoad = null, int priority = 0)
        {
            return AddPageLinkButton<DebugPage>(textModel, iconModel, titleOverride, onLoad, priority);
        }

        public int AddPageLinkButton(Type pageType, CellTextsModel textModel, CellIconModel iconModel = null,
            string titleOverride = null, Action<DebugPageBase> onLoad = null, int priority = 0)
        {
            return AddPageLinkButton(pageType, null, textModel, iconModel, titleOverride, onLoad, priority);
        }

        public int AddPageLinkButton<TPage>(CellTextsModel textModel, CellIconModel iconModel = null,
            string titleOverride = null, Action<TPage> onLoad = null, int priority = 0) where TPage : DebugPageBase
        {
            return AddPageLinkButton(null, textModel, iconModel, titleOverride, onLoad, priority);
        }

        public int AddPageLinkButton(Type pageType, DebugPageBase prefab, string text, string subText = null,
            Color? textColor = null, Color? subTextColor = null, Sprite icon = null, Color? iconColor = null,
            string titleOverride = null, Action<DebugPageBase> onLoad = null, int priority = 0)
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(pageType, prefab, textModel, iconModel, titleOverride, onLoad, priority);
        }

        public int AddPageLinkButton<TPage>(TPage prefab, string text, string subText = null, Color? textColor = null,
            Color? subTextColor = null, Sprite icon = null, Color? iconColor = null, string titleOverride = null,
            Action<TPage> onLoad = null, int priority = 0) where TPage : DebugPageBase
        {
            var textModel = new CellTextsModel();
            textModel.Text = text;
            textModel.SubText = subText;
            if (textColor != null) textModel.TextColor = textColor.Value;
            if (subTextColor != null) textModel.SubTextColor = subTextColor.Value;
            var iconModel = new CellIconModel();
            iconModel.Sprite = icon;
            if (iconColor != null) iconModel.Color = iconColor.Value;
            return AddPageLinkButton(prefab, textModel, iconModel, titleOverride, onLoad, priority);
        }

        public int AddPageLinkButton(Type pageType, DebugPageBase prefab, CellTextsModel textModel,
            CellIconModel iconModel = null, string titleOverride = null, Action<DebugPageBase> onLoad = null,
            int priority = 0)
        {
            var useSubText = textModel != null && !string.IsNullOrEmpty(textModel.SubText);
            var useIcon = iconModel != null && iconModel.Sprite != null;
            var useSubTextOrIcon = useSubText || useIcon;
            var buttonModel = new PageLinkButtonCellModel(useSubTextOrIcon);
            if (textModel != null)
            {
                buttonModel.CellTexts.Text = textModel.Text;
                buttonModel.CellTexts.TextColor = textModel.TextColor;
                buttonModel.CellTexts.SubText = textModel.SubText;
                buttonModel.CellTexts.SubTextColor = textModel.SubTextColor;
            }
            else
            {
                buttonModel.CellTexts.Text = pageType.Name;
            }

            if (iconModel != null)
            {
                buttonModel.Icon.Sprite = iconModel.Sprite;
                buttonModel.Icon.Color = iconModel.Color;
            }

            buttonModel.PageType = pageType;
            buttonModel.Prefab = prefab;
            buttonModel.PageTitleOverride = titleOverride;
            buttonModel.OnLoad += onLoad;
            buttonModel.ShowArrow = true;
            return AddPageLinkButton(buttonModel, priority);
        }

        public int AddPageLinkButton<TPage>(TPage prefab, CellTextsModel textModel, CellIconModel iconModel = null,
            string titleOverride = null, Action<TPage> onLoad = null, int priority = 0) where TPage : DebugPageBase
        {
            var useSubText = textModel != null && !string.IsNullOrEmpty(textModel.SubText);
            var useIcon = iconModel != null && iconModel.Sprite != null;
            var useSubTextOrIcon = useSubText || useIcon;
            var buttonModel = new PageLinkButtonCellModel(useSubTextOrIcon);
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

            buttonModel.PageType = typeof(TPage);
            buttonModel.Prefab = prefab;
            buttonModel.PageTitleOverride = titleOverride;
            buttonModel.OnLoad += x => onLoad?.Invoke((TPage)x);
            buttonModel.ShowArrow = true;
            return AddPageLinkButton(buttonModel, priority);
        }

        public int AddPageLinkButton(PageLinkButtonCellModel model, int priority = 0)
        {
            return AddItem(AssetKeys.PageLinkButtonCell, model, priority);
        }


        public sealed class ItemInfo
        {
            public readonly CellModel CellModel;
            public readonly int ItemId;
            public readonly string PrefabKey;

            public ItemInfo(int itemId, string prefabKey, CellModel cellModel)
            {
                ItemId = itemId;
                PrefabKey = prefabKey;
                CellModel = cellModel;
            }
        }
    }
}
