#if !EXCLUDE_UNITY_DEBUG_SHEET
using System.Collections;
using Demo._00_Shared.Scripts;
using Demo._01_CharacterViewer.Scripts.DebugTools;
using Demo._01_CharacterViewer.Scripts.Viewer;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;

namespace Demo._01_CharacterViewer.Scripts
{
    public sealed class CharacterViewerDemoDebugPage : DefaultDebugPageBase
    {
        private CharacterSpawner _characterSpawner;
        private ButtonCellModel _characterViewerButtonModel;
        private DebugLogManager _debugLogManager;
        private ButtonCellModel _debugToolsButtonModel;
        private GraphyManager _graphyManager;
        private StandController _standController;

        protected override string Title => "Main Demo";

        public void Setup(CharacterSpawner characterSpawner, StandController standController,
            GraphyManager graphyManager, DebugLogManager debugLogManager)
        {
            _characterSpawner = characterSpawner;
            _standController = standController;
            _graphyManager = graphyManager;
            _debugLogManager = debugLogManager;
        }

        public override IEnumerator Initialize()
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

            yield break;
        }

        public override IEnumerator Cleanup()
        {
            _characterViewerButtonModel.Clicked -= OnCharacterViewerButtonClicked;
            _debugToolsButtonModel.Clicked -= OnDebugToolsButtonClicked;

            yield break;
        }

        private void OnDebugToolsButtonClicked()
        {
            DebugSheet.Of(transform)
                .PushPage<DebugToolsPage>(true, onLoad: x => x.Setup(_graphyManager, _debugLogManager));
        }

        private void OnCharacterViewerButtonClicked()
        {
            DebugSheet.Of(transform)
                .PushPage<CharacterViewerPage>(true, onLoad: x => x.Setup(_characterSpawner, _standController));
        }
    }
}
#endif
