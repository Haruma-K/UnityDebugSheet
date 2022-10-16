using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityEngine;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public sealed class SystemInfoDebugPage : DebugPageBase
    {
        private readonly List<int> _propertyCellIds = new List<int>();
        private readonly Dictionary<string, string> _propertyValues = new Dictionary<string, string>();
        protected override string Title => "System Info";

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            AddSearchField(submitted: ReloadAllItems);

            var props = typeof(SystemInfo).GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var prop in props)
            {
                if (prop.GetCustomAttribute<ObsoleteAttribute>() != null)
                    continue;

                var value = prop.GetValue(null);
                if (value == null)
                    continue;

                _propertyValues.Add(prop.Name, value.ToString());
            }

            ReloadAllItems(null);

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

        private void ReloadAllItems(string searchString)
        {
            foreach (var cellId in _propertyCellIds)
                RemoveItem(cellId);
            _propertyCellIds.Clear();

            foreach (var prop in _propertyValues)
            {
                if (!string.IsNullOrEmpty(searchString) &&
                    prop.Key.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) == -1)
                    continue;

                var buttonCellModel = new ButtonCellModel(true);
                buttonCellModel.CellTexts.Text = prop.Key;
                buttonCellModel.CellTexts.SubText = prop.Value;
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

                _propertyCellIds.Add(itemId);
            }

            Reload();
        }
    }
}
