using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Foundation.Drawer;
using UnityDebugSheet.Runtime.Foundation.Gestures.Flicks;
using UnityDebugSheet.Runtime.Foundation.Gestures.MultiClicks;
using UnityDebugSheet.Runtime.Foundation.PageNavigator;
using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules;
using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules.AssetLoader;
using UnityEngine;
using UnityEngine.Serialization;
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

        [FormerlySerializedAs("_globalControlMode")] [SerializeField]
        private FlickToOpenMode _flickToOpen = FlickToOpenMode.Edge;

        [SerializeField] private ClickToOpenMode _clickToOpen = ClickToOpenMode.Off;
        [SerializeField] private int _clickCountToOpen = 3;
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
        [SerializeField] private FloatingButton _floatingButton;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private MultiClickEventDispatcher _openButton;
        private float _dpi;
        private bool _isInitialized;
        private PreloadedAssetLoader _preloadedAssetLoader;

        public static DebugSheet Instance { get; private set; }

        public string InitialPageId { get; private set; }
        public DebugPageBase InitialDebugPage { get; private set; }
        public DebugPageBase CurrentDebugPage { get; private set; }
        public DebugPageBase EnteringDebugPage { get; private set; }
        public DebugPageBase ExitingDebugPage { get; private set; }
        public IReadOnlyDictionary<string, Page> Pages => _pageContainer.Pages;
        public IList<GameObject> CellPrefabs => _cellPrefabs;

        public FlickToOpenMode FlickToOpen
        {
            get => _flickToOpen;
            set => _flickToOpen = value;
        }

        public ClickToOpenMode ClickToOpen
        {
            get => _clickToOpen;
            set => _clickToOpen = value;
        }

        public int ClickCountToOpen
        {
            get => _clickCountToOpen;
            set => _clickCountToOpen = value;
        }

        public KeyboardShortcut KeyboardShortcut => _keyboardShortcut;

        public BalloonButton BalloonButton => _balloonButton;

        public FloatingButton FloatingButton => _floatingButton;

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
            SetupMultiClickEventDispatcher();
            _backButton.onClick.AddListener(OnBackButtonClicked);
            _closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        private void OnDisable()
        {
            _backButton.onClick.RemoveListener(OnBackButtonClicked);
            _closeButton.onClick.RemoveListener(OnCloseButtonClicked);
            _flickEvent.flicked.RemoveListener(OnFlicked);
            CleanupMultiClickEventDispatcher();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        void IPageContainerCallbackReceiver.BeforePush(Page enterPage, Page exitPage)
        {
            _backButton.interactable = false;
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
            _backButton.interactable = false;
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

        public TInitialPage Initialize<TInitialPage>(string titleOverride = null,
            Action<(string pageId, TInitialPage page)> onLoad = null, string pageId = null)
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
                InitialPageId = x.pageId;
                InitialDebugPage = x.page;
                onLoad?.Invoke((x.pageId, x.page));
            }, pageId);
            _isInitialized = true;
            return (TInitialPage)InitialDebugPage;
        }

        public TInitialPage GetOrCreateInitialPage<TInitialPage>(string titleOverride = null,
            Action<(string pageId, TInitialPage page)> onLoad = null, string pageId = null)
            where TInitialPage : DebugPageBase
        {
            if (_isInitialized)
                return (TInitialPage)InitialDebugPage;

            return Initialize(titleOverride, onLoad, pageId);
        }

        public DebugPage GetOrCreateInitialPage(string titleOverride = null, string pageId = null,
            Action<(string pageId, DebugPage page)> onLoad = null)
        {
            return GetOrCreateInitialPage(titleOverride, onLoad, pageId);
        }

        public AsyncProcessHandle PushPage(Type pageType, DebugPageBase prefab, bool playAnimation,
            string titleOverride = null, Action<(string pageId, DebugPageBase page)> onLoad = null,
            string pageId = null)
        {
            if (!_preloadedAssetLoader.PreloadedObjects.ContainsValue(prefab.gameObject))
                _preloadedAssetLoader.AddObject(prefab.gameObject);

            return PushPage(pageType, prefab.gameObject.name, playAnimation, titleOverride, onLoad, pageId);
        }

        public AsyncProcessHandle PushPage<TPage>(TPage prefab, bool playAnimation, string titleOverride = null,
            Action<(string pageId, TPage page)> onLoad = null, string pageId = null) where TPage : DebugPageBase
        {
            if (!_preloadedAssetLoader.PreloadedObjects.ContainsValue(prefab.gameObject))
                _preloadedAssetLoader.AddObject(prefab.gameObject);

            return PushPage(typeof(TPage), prefab.gameObject.name, playAnimation, titleOverride,
                x => onLoad?.Invoke((pageId, (TPage)x.page)), pageId);
        }

        public AsyncProcessHandle PushPage(Type pageType, bool playAnimation, string titleOverride = null,
            Action<(string pageId, DebugPageBase page)> onLoad = null, string pageId = null)
        {
            return PushPage(pageType, _pagePrefab.gameObject.name, playAnimation, titleOverride, onLoad, pageId);
        }

        public AsyncProcessHandle PushPage<TPage>(bool playAnimation, string titleOverride = null,
            Action<(string pageId, TPage page)> onLoad = null, string pageId = null) where TPage : DebugPageBase
        {
            return PushPage(typeof(TPage), _pagePrefab.gameObject.name, playAnimation, titleOverride,
                x => onLoad?.Invoke((x.pageId, (TPage)x.page)), pageId);
        }

        private AsyncProcessHandle PushPage(Type pageType, string prefabName, bool playAnimation,
            string titleOverride = null, Action<(string pageId, DebugPageBase page)> onLoad = null,
            string pageId = null)
        {
            return _pageContainer.Push(pageType, prefabName, playAnimation, pageId: pageId, onLoad: x =>
            {
                var debugPage = (DebugPageBase)x.page;
                if (titleOverride != null)
                    debugPage.SetTitle(titleOverride);

                var prefabContainer = debugPage.GetComponent<PrefabContainer>();
                prefabContainer.Prefabs.AddRange(_cellPrefabs);

                onLoad?.Invoke((x.pageId, debugPage));
            }, loadAsync: false);
        }

        public AsyncProcessHandle PopPage(bool playAnimation, int popCount = 1)
        {
            return _pageContainer.Pop(playAnimation, popCount);
        }

        public AsyncProcessHandle PopPage(bool playAnimation, string destinationPageId)
        {
            return _pageContainer.Pop(playAnimation, destinationPageId);
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
            if (_flickToOpen == FlickToOpenMode.Off)
                return;

            // If it is horizontal flick, ignore it.
            var isVertical = Mathf.Abs(flick.DeltaInchPosition.y) > Mathf.Abs(flick.DeltaInchPosition.x);
            if (!isVertical)
                return;

            // Determines whether flicking is valid or not by the global control mode.
            var startPosXInch = flick.TouchStartScreenPosition.x / _dpi;
            var totalInch = Screen.width / _dpi;
            var leftSafeAreaInch = Screen.safeArea.xMin / _dpi;
            var isLeftEdge = startPosXInch <= ThresholdInch + leftSafeAreaInch;
            var rightSafeAreaInch = (Screen.width - Screen.safeArea.xMax) / _dpi;
            var isRightEdge = startPosXInch >= totalInch - (ThresholdInch + rightSafeAreaInch);
            var isValid = false;
            switch (_flickToOpen)
            {
                case FlickToOpenMode.Edge:
                    isValid = isLeftEdge || isRightEdge;
                    break;
                case FlickToOpenMode.LeftEdge:
                    isValid = isLeftEdge;
                    break;
                case FlickToOpenMode.RightEdge:
                    isValid = isRightEdge;
                    break;
                case FlickToOpenMode.Off:
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

        private void SetupMultiClickEventDispatcher()
        {
            var shouldActivate = _clickToOpen != ClickToOpenMode.Off;
            _openButton.gameObject.SetActive(shouldActivate);
            _openButton.onMultiClicked.AddListener(OnOpenButtonMultiClicked);
            _openButton.clickCountThreshold = _clickCountToOpen;
            var rectTrans = (RectTransform)_openButton.transform;
            _clickToOpen.ApplyToRectTransform(rectTrans);
        }

        private void CleanupMultiClickEventDispatcher()
        {
            _openButton.onMultiClicked.RemoveListener(OnOpenButtonMultiClicked);
        }

        private void OnOpenButtonMultiClicked()
        {
            if (_clickToOpen == ClickToOpenMode.Off)
                return;

            var state = _drawer.GetNearestState() == DrawerState.Max ? DrawerState.Min : DrawerState.Max;
            _drawerController.SetStateWithAnimation(state);
        }
    }
}
