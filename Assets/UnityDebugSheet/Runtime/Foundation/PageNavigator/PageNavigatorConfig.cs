using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules.AssetLoader;
#if UNITY_EDITOR
#endif

namespace UnityDebugSheet.Runtime.Foundation.PageNavigator
{
    public sealed class PageNavigatorConfig
    {
        private static PageNavigatorConfig _instance;

        private IAssetLoader _assetLoader;
        private ITransitionAnimation _pagePopEnterAnimation;
        private ITransitionAnimation _pagePopExitAnimation;
        private ITransitionAnimation _pagePushEnterAnimation;
        private ITransitionAnimation _pagePushExitAnimation;

        public static PageNavigatorConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PageNavigatorConfig();

                return _instance;
            }
        }

        public ITransitionAnimation PagePushEnterAnimation
        {
            get
            {
                if (_pagePushEnterAnimation == null)
                    _pagePushEnterAnimation =
                        SimpleTransitionAnimation.CreateInstance(beforeAlignment: PageAlignment.Right,
                            afterAlignment: PageAlignment.Center);

                return _pagePushEnterAnimation;
            }
            set => _pagePushEnterAnimation = value;
        }

        public ITransitionAnimation PagePushExitAnimation
        {
            get
            {
                if (_pagePushExitAnimation == null)
                    _pagePushExitAnimation =
                        SimpleTransitionAnimation.CreateInstance(beforeAlignment: PageAlignment.Center,
                            afterAlignment: PageAlignment.Left);

                return _pagePushExitAnimation;
            }
            set => _pagePushExitAnimation = value;
        }

        public ITransitionAnimation PagePopEnterAnimation
        {
            get
            {
                if (_pagePopEnterAnimation == null)
                    _pagePopEnterAnimation =
                        SimpleTransitionAnimation.CreateInstance(beforeAlignment: PageAlignment.Left,
                            afterAlignment: PageAlignment.Center);

                return _pagePopEnterAnimation;
            }
            set => _pagePopEnterAnimation = value;
        }

        public ITransitionAnimation PagePopExitAnimation
        {
            get
            {
                if (_pagePopExitAnimation == null)
                    _pagePopExitAnimation =
                        SimpleTransitionAnimation.CreateInstance(beforeAlignment: PageAlignment.Center,
                            afterAlignment: PageAlignment.Right);

                return _pagePopExitAnimation;
            }
            set => _pagePopExitAnimation = value;
        }

        public IAssetLoader AssetLoader
        {
            get
            {
                if (_assetLoader == null)
                    _assetLoader = new ResourcesAssetLoader();
                return _assetLoader;
            }
            set => _assetLoader = value;
        }

        public bool EnableInteractionInTransition { get; set; }

        public ITransitionAnimation GetTransitionAnimation(bool push, bool enter)
        {
            if (push)
                return enter ? PagePushEnterAnimation : PagePushExitAnimation;
            return enter ? PagePopEnterAnimation : PagePopExitAnimation;
        }
    }
}
