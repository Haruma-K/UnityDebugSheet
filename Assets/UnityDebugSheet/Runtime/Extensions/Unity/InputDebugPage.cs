using System.Collections.Generic;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public sealed class InputDebugPage : PropertyListDebugPageBase<Input>
    {
        public override List<string> UpdateTargetPropertyNames { get; } = new List<string>
        {
            nameof(Input.mousePosition),
            nameof(Input.mouseScrollDelta),
            nameof(Input.compositionCursorPos),
            nameof(Input.touchCount),
            nameof(Input.acceleration),
            nameof(Input.touches)
        };

        public override Dictionary<string, SubPageInfo> SubPageTargetPropertyNameToInfoMap { get; } =
            new Dictionary<string, SubPageInfo>
            {
                {
                    nameof(Input.gyro),
                    new SubPageInfo(typeof(GyroscopeDebugPage), x => ((GyroscopeDebugPage)x.page).Setup(Input.gyro))
                },
                {
                    nameof(Input.compass),
                    new SubPageInfo(typeof(CompassDebugPage), x => ((CompassDebugPage)x.page).Setup(Input.compass))
                }
            };
    }
}
