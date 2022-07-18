#if !EXCLUDE_UNITY_DEBUG_SHEET
using System.Collections;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

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
        private DebugPageBase _initialPage;
        private int _linkButtonId = -1;

        private void Start()
        {
            SetAllButtonsInteractable(false);

            _initialPage = DebugSheet.Instance.GetOrCreateInitialPage();
            _linkButtonId = _initialPage.AddPageLinkButton<DefaultCellsDemoDebugPage>("Default Cells Demo",
                onLoad: page =>
                {
                    page.AddDefaultCells();
                    _demoDebugPage = page;
                    page.AddLifecycleEvent(onDidPushEnter: OnDidPushEnter, onWillPopExit: OnWillPopExit);
                });
            _initialPage.Reload();
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
            if (_linkButtonId != -1 && _initialPage != null)
            {
                _initialPage.RemoveItem(_linkButtonId);
                _initialPage.Reload();
            }
        }

        private void OnDidPushEnter()
        {
            SetAllButtonsInteractable(true);
        }

        private IEnumerator OnWillPopExit()
        {
            SetAllButtonsInteractable(false);
            yield break;
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
            _demoDebugPage.Reload();
        }

        private void OnAddDefaultCellsButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            _demoDebugPage.AddDefaultCells();
            _demoDebugPage.Reload();
        }

        private void OnAddLabelCellAtFirstButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            var model = new LabelCellModel(false);
            model.CellTexts.Text = "Additional Label";
            _demoDebugPage.AddLabel(model, 0);
            _demoDebugPage.Reload();
        }

        private void OnAddLabelCellAtLastButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            var model = new LabelCellModel(false);
            model.CellTexts.Text = "Additional Label";
            _demoDebugPage.AddLabel(model);
            _demoDebugPage.Reload();
        }

        private void OnRemoveFirstCellButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            var firstDataId = _demoDebugPage.ItemIds[0];
            _demoDebugPage.RemoveItem(firstDataId);
            _demoDebugPage.Reload();
        }

        private void OnRemoveLastCellButtonClicked()
        {
            Assert.IsNotNull(_demoDebugPage);

            var lastDataId = _demoDebugPage.ItemIds[_demoDebugPage.ItemIds.Count - 1];
            _demoDebugPage.RemoveItem(lastDataId);
            _demoDebugPage.Reload();
        }
    }
}
#endif
