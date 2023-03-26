using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public sealed class CompassDebugPage : PropertyListDebugPageBase<Compass>
    {
        private object _targetObject;

        public override BindingFlags BindingFlags => BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance;
        public override object TargetObject => _targetObject;

        public override List<string> UpdateTargetPropertyNames { get; } = new List<string>
        {
            "magneticHeading",
            "trueHeading",
            "headingAccuracy",
            "rawVector",
            "timestamp"
        };

        public void Setup(Compass targetObject)
        {
            _targetObject = targetObject;
        }
    }
}
