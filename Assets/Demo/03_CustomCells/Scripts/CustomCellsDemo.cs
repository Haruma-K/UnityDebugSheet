#if !EXCLUDE_UNITY_DEBUG_SHEET
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif
using System.Collections;
using Demo._99_Shared.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityEngine;
using UnityEngine.UI;

namespace Demo._03_CustomCells.Scripts
{
    public sealed class CustomCellsDemo : MonoBehaviour
    {
        [SerializeField] private Button _refreshButton;

        private CustomCellsDemoDebugPage _demoDebugPage;
        private PageItemDisposer _itemDisposer;

        private void Start()
        {
            var initialPage = DebugSheet.Instance.GetOrCreateInitialPage();
            var pageLinkButtonId = initialPage.AddPageLinkButton<CustomCellsDemoDebugPage>("Custom Cells Demo",
                onLoad: x =>
                {
                    var page = x.page;
                    page.Setup(30);
                    _demoDebugPage = page;
                    page.AddLifecycleEvent(onDidPushEnter: OnDidPushEnter, onWillPopExit: OnWillPopExit);
                }, priority: 0);
            _refreshButton.interactable = false;

            _itemDisposer = new PageItemDisposer(initialPage);
            _itemDisposer.AddItemId(pageLinkButtonId);
        }

        private void OnEnable()
        {
            _refreshButton.onClick.AddListener(OnRefreshButtonClicked);
        }

        private void OnDisable()
        {
            _refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
        }

        private void OnDestroy()
        {
            _itemDisposer?.Dispose();
        }

        private void OnRefreshButtonClicked()
        {
            if (_demoDebugPage == null)
                return;

            _demoDebugPage.ChangeData();
        }

        private void OnDidPushEnter()
        {
            _refreshButton.interactable = true;
        }

#if UDS_USE_ASYNC_METHODS
        private Task OnWillPopExit()
#else
        private IEnumerator OnWillPopExit()
#endif
        {
            _refreshButton.interactable = false;

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }
    }
}
#endif
