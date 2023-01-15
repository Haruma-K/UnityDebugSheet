#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

namespace Demo._99_Shared.Scripts.DebugTools
{
    public sealed class DebugTools : MonoBehaviour
    {
        private static DebugTools _instance;

        public GraphyManager graphyManager;
        public DebugLogManager debugLogManager;

        private DefaultDebugPageBase _initialPage;
        private int _linkButtonId = -1;

        public static DebugTools Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("The singleton instance of the DebugTools does not exits.");
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
                return;
            }

            Destroy(gameObject);
        }

        private void Start()
        {
            _initialPage = DebugSheet.Instance.GetOrCreateInitialPage();
            _linkButtonId = _initialPage.AddPageLinkButton<DebugToolsPage>("Debug Tools",
                icon: DemoSprites.Icon.Tools,
                priority: int.MaxValue);
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
