#if !EXCLUDE_UNITY_DEBUG_SHEET
using Demo._01_CharacterViewer.Scripts.Viewer;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using UnityEngine.Assertions;

namespace Demo._01_CharacterViewer.Scripts
{
    public sealed class CharacterViewerDemo : MonoBehaviour
    {
        [SerializeField] private DebugSheet _sheet;
        [SerializeField] private CharacterSpawner _spawner;
        [SerializeField] private StandController _standController;

        private void Start()
        {
            Assert.IsNotNull(_sheet);
            Assert.IsNotNull(_spawner);
            Assert.IsNotNull(_standController);
            
            _spawner.Initialize();

            _sheet.Initialize<CharacterViewerDemoDebugPage>(page =>
            {
                page.Setup(_spawner, _standController, GraphyManager.Instance, DebugLogManager.Instance);
            });
        }
    }
}
#endif
