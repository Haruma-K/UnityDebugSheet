#if !EXCLUDE_UNITY_DEBUG_SHEET
using Demo._01_CharacterViewer.Scripts.Viewer;
using Demo._99_Shared.Scripts;
using Demo._99_Shared.Scripts.DebugTools;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#else
using System.Collections;
#endif

namespace Demo._01_CharacterViewer.Scripts
{
    public sealed class CharacterViewerDemoDebugPage : DebugPageBase
    {
        private CharacterSpawner _characterSpawner;
        private ButtonCellModel _characterViewerButtonModel;
        private DebugLogManager _debugLogManager;
        private ButtonCellModel _debugToolsButtonModel;
        private GraphyManager _graphyManager;
        private StandController _standController;

        protected override string Title => "Character Viewer Demo";

        public void Setup(CharacterSpawner characterSpawner, StandController standController,
            GraphyManager graphyManager, DebugLogManager debugLogManager)
        {
            _characterSpawner = characterSpawner;
            _standController = standController;
            _graphyManager = graphyManager;
            _debugLogManager = debugLogManager;
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            // Character Viewer
            var characterViewerButtonModel = new ButtonCellModel(false);
            _characterViewerButtonModel = characterViewerButtonModel;
            characterViewerButtonModel.CellTexts.Text = "Character Viewer";
            characterViewerButtonModel.ShowArrow = true;
            characterViewerButtonModel.Icon.Sprite = DemoSprites.Icon.CharacterViewer;
            characterViewerButtonModel.Clicked += OnCharacterViewerButtonClicked;
            AddButton(characterViewerButtonModel);

            // Debug Tools
            var debugToolsButtonModel = new ButtonCellModel(false);
            _debugToolsButtonModel = debugToolsButtonModel;
            debugToolsButtonModel.CellTexts.Text = "Debug Tools";
            debugToolsButtonModel.ShowArrow = true;
            debugToolsButtonModel.Icon.Sprite = DemoSprites.Icon.Tools;
            debugToolsButtonModel.Clicked += OnDebugToolsButtonClicked;
            AddButton(debugToolsButtonModel);

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

#if UDS_USE_ASYNC_METHODS
        public override Task Cleanup()
#else
        public override IEnumerator Cleanup()
#endif
        {
            _characterViewerButtonModel.Clicked -= OnCharacterViewerButtonClicked;
            _debugToolsButtonModel.Clicked -= OnDebugToolsButtonClicked;

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }

        private void OnDebugToolsButtonClicked()
        {
            DebugSheet.Of(transform)
                .PushPage<DebugToolsPage>(true);
        }

        private void OnCharacterViewerButtonClicked()
        {
            DebugSheet.Of(transform)
                .PushPage<CharacterViewerPage>(true, onLoad: x => x.Setup(_characterSpawner, _standController));
        }
    }
}
#endif
