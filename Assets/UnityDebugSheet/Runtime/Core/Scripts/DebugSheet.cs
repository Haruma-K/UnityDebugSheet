using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Foundation.Drawer;
using UnityDebugSheet.Runtime.Foundation.Gestures.Flicks;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules;
using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules.AssetLoader;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    [DefaultExecutionOrder(int.MinValue)]
    public sealed class DebugSheet : MonoBehaviour, IPageContainerCallbackReceiver
    {
        private const float ThresholdInch = 0.24f;

        private static readonly Dictionary<int, DebugSheet> InstanceCacheByTransform =
            new Dictionary<int, DebugSheet>();

        [SerializeField] private bool _singleton = true;
        [SerializeField] private GlobalControlMode _globalControlMode = GlobalControlMode.FlickEdge;
        [SerializeField] private KeyboardShortcut _keyboardShortcut = new KeyboardShortcut();
        [SerializeField] private List<GameObject> _cellPrefabs = new List<GameObject>();
        [SerializeField] private StatefulDrawer _drawer;
        [SerializeField] private StatefulDrawerController _drawerController;
        [SerializeField] private Button _backButton;
        [SerializeField] private Text _exitTitleText;
        [SerializeField] private Text _enterTitleText;
        [SerializeField] private Button _closeButton;
        [SerializeField] private PageContainer _pageContainer;
        [SerializeField] [HideInInspector] private GameObject _pagePrefab;
        [SerializeField] private InputBasedFlickEvent _flickEvent;
        [SerializeField] private BalloonButton _balloonButton;
        [SerializeField] private Canvas _canvas;
        private float _dpi;
        private bool _isInitialized;
        private PreloadedAssetLoader _preloadedAssetLoader;

        public static DebugSheet Instance { get; private set; }

        public DebugPageBase InitialDebugPage { get; private set; }
        public DebugPageBase CurrentDebugPage { get; private set; }
        public DebugPageBase EnteringDebugPage { get; private set; }
        public DebugPageBase ExitingDebugPage { get; private set; }
        public IReadOnlyList<Page> Pages => _pageContainer.Pages;
        public IList<GameObject> CellPrefabs => _cellPrefabs;

        public GlobalControlMode GlobalControlMode
        {
            get => _globalControlMode;
            set => _globalControlMode = value;
        }

        public KeyboardShortcut KeyboardShortcut => _keyboardShortcut;

        public BalloonButton BalloonButton => _balloonButton;

        private void Awake()
        {
            var dpi = Screen.dpi;
            if (dpi == 0)
                dpi = 326;
            _dpi = dpi;

            _backButton.interactable = false;
            SetBackButtonVisibility(0.0f);
            _balloonButton.Initialize(_canvas);

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

        private void Update()
        {
            if (_keyboardShortcut.Evaluate())
            {
                var isClosed = Mathf.Approximately(_drawer.Progress, _drawer.MinProgress);
                var targetState = isClosed ? DrawerState.Max : DrawerState.Min;
                _drawerController.SetStateWithAnimation(targetState);
            }
        }

        private void OnEnable()
        {
            _flickEvent.flicked.AddListener(OnFlicked);
            _backButton.onClick.AddListener(OnBackButtonClicked);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            _flickEvent.flicked.RemoveListener(OnFlicked);
        }

        private void OnDestroy()
        {
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

            _backButton.interactable = false;
            SetBackButtonVisibility(0.0f);
            _pageContainer.AddCallbackReceiver(this);
            var preloadedAssetLoader = new PreloadedAssetLoader();
            _preloadedAssetLoader = preloadedAssetLoader;
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
        /// <returns></returns>
        public DebugPageBase GetOrCreateInitialPage<TInitialPage>(string titleOverride = null,
            Action<TInitialPage> onLoad = null) where TInitialPage : DebugPageBase
        {
            if (_isInitialized)
                return InitialDebugPage;

            return Initialize(titleOverride, onLoad);
        }

        public DebugPageBase GetOrCreateInitialPage(string titleOverride = null, Action<DebugPage> onLoad = null)
        {
            return GetOrCreateInitialPage<DebugPage>(titleOverride, onLoad);
        }

        public AsyncProcessHandle PushPage(Type pageType, DebugPageBase prefab, bool playAnimation,
            string titleOverride = null,
            Action<DebugPageBase> onLoad = null)
        {
            if (!_preloadedAssetLoader.PreloadedObjects.ContainsValue(prefab.gameObject))
                _preloadedAssetLoader.AddObject(prefab.gameObject);

            return PushPage(pageType, prefab.gameObject.name, playAnimation, titleOverride, onLoad);
        }

        public AsyncProcessHandle PushPage<TPage>(TPage prefab, bool playAnimation, string titleOverride = null,
            Action<TPage> onLoad = null) where TPage : DebugPageBase
        {
            if (!_preloadedAssetLoader.PreloadedObjects.ContainsValue(prefab.gameObject))
                _preloadedAssetLoader.AddObject(prefab.gameObject);

            return PushPage(typeof(TPage), prefab.gameObject.name, playAnimation, titleOverride,
                x => onLoad?.Invoke((TPage)x));
        }

        public AsyncProcessHandle PushPage(Type pageType, bool playAnimation, string titleOverride = null,
            Action<DebugPageBase> onLoad = null)
        {
            return PushPage(pageType, _pagePrefab.gameObject.name, playAnimation, titleOverride, onLoad);
        }

        public AsyncProcessHandle PushPage<TPage>(bool playAnimation, string titleOverride = null,
            Action<TPage> onLoad = null) where TPage : DebugPageBase
        {
            return PushPage(typeof(TPage), _pagePrefab.gameObject.name, playAnimation, titleOverride,
                x => onLoad?.Invoke((TPage)x));
        }

        private AsyncProcessHandle PushPage(Type pageType, string prefabName, bool playAnimation,
            string titleOverride = null,
            Action<DebugPageBase> onLoad = null)
        {
            return _pageContainer.Push(pageType, prefabName, playAnimation, onLoad: x =>
            {
                var debugPage = (DebugPageBase)x;
                if (titleOverride != null)
                    debugPage.SetTitle(titleOverride);

                var prefabContainer = x.GetComponent<PrefabContainer>();
                prefabContainer.Prefabs.AddRange(_cellPrefabs);

                onLoad?.Invoke(debugPage);
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
            if (_pageContainer.IsInTransition)
                return;

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

        private void OnFlicked(Flick flick)
        {
            if (_globalControlMode == GlobalControlMode.None)
                return;

            // If it is horizontal flick, ignore it.
            var isVertical = Mathf.Abs(flick.DeltaInchPosition.y) > Mathf.Abs(flick.DeltaInchPosition.x);
            if (!isVertical)
                return;

            // Determines whether flicking is valid or not by the global control mode.
            var startPosXInch = flick.StartScreenPosition.x / _dpi;
            var totalInch = Screen.width / _dpi;
            var leftSafeAreaInch = Screen.safeArea.xMin / _dpi;
            var isLeftEdge = startPosXInch <= ThresholdInch + leftSafeAreaInch;
            var rightSafeAreaInch = (Screen.width - Screen.safeArea.xMax) / _dpi;
            var isRightEdge = startPosXInch >= totalInch - (ThresholdInch + rightSafeAreaInch);
            var isValid = false;
            switch (_globalControlMode)
            {
                case GlobalControlMode.FlickEdge:
                    isValid = isLeftEdge || isRightEdge;
                    break;
                case GlobalControlMode.FlickLeftEdge:
                    isValid = isLeftEdge;
                    break;
                case GlobalControlMode.FlickRightEdge:
                    isValid = isRightEdge;
                    break;
                case GlobalControlMode.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!isValid)
                return;

            // Apply the flick.
            var isUp = flick.DeltaInchPosition.y >= 0;
            var state = isUp ? _drawer.GetUpperState() : _drawer.GetLowerState();
            _drawerController.SetStateWithAnimation(state);
        }
    }
}
