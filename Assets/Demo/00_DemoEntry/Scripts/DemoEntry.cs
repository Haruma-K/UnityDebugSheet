#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using System.Collections;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Demo._00_DemoEntry.Scripts
{
    public sealed class DemoEntry : MonoBehaviour
    {
        private const string CharacterViewerScenePath =
            "Assets/Demo/01_CharacterViewer/Scenes/CharacterViewerDemo.unity";

        private const string DefaultCellsScenePath = "Assets/Demo/02_DefaultCells/Scenes/DefaultCellsDemo.unity";
        private const string CustomCellsScenePath = "Assets/Demo/03_CustomCells/Scenes/CustomCellsDemo.unity";

        [SerializeField] private Button _characterViewerButton;
        [SerializeField] private Button _defaultCellsButton;
        [SerializeField] private Button _customCellsButton;

        private Scene? _loadedScene;

        private void Awake()
        {
            _characterViewerButton.onClick.AddListener(OnCharacterViewerButtonClicked);
            _defaultCellsButton.onClick.AddListener(OnDefaultCellsButtonClicked);
            _customCellsButton.onClick.AddListener(OnCustomCellsButtonClicked);
        }

        private void OnDestroy()
        {
            _characterViewerButton.onClick.RemoveListener(OnCharacterViewerButtonClicked);
            _defaultCellsButton.onClick.RemoveListener(OnDefaultCellsButtonClicked);
            _customCellsButton.onClick.RemoveListener(OnCustomCellsButtonClicked);
        }

        private void OnCharacterViewerButtonClicked()
        {
            SetInteractable(false);
            StartCoroutine(LoadSceneRoutine(CharacterViewerScenePath, () => SetInteractable(true)));
        }

        private void OnDefaultCellsButtonClicked()
        {
            SetInteractable(false);
            StartCoroutine(LoadSceneRoutine(DefaultCellsScenePath, () => SetInteractable(true)));
        }

        private void OnCustomCellsButtonClicked()
        {
            SetInteractable(false);
            StartCoroutine(LoadSceneRoutine(CustomCellsScenePath, () => SetInteractable(true)));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, Action onComplete = null)
        {
            // Pop to initial page.
            var debugSheet = DebugSheet.Instance;
            if (debugSheet != null)
            {
                while (debugSheet.CurrentDebugPage != debugSheet.InitialDebugPage)
                    yield return debugSheet.PopPage(false);
            }
            
            // Unload previous scene if exists.
            if (_loadedScene.HasValue)
                yield return SceneManager.UnloadSceneAsync(_loadedScene.Value);
            
            // Load and activate new scene.
            var scene = SceneManager.LoadScene(sceneName, new LoadSceneParameters(LoadSceneMode.Additive));
            _loadedScene = scene;
            yield return new WaitUntil(() => scene.isLoaded);
            SceneManager.SetActiveScene(scene);

            onComplete?.Invoke();
        }

        private void SetInteractable(bool interactable)
        {
            _characterViewerButton.interactable = interactable;
            _defaultCellsButton.interactable = interactable;
            _customCellsButton.interactable = interactable;
        }
    }
}
#endif
