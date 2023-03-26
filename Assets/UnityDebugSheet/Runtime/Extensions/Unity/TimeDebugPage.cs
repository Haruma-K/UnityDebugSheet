using System.Collections.Generic;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public sealed class TimeDebugPage : PropertyListDebugPageBase<Time>
    {
        public override List<string> UpdateTargetPropertyNames { get; } = new List<string>
        {
            nameof(Time.time),
            nameof(Time.timeAsDouble),
            nameof(Time.timeSinceLevelLoad),
            nameof(Time.timeSinceLevelLoadAsDouble),
            nameof(Time.deltaTime),
            nameof(Time.fixedTime),
            nameof(Time.fixedTimeAsDouble),
            nameof(Time.unscaledTime),
            nameof(Time.unscaledTimeAsDouble),
            nameof(Time.fixedUnscaledTime),
            nameof(Time.fixedUnscaledTimeAsDouble),
            nameof(Time.unscaledDeltaTime),
            nameof(Time.fixedUnscaledDeltaTime),
            nameof(Time.fixedDeltaTime),
            nameof(Time.maximumDeltaTime),
            nameof(Time.smoothDeltaTime),
            nameof(Time.timeScale),
            nameof(Time.frameCount),
            nameof(Time.renderedFrameCount),
            nameof(Time.realtimeSinceStartup),
            nameof(Time.realtimeSinceStartupAsDouble),
            nameof(Time.captureDeltaTime),
            nameof(Time.captureFramerate)
        };
    }
}
