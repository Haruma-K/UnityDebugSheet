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
        private PageItemDisposer _itemDisposer;

        private void Start()
        {
            Assert.IsNotNull(_spawner);
            Assert.IsNotNull(_standController);

            _spawner.Initialize();

            var initialPage = DebugSheet.Instance.GetOrCreateInitialPage();
            var linkButtonId = initialPage.AddPageLinkButton<CharacterViewerPage>("Character Viewer",
                icon: DemoSprites.Icon.CharacterViewer,
                onLoad: x => { x.page.Setup(_spawner, _standController); },
                priority: 0);

            _itemDisposer = new PageItemDisposer(initialPage);
            _itemDisposer.AddItemId(linkButtonId);
        }

        private void OnDestroy()
        {
            _itemDisposer?.Dispose();
        }
    }
}
#endif
