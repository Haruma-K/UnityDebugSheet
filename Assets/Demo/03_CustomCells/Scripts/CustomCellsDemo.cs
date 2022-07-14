#if !EXCLUDE_UNITY_DEBUG_SHEET
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Demo._03_CustomCells.Scripts
{
    public sealed class CustomCellsDemo : MonoBehaviour
    {
        [SerializeField] private DebugSheet _sheet;
        [SerializeField] private Button _refreshButton;

        private CustomCellsDemoDebugPage _demoDebugPage;

        private void Start()
        {
            var demoPage = _sheet.Initialize<CustomCellsDemoDebugPage>();
            demoPage.Setup(30);
            _demoDebugPage = demoPage;
        }

        private void OnEnable()
        {
            _refreshButton.onClick.AddListener(OnRefreshButtonClicked);
        }

        private void OnDisable()
        {
            _refreshButton.onClick.RemoveListener(OnRefreshButtonClicked);
        }

        private void OnRefreshButtonClicked()
        {
            if (_demoDebugPage == null)
                return;

            _demoDebugPage.ChangeData();
        }
    }
}
#endif
