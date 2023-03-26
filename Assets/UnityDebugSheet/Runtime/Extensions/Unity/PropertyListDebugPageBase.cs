using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public class PropertyListDebugPageBase<TTargetClass> : DefaultDebugPageBase
    {
        private readonly Dictionary<string, int> _cellIds = new Dictionary<string, int>();

        private readonly Dictionary<string, ButtonCellModel> _buttonCellModels =
            new Dictionary<string, ButtonCellModel>();

        private IReadOnlyDictionary<string, PropertyInfo> _propertyInfos;
        private Type TargetClassType => typeof(TTargetClass);

        public virtual List<string> IgnoreTargetPropertyNames { get; } = new List<string>();
        public virtual List<string> UpdateTargetPropertyNames { get; } = new List<string>();

        public virtual Dictionary<string, SubPageInfo> SubPageTargetPropertyNameToInfoMap { get; } =
            new Dictionary<string, SubPageInfo>();

        public virtual BindingFlags BindingFlags { get; } = BindingFlags.Public | BindingFlags.Static;
        public virtual object TargetObject { get; } = null;


#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            AddSearchField(submitted: ReloadAllItems);

            _propertyInfos = GetPropertyInfos();

            ReloadAllItems(null);

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

        private IReadOnlyDictionary<string, PropertyInfo> GetPropertyInfos()
        {
            var propertyInfos = TargetClassType.GetProperties(BindingFlags);
            return propertyInfos
                .Where(propertyInfo => propertyInfo.GetCustomAttribute<ObsoleteAttribute>() == null)
                .ToDictionary(x => x.Name);
        }

        protected virtual void Update()
        {
            foreach (var targetPropertyName in UpdateTargetPropertyNames)
            {
                if (!_buttonCellModels.TryGetValue(targetPropertyName, out var cellModel)
                    || !_propertyInfos.TryGetValue(targetPropertyName, out var propertyInfo))
                    continue;

                var valueText = CreatePropertyDescription(propertyInfo);
                cellModel.CellTexts.SubText = valueText;
            }

            RefreshData();
        }

        private void ReloadAllItems(string searchString)
        {
            foreach (var cellId in _cellIds.Values)
                RemoveItem(cellId);
            _cellIds.Clear();
            _buttonCellModels.Clear();

            foreach (var propertyInfo in _propertyInfos.Values)
            {
                var propertyName = propertyInfo.Name;
                if (IgnoreTargetPropertyNames.Contains(propertyName))
                    continue;

                if (!string.IsNullOrEmpty(searchString) &&
                    propertyName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) == -1)
                    continue;

                int itemId;

                if (SubPageTargetPropertyNameToInfoMap.TryGetValue(propertyName, out var subPageInfo))
                {
                    var pageLinkButtonCellModel = new PageLinkButtonCellModel(false);
                    pageLinkButtonCellModel.CellTexts.Text = propertyName;
                    pageLinkButtonCellModel.ShowArrow = true;
                    pageLinkButtonCellModel.PageType = subPageInfo.Type;
                    pageLinkButtonCellModel.OnLoad += x => subPageInfo.OnLoad?.Invoke(x);
                    itemId = AddPageLinkButton(pageLinkButtonCellModel);
                }
                else
                {
                    var buttonCellModel = new ButtonCellModel(true);
                    buttonCellModel.CellTexts.Text = propertyName;
                    buttonCellModel.CellTexts.SubText = CreatePropertyDescription(propertyInfo);
                    itemId = AddButton(buttonCellModel);
                    buttonCellModel.Clicked += () =>
                    {
                        var cell = GetCellIfExists(itemId);
                        var cellRectTrans = (RectTransform)cell.transform;
                        DebugSheet.Instance.BalloonButton.Show(cellRectTrans, "Copy",
                            () =>
                            {
                                GUIUtility.systemCopyBuffer =
                                    $"{buttonCellModel.CellTexts.Text}: {buttonCellModel.CellTexts.SubText}";
                            });
                    };
                    _buttonCellModels.Add(propertyName, buttonCellModel);
                }

                _cellIds.Add(propertyName, itemId);
            }

            Reload();
        }

        private string CreatePropertyDescription(PropertyInfo propertyInfo)
        {
            var propertyName = propertyInfo.Name;

            var value = propertyInfo.GetValue(TargetObject);
            if (TryGetOverridePropertyDescription(propertyName, value, out var description))
                return description;

            if (!propertyInfo.PropertyType.IsArray)
                return value?.ToString();

            var array = (Array)propertyInfo.GetValue(null);
            var valueDescriptions = array.Cast<object>().Select(x => x?.ToString());
            description = string.Join(", ", valueDescriptions);
            return $"[{description}]";
        }

        protected virtual bool TryGetOverridePropertyDescription(string propertyName, object value,
            out string description)
        {
            description = null;
            return false;
        }

        protected override string Title => TargetClassType.Name;

        public sealed class SubPageInfo
        {
            public SubPageInfo(Type type, Action<(string pageId, DebugPageBase page)> onLoad)
            {
                Type = type;
                OnLoad = onLoad;
            }

            public Type Type { get; }
            public Action<(string pageId, DebugPageBase page)> OnLoad { get; }
        }
    }
}
