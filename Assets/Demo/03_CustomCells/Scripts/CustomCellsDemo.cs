#if !EXCLUDE_UNITY_DEBUG_SHEET
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityEngine;
using UnityEngine.UI;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#else
using System.Collections;
#endif

namespace Demo._03_CustomCells.Scripts
{
    public sealed class CustomCellsDemo : MonoBehaviour
    {
        [SerializeField] private Button _refreshButton;

        private CustomCellsDemoDebugPage _demoDebugPage;
        private DebugPageBase _initialPage;
        private int _pageLinkButtonId;

        private void Start()
        {
            _initialPage = DebugSheet.Instance.GetOrCreateInitialPage();
            _pageLinkButtonId = _initialPage.AddPageLinkButton<CustomCellsDemoDebugPage>("Custom Cells Demo",
                onLoad: page =>
                {
                    page.Setup(30);
                    _demoDebugPage = page;
                    page.AddLifecycleEvent(onDidPushEnter: OnDidPushEnter, onWillPopExit: OnWillPopExit);
                }, priority: 0);
            _refreshButton.interactable = false;
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
            if (_initialPage != null)
                _initialPage.RemoveItem(_pageLinkButtonId);
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
