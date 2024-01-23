using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityEngine;
using UnityEngine.UI;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;

#else
using System.Collections;
#endif

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells
{
    public sealed class EnumPickerCell : Cell<EnumPickerCellModel>
    {
        [SerializeField] private CanvasGroup _contentsCanvasGroup;

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

            _contentsCanvasGroup.alpha = model.Interactable ? 1.0f : 0.3f;
            
            // Cleanup
            button.onClick.RemoveAllListeners();

            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            // Texts
            cellTexts.Text = model.Text;
            cellTexts.SubText = model.ActiveValue.ToString();
            cellTexts.TextColor = model.TextColor;
            cellTexts.SubTextColor = model.SubTextColor;

            // Button
            button.interactable = model.Interactable;
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
#if UDS_USE_ASYNC_METHODS
                Task OnWillPushEnter()
#else
                IEnumerator OnWillPushEnter()
#endif
                {
                    _pickingPage = page;
#if UDS_USE_ASYNC_METHODS
                    return Task.CompletedTask;
#else
                    yield break;
#endif
                }

#if UDS_USE_ASYNC_METHODS
                Task OnWillPopExit()
#else
                IEnumerator OnWillPopExit()
#endif
                {
                    cellTexts.Text = model.Text;
                    var option = _options[_values.IndexOf(model.ActiveValue)];
                    cellTexts.SubText = option;
                    model.InvokeConfirmed();
#if UDS_USE_ASYNC_METHODS
                    return Task.CompletedTask;
#else
                    yield break;
#endif
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

            DebugSheet.Of(transform).PushPage<PickingPage>(true, model.Text, x => OnLoadPickingPage(x.page));
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

        public bool Interactable { get; set; } = true;

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
