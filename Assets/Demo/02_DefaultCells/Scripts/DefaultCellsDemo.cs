#if !EXCLUDE_UNITY_DEBUG_SHEET
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#else
using System.Collections;
using Demo._99_Shared.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
#endif

namespace Demo._02_DefaultCells.Scripts
{
    public sealed class DefaultCellsDemo : MonoBehaviour
    {
        [SerializeField] private Button _clearCellsButton;
        [SerializeField] private Button _addDefaultCellsButton;
        [SerializeField] private Button _addLabelCellAtFirstButton;
        [SerializeField] private Button _addLabelCellAtLastButton;
        [SerializeField] private Button _removeFirstCellButton;
        [SerializeField] private Button _removeLastCellButton;

        private DefaultCellsDemoDebugPage _demoDebugPage;
        private PageItemDisposer _itemDisposer;

        private void Start()
        {
            SetAllButtonsInteractable(false);

            var initialPage = DebugSheet.Instance.GetOrCreateInitialPage();
            var linkButtonId = initialPage.AddPageLinkButton<DefaultCellsDemoDebugPage>("Default Cells Demo",
                onLoad: page =>
                {
                    _demoDebugPage = page;
                    page.AddLifecycleEvent(onDidPushEnter: OnDidPushEnter, onWillPopExit: OnWillPopExit);
                }, priority: 0);

            _itemDisposer = new PageItemDisposer(initialPage);
            _itemDisposer.AddItemId(linkButtonId);
        }

        private void OnEnable()
        {
            _clearCellsButton.onClick.AddListener(OnClearCellsButtonClicked);
            _addDefaultCellsButton.onClick.AddListener(OnAddDefaultCellsButtonClicked);
            _addLabelCellAtFirstButton.onClick.AddListener(OnAddLabelCellAtFirstButtonClicked);
            _addLabelCellAtLastButton.onClick.AddListener(OnAddLabelCellAtLastButtonClicked);
            _removeFirstCellButton.onClick.AddListener(OnRemoveFirstCellButtonClicked);
            _removeLastCellButton.onClick.AddListener(OnRemoveLastCellButtonClicked);
        }

        private void OnDisable()
        {
            _clearCellsButton.onClick.RemoveListener(OnClearCellsButtonClicked);
            _addDefaultCellsButton.onClick.RemoveListener(OnAddDefaultCellsButtonClicked);
            _addLabelCellAtFirstButton.onClick.RemoveListener(OnAddLabelCellAtFirstButtonClicked);
            _addLabelCellAtLastButton.onClick.RemoveListener(OnAddLabelCellAtLastButtonClicked);
            _removeFirstCellButton.onClick.RemoveListener(OnRemoveFirstCellButtonClicked);
            _removeLastCellButton.onClick.RemoveListener(OnRemoveLastCellButtonClicked);
        }

        private void OnDestroy()
        {
            _itemDisposer.Dispose();
        }

        private void OnDidPushEnter()
        {
            SetAllButtonsInteractable(true);
        }

#if UDS_USE_ASYNC_METHODS
        private Task OnWillPopExit()
#else
        private IEnumerator OnWillPopExit()
#endif
        {
            SetAllButtonsInteractable(false);

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

        private void SetAllButtonsInteractable(bool interactable)
        {
            _clearCellsButton.interactable = interactable;
            _addDefaultCellsButton.interactable = interactable;
            _addLabelCellAtFirstButton.interactable = interactable;
            _addLabelCellAtLastButton.interactable = interactable;
            _removeFirstCellButton.interactable = interactable;
            _removeLastCellButton.interactable = interactable;
        }

        private void OnClearCellsButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            _demoDebugPage.ClearItems();
        }

        private void OnAddDefaultCellsButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            _demoDebugPage.AddDefaultCells();
        }

        private void OnAddLabelCellAtFirstButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            var model = new LabelCellModel(false);
            model.CellTexts.Text = "Additional Label";
            _demoDebugPage.AddLabel(model, -1);
        }

        private void OnAddLabelCellAtLastButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            var model = new LabelCellModel(false);
            model.CellTexts.Text = "Additional Label";
            _demoDebugPage.AddLabel(model);
        }

        private void OnRemoveFirstCellButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            var firstItemId = _demoDebugPage.ItemInfos[0].ItemId;
            _demoDebugPage.RemoveItem(firstItemId);
        }

        private void OnRemoveLastCellButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            var lastItemId = _demoDebugPage.ItemInfos[_demoDebugPage.ItemInfos.Count - 1].ItemId;
            _demoDebugPage.RemoveItem(lastItemId);
        }
    }
}
#endif
