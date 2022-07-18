using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Foundation.Drawer;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules;
using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules.AssetLoader;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public sealed class DebugSheet : MonoBehaviour, IPageContainerCallbackReceiver
    {
        private static readonly Dictionary<int, DebugSheet> InstanceCacheByTransform =
            new Dictionary<int, DebugSheet>();

        [SerializeField] private bool _singleton = true;
        [SerializeField] private StatefulDrawerController _drawerController;
        [SerializeField] private Button _backButton;
        [SerializeField] private Text _exitTitleText;
        [SerializeField] private Text _enterTitleText;
        [SerializeField] private Button _closeButton;
        [SerializeField] private PageContainer _pageContainer;
        [SerializeField] [HideInInspector] private GameObject _pagePrefab;
        [SerializeField] private List<GameObject> _cellPrefabs = new List<GameObject>();
        private bool _isInitialized;

        public static DebugSheet Instance { get; private set; }

        public DebugPageBase InitialDebugPage { get; private set; }
        public DebugPageBase CurrentDebugPage { get; private set; }
        public DebugPageBase EnteringDebugPage { get; private set; }
        public DebugPageBase ExitingDebugPage { get; private set; }
        public IReadOnlyList<Page> Pages => _pageContainer.Pages;
        public IList<GameObject> CellPrefabs => _cellPrefabs;

        private void Awake()
        {
            if (_singleton)
            {
                if (Instance == null)
                {
                    Instance = this;
                    DontDestroyOnLoad(gameObject);
                    return;
                }

                foreach (var cellPrefab in _cellPrefabs)
                    if (!Instance.CellPrefabs.Contains(cellPrefab))
                        Instance.CellPrefabs.Add(cellPrefab);
                
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);

            if (Instance == this)
                Instance = null;
        }

        void IPageContainerCallbackReceiver.BeforePush(Page enterPage, Page exitPage)
        {
            EnteringDebugPage = enterPage.GetComponent<DebugPageBase>();
            ExitingDebugPage = exitPage == null ? null : exitPage.GetComponent<DebugPageBase>();
            _enterTitleText.text = EnteringDebugPage.GetTitle();
            _exitTitleText.text = ExitingDebugPage == null ? "" : ExitingDebugPage.GetTitle();
            CurrentDebugPage = EnteringDebugPage;
            enterPage.TransitionAnimationProgressChanged += OnTransitionProgressChanged;
        }

        void IPageContainerCallbackReceiver.AfterPush(Page enterPage, Page exitPage)
        {
            _backButton.interactable = _pageContainer.Pages.Count >= 2;
            enterPage.TransitionAnimationProgressChanged -= OnTransitionProgressChanged;
            EnteringDebugPage = null;
            ExitingDebugPage = null;
        }

        void IPageContainerCallbackReceiver.BeforePop(Page enterPage, Page exitPage)
        {
            EnteringDebugPage = enterPage.GetComponent<DebugPageBase>();
            ExitingDebugPage = exitPage.GetComponent<DebugPageBase>();
            _enterTitleText.text = EnteringDebugPage.GetTitle();
            _exitTitleText.text = ExitingDebugPage.GetTitle();
            CurrentDebugPage = EnteringDebugPage;
            enterPage.TransitionAnimationProgressChanged += OnTransitionProgressChanged;
        }

        void IPageContainerCallbackReceiver.AfterPop(Page enterPage, Page exitPage)
        {
            _backButton.interactable = _pageContainer.Pages.Count >= 2;
            enterPage.TransitionAnimationProgressChanged -= OnTransitionProgressChanged;
            EnteringDebugPage = null;
            ExitingDebugPage = null;
        }

        /// <summary>
        ///     Get the <see cref="PageContainer" /> that manages the debug page to which <see cref="transform" /> belongs.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="transform" />.</param>
        /// <returns></returns>
        public static DebugSheet Of(Transform transform, bool useCache = true)
        {
            return Of((RectTransform)transform, useCache);
        }

        /// <summary>
        ///     Get the <see cref="DebugSheet" /> that manages the debug page to which <see cref="rectTransform" /> belongs.
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="useCache">Use the previous result for the <see cref="rectTransform" />.</param>
        /// <returns></returns>
        public static DebugSheet Of(RectTransform rectTransform, bool useCache = true)
        {
            var id = rectTransform.GetInstanceID();

            if (useCache && InstanceCacheByTransform.TryGetValue(id, out var container)) return container;

            container = rectTransform.GetComponentInParent<DebugSheet>();
            if (container != null)
            {
                InstanceCacheByTransform.Add(id, container);
                return container;
            }

            return null;
        }

        public TInitialPage Initialize<TInitialPage>(string titleOverride = null, Action<TInitialPage> onLoad = null)
            where TInitialPage : DebugPageBase
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(DebugSheet)} is already initialized.");

            _backButton.onClick.AddListener(OnBackButtonClicked);
            _backButton.interactable = false;
            SetBackButtonVisibility(0.0f);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
            _pageContainer.AddCallbackReceiver(this);
            var preloadedAssetLoader = new PreloadedAssetLoader();
            preloadedAssetLoader.AddObject(_pagePrefab.gameObject);
            _pageContainer.AssetLoader = preloadedAssetLoader;

            PushPage<TInitialPage>(false, titleOverride, x =>
            {
                InitialDebugPage = x;
                onLoad?.Invoke(x);
            });
            _isInitialized = true;
            return (TInitialPage)InitialDebugPage;
        }

        /// <summary>
        /// </summary>
        /// <param name="titleOverride"></param>
        /// <param name="onLoad"></param>
        /// <typeparam name="TInitialPage"></typeparam>
        /// <returns>If initial page type is not <see cref="TInitialPage" />, return null.</returns>
        public TInitialPage GetOrCreateInitialPage<TInitialPage>(string titleOverride = null,
            Action<TInitialPage> onLoad = null)
            where TInitialPage : DebugPageBase
        {
            if (_isInitialized)
                return InitialDebugPage as TInitialPage;

            return Initialize(titleOverride, onLoad);
        }

        /// <summary>
        /// </summary>
        /// <param name="titleOverride"></param>
        /// <param name="onLoad"></param>
        /// <returns></returns>
        public DebugPage GetOrCreateInitialPage(string titleOverride = null, Action<DebugPage> onLoad = null)
        {
            return GetOrCreateInitialPage<DebugPage>();
        }

        public AsyncProcessHandle PushPage<TPage>(bool playAnimation, string titleOverride = null,
            Action<TPage> onLoad = null)
            where TPage : DebugPageBase
        {
            return _pageContainer.Push<TPage>(_pagePrefab.gameObject.name, playAnimation, onLoad: x =>
            {
                if (titleOverride != null)
                    x.SetTitle(titleOverride);

                var prefabContainer = x.GetComponent<PrefabContainer>();
                prefabContainer.Prefabs.AddRange(_cellPrefabs);

                onLoad?.Invoke(x);
            }, loadAsync: false);
        }

        public AsyncProcessHandle PopPage(bool playAnimation)
        {
            return _pageContainer.Pop(playAnimation);
        }

        public void Show()
        {
            _drawerController.SetStateWithAnimation(DrawerState.Max);
        }

        public void Hide()
        {
            _drawerController.SetStateWithAnimation(DrawerState.Min);
        }

        private void OnBackButtonClicked()
        {
            _pageContainer.Pop(true);
        }

        private void OnCloseButtonClicked()
        {
            _drawerController.SetStateWithAnimation(DrawerState.Min);
        }

        private void OnTransitionProgressChanged(float progress)
        {
            var exitTitleCol = _exitTitleText.color;
            exitTitleCol.a = 1.0f - progress;
            _exitTitleText.color = exitTitleCol;

            var enterTitleCol = _enterTitleText.color;
            enterTitleCol.a = progress;
            _enterTitleText.color = enterTitleCol;

            if (EnteringDebugPage.TransitionAnimationType == PageTransitionAnimationType.PushEnter
                && ExitingDebugPage != null && ExitingDebugPage == InitialDebugPage)
                SetBackButtonVisibility(progress);

            if (EnteringDebugPage.TransitionAnimationType == PageTransitionAnimationType.PopEnter &&
                EnteringDebugPage == InitialDebugPage)
                SetBackButtonVisibility(1.0f - progress);
        }

        private void SetBackButtonVisibility(float visibility)
        {
            var color = _backButton.image.color;
            color.a = visibility;
            _backButton.image.color = color;
        }
    }
}
