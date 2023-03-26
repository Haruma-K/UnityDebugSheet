using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEditor;
using UnityEngine;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public class PropertyListDebugPageBase<TTargetClass> : DefaultDebugPageBase
    {
        private readonly Dictionary<string, int> _cellIds = new Dictionary<string, int>();

        private readonly Dictionary<string, ButtonCellModel> _cellModels = new Dictionary<string, ButtonCellModel>();
        private IReadOnlyDictionary<string, PropertyInfo> _propertyInfos;
        private Type TargetClassType => typeof(TTargetClass);

        public virtual List<string> UpdateTargetPropertyNames { get; } = new List<string>();


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
            var propertyInfos = TargetClassType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            return propertyInfos
                .Where(propertyInfo => propertyInfo.GetCustomAttribute<ObsoleteAttribute>() == null)
                .ToDictionary(x => x.Name);
        }

        protected virtual void Update()
        {
            foreach (var targetPropertyName in UpdateTargetPropertyNames)
            {
                if (!_cellModels.TryGetValue(targetPropertyName, out var cellModel)
                    || !_propertyInfos.TryGetValue(targetPropertyName, out var propertyInfo))
                    continue;

                var valueText = propertyInfo.GetValue(null).ToString();
                cellModel.CellTexts.SubText = valueText;
            }

            RefreshData();
        }

        private void ReloadAllItems(string searchString)
        {
            foreach (var cellId in _cellIds.Values)
                RemoveItem(cellId);
            _cellIds.Clear();
            _cellModels.Clear();

            foreach (var propertyInfo in _propertyInfos.Values)
            {
                var propertyName = propertyInfo.Name;
                if (!string.IsNullOrEmpty(searchString) &&
                    propertyName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) == -1)
                    continue;

                var buttonCellModel = new ButtonCellModel(true);
                buttonCellModel.CellTexts.Text = propertyName;
                buttonCellModel.CellTexts.SubText = propertyInfo.GetValue(null).ToString();
                var itemId = AddButton(buttonCellModel);
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

                _cellIds.Add(propertyName, itemId);
                _cellModels.Add(propertyName, buttonCellModel);
            }

            Reload();
        }

        protected override string Title => ObjectNames.NicifyVariableName(TargetClassType.Name);
    }
}
