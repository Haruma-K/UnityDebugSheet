#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using System.Collections;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.Cells;
using UnityDebugSheet.Runtime.Extensions.Graphy;
using UnityDebugSheet.Runtime.Extensions.IngameDebugConsole;
using UnityDebugSheet.Runtime.Extensions.Unity;
using UnityEngine;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace Demo._99_Shared.Scripts.DebugTools
{
    public sealed class DebugToolsPage : DebugPageBase
    {
        protected override string Title => "Debug Tools";

#if UDS_USE_ASYNC_METHODS
        public override Task Initialize()
#else
        public override IEnumerator Initialize()
#endif
        {
            // Graphy
            AddPageLinkButton<GraphyDebugPage>("Graphy",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.FPS),
                onLoad: x => x.Setup(GraphyManager.Instance));
            
            // In-Game Debug Console
            AddPageLinkButton<IngameDebugConsoleDebugPage>("In-Game Debug Console",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Console),
                onLoad: x => x.Setup(DebugLogManager.Instance));

            // System Info
            AddPageLinkButton<SystemInfoDebugPage>("System Info",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            Reload();

#if UDS_USE_ASYNC_METHODS
            return Task.CompletedTask;
#else
            yield break;
#endif
        }
    }
}
#endif
