using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public sealed class EnumMultiPickerCell : Cell<EnumMultiPickerCellModel>
    {
        [SerializeField] private CanvasGroup _contentsCanvasGroup;

        public CellIcon icon;
        public CellTexts cellTexts;
        public Button button;

        private readonly List<Option> _options = new List<Option>();

        private bool _isInitialized;
        private MultiPickingPage _pickingPage;

        protected override void SetModel(EnumMultiPickerCellModel model)
        {
            if (!_isInitialized)
            {
                var enumType = model.ActiveValue.GetType();
                var activeIntValue = Convert.ToInt32(model.ActiveValue);
                foreach (Enum value in Enum.GetValues(enumType))
                {
                    var intValue = Convert.ToInt32(value);
                    _options.Add(new Option
                    {
                        Name = value.ToString(),
                        Value = intValue,
                        IsActive = (activeIntValue & intValue) == intValue
                    });
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

            // Reload the picking page if it is already created.
            if (_pickingPage != null)
            {
                var optionStrings = new List<string>();
                var activeOptionIndices = new List<int>();
                for (var i = 0; i < _options.Count; i++)
                {
                    var option = _options[i];
                    optionStrings.Add(option.Name);
                    if (option.IsActive)
                        activeOptionIndices.Add(i);
                }

                _pickingPage.Setup(optionStrings, activeOptionIndices);
            }
        }

        private void OnClicked(EnumMultiPickerCellModel model)
        {
            void OnLoadPickingPage(MultiPickingPage page)
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
                    cellTexts.SubText = GetSubText(model);
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

                var optionStrings = new List<string>();
                var activeOptionIndices = new List<int>();
                for (var i = 0; i < _options.Count; i++)
                {
                    var option = _options[i];
                    optionStrings.Add(option.Name);
                    if (option.IsActive)
                        activeOptionIndices.Add(i);
                }

                page.Setup(optionStrings, activeOptionIndices);
                page.OptionActiveStateChanged += x =>
                {
                    _options[x.index].IsActive = x.isActive;
                    var intValue = 0;
                    foreach (var option in _options)
                        if (option.IsActive)
                            intValue |= option.Value;
                    var newValue = Enum.Parse(model.ActiveValue.GetType(), intValue.ToString()) as Enum;
                    model.ActiveValue = newValue;
                    model.InvokeActiveValueIndexChanged(newValue);
                };
            }

            DebugSheet.Of(transform)
                .PushPage<MultiPickingPage>(true, model.Text, x => OnLoadPickingPage(x.page));
            model.InvokeClicked();
        }

        private string GetSubText(EnumMultiPickerCellModel model)
        {
            var optionsText = new StringBuilder();
            foreach (var option in _options.Where(x => x.IsActive))
            {
                if (optionsText.Length >= 1)
                    optionsText.Append(", ");

                optionsText.Append(option.Name);
            }

            return optionsText.ToString();
        }

        private class Option
        {
            public string Name { get; set; }
            public int Value { get; set; }
            public bool IsActive { get; set; }
        }
    }

    public sealed class EnumMultiPickerCellModel : CellModel
    {
        public EnumMultiPickerCellModel(Enum activeValue)
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
