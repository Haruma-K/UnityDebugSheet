using System;
using System.Collections;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class EnumPickerCell : Cell<EnumPickerCellModel>
    {
        public CellIcon icon;
        public CellTexts cellTexts;
        public Button button;

        private readonly List<string> _options = new List<string>();
        private readonly List<Enum> _values = new List<Enum>();

        private bool _isInitialized;
        private PickingPage _pickingPage;

        protected override void SetModel(EnumPickerCellModel model)
        {
            if (!_isInitialized)
            {
                var enumType = model.ActiveValue.GetType();
                foreach (Enum value in Enum.GetValues(enumType))
                {
                    _options.Add(value.ToString());
                    _values.Add(value);
                }

                _isInitialized = true;
            }

            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            //Texts
            cellTexts.Text = model.Text;
            cellTexts.SubText = model.ActiveValue.ToString();
            cellTexts.TextColor = model.TextColor;
            cellTexts.SubTextColor = model.SubTextColor;

            // Click Event
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => OnClicked(model));

            // Refresh the picking page if needed.
            var activeIndex = _values.IndexOf(model.ActiveValue);
            if (_pickingPage != null && _pickingPage.ActiveIndex != activeIndex)
            {
                _pickingPage.SetActiveIndex(activeIndex);
                _pickingPage.RefreshData();
            }
        }

        private void OnClicked(EnumPickerCellModel model)
        {
            void OnLoadPickingPage(PickingPage page)
            {
                IEnumerator OnWillPushEnter()
                {
                    _pickingPage = page;
                    yield break;
                }

                IEnumerator OnWillPopExit()
                {
                    cellTexts.Text = model.Text;
                    var option = _options[_values.IndexOf(model.ActiveValue)];
                    cellTexts.SubText = option;
                    model.InvokeConfirmed();
                    yield break;
                }

                void OnDidPopExit()
                {
                    _pickingPage = null;
                }

                page.AddLifecycleEvent(onWillPushEnter: OnWillPushEnter, onWillPopExit: OnWillPopExit,
                    onDidPopExit: OnDidPopExit);

                page.Setup(_options, _values.IndexOf(model.ActiveValue));
                page.ValueChanged += x =>
                {
                    var activeValue = _values[x];
                    model.ActiveValue = activeValue;
                    model.InvokeActiveValueIndexChanged(activeValue);
                };
            }

            DebugSheet.Of(transform).PushPage<PickingPage>(true, model.Text, OnLoadPickingPage);
            model.InvokeClicked();
        }
    }

    public sealed class EnumPickerCellModel : CellModel
    {
        public EnumPickerCellModel(Enum activeValue)
        {
            ActiveValue = activeValue;
        }

        public string Text { get; set; }

        public Color TextColor { get; set; } = Color.black;

        public Color SubTextColor { get; set; } = Color.gray;

        public CellIconModel Icon { get; } = new CellIconModel();

        public Enum ActiveValue { get; set; }

        /// <summary>
        ///     Event when this cell is clicked.
        /// </summary>
        public event Action Clicked;

        /// <summary>
        ///     Event that is called before the page to select values is closed.
        /// </summary>
        public event Action Confirmed;

        /// <summary>
        ///     Event when active value is changed.
        /// </summary>
        public event Action<Enum> ActiveValueChanged;

        internal void InvokeClicked()
        {
            Clicked?.Invoke();
        }

        internal void InvokeConfirmed()
        {
            Confirmed?.Invoke();
        }

        internal void InvokeActiveValueIndexChanged(Enum value)
        {
            ActiveValueChanged?.Invoke(value);
        }
    }
}
