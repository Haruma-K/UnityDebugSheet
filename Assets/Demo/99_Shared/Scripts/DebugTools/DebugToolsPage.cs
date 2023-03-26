#if !EXCLUDE_UNITY_DEBUG_SHEET
using System.Collections;
using IngameDebugConsole;
using Tayx.Graphy;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityDebugSheet.Runtime.Extensions.Graphy;
using UnityDebugSheet.Runtime.Extensions.IngameDebugConsole;
using UnityDebugSheet.Runtime.Extensions.Unity;
using UnityEngine;
#if UDS_USE_ASYNC_METHODS
using System.Threading.Tasks;
#endif

namespace Demo._99_Shared.Scripts.DebugTools
{
    public sealed class DebugToolsPage : DefaultDebugPageBase
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
                onLoad: x => x.page.Setup(GraphyManager.Instance));
            
            // In-Game Debug Console
            AddPageLinkButton<IngameDebugConsoleDebugPage>("In-Game Debug Console",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Console),
                onLoad: x => x.page.Setup(DebugLogManager.Instance));

            // System Info
            AddPageLinkButton<SystemInfoDebugPage>("System Info",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            // Time
            AddPageLinkButton<TimeDebugPage>("Time",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            // Application
            AddPageLinkButton<ApplicationDebugPage>("Application",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            // Screen
            AddPageLinkButton<ScreenDebugPage>("Screen",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            // Quality Settings
            AddPageLinkButton<QualitySettingsDebugPage>("Quality Settings",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            // Input
            AddPageLinkButton<InputDebugPage>("Input",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            // Physics
            AddPageLinkButton<PhysicsDebugPage>("Physics",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            // Physics 2D
            AddPageLinkButton<Physics2DDebugPage>("Physics 2D",
                icon: Resources.Load<Sprite>(AssetKeys.Resources.Icon.Settings));

            // Graphics
            AddPageLinkButton<GraphicsDebugPage>("Graphics",
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
