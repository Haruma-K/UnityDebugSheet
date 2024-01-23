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
    public sealed class PickerCell : Cell<PickerCellModel>
    {
        [SerializeField] private CanvasGroup _contentsCanvasGroup;

        public CellIcon icon;
        public CellTexts cellTexts;
        public Button button;

        private PickingPage _pickingPage;

        protected override void SetModel(PickerCellModel model)
        {
            _contentsCanvasGroup.alpha = model.Interactable ? 1.0f : 0.3f;
            
            // Cleanup
            button.onClick.RemoveAllListeners();

            // Icon
            icon.Setup(model.Icon);
            icon.gameObject.SetActive(model.Icon.Sprite != null);

            // Texts
            cellTexts.Text = model.Text;
            var option = model.Options[model.ActiveOptionIndex];
            cellTexts.SubText = option;
            cellTexts.TextColor = model.TextColor;
            cellTexts.SubTextColor = model.SubTextColor;

            // Button
            button.interactable = model.Interactable;
            button.onClick.AddListener(() => OnClicked(model));

            // Refresh the picking page if needed.
            if (_pickingPage != null && _pickingPage.ActiveIndex != model.ActiveOptionIndex)
            {
                _pickingPage.SetActiveIndex(model.ActiveOptionIndex);
                _pickingPage.RefreshData();
            }
        }

        private void OnClicked(PickerCellModel model)
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
                    var option = model.Options[model.ActiveOptionIndex];
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

                page.Setup(model.Options, model.ActiveOptionIndex);
                page.ValueChanged += x =>
                {
                    model.ActiveOptionIndex = x;
                    model.InvokeSelectedOptionIndexChanged(x);
                };
            }

            DebugSheet.Of(transform).PushPage<PickingPage>(true, model.Text, x => OnLoadPickingPage(x.page));
            model.InvokeClicked();
        }
    }

    public sealed class PickerCellModel : CellModel
    {
        private List<string> _options = new List<string>();

        public string Text { get; set; }

        public Color TextColor { get; set; } = Color.black;

        public Color SubTextColor { get; set; } = Color.gray;

        public CellIconModel Icon { get; } = new CellIconModel();

        public int ActiveOptionIndex { get; set; }

        public bool Interactable { get; set; } = true;

        public IReadOnlyList<string> Options => _options;

        /// <summary>
        ///     Event when this cell is clicked.
        /// </summary>
        public event Action Clicked;

        /// <summary>
        ///     Event that is called before the page to select options is closed.
        /// </summary>
        public event Action Confirmed;

        /// <summary>
        ///     Event when active option is changed.
        /// </summary>
        public event Action<int> ActiveOptionChanged;

        public void SetOptions(IEnumerable<string> options, int activeOptionIndex)
        {
            _options = new List<string>(options);
            ActiveOptionIndex = activeOptionIndex;
        }

        internal void InvokeClicked()
        {
            Clicked?.Invoke();
        }

        internal void InvokeConfirmed()
        {
            Confirmed?.Invoke();
        }

        internal void InvokeSelectedOptionIndexChanged(int index)
        {
            ActiveOptionChanged?.Invoke(index);
        }
    }
}
