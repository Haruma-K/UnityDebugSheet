#if !EXCLUDE_UNITY_DEBUG_SHEET
using Demo._01_CharacterViewer.Scripts.Viewer;
using Demo._99_Shared.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

namespace Demo._01_CharacterViewer.Scripts
{
    public sealed class CharacterViewerDemo : MonoBehaviour
    {
        [SerializeField] private CharacterSpawner _spawner;
        [SerializeField] private StandController _standController;
        private DebugPageBase _initialPage;

        private int _linkButtonId = -1;

        private void Start()
        {
            Assert.IsNotNull(_spawner);
            Assert.IsNotNull(_standController);

            _spawner.Initialize();

            _initialPage = DebugSheet.Instance.GetOrCreateInitialPage();
            _linkButtonId = _initialPage.AddPageLinkButton<CharacterViewerPage>("Character Viewer",
                icon: DemoSprites.Icon.CharacterViewer,
                onLoad: page => page.Setup(_spawner, _standController),
                priority: 0);
            _initialPage.Reload();
        }

        private void OnDestroy()
        {
            if (_linkButtonId != -1 && _initialPage != null)
            {
                _initialPage.RemoveItem(_linkButtonId);
                _initialPage.Reload();
            }
        }
    }
}
#endif
