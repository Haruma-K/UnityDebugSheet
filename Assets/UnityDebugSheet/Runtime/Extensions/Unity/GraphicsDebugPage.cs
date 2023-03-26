using System.Collections.Generic;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public sealed class GraphicsDebugPage : PropertyListDebugPageBase<Graphics>
    {
        public override List<string> IgnoreTargetPropertyNames { get; } = new List<string>
        {
            nameof(Graphics.activeColorBuffer),
            nameof(Graphics.activeDepthBuffer)
        };
    }
}
