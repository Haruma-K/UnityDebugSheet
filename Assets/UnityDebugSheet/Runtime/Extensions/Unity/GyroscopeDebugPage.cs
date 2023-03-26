using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public sealed class GyroscopeDebugPage : PropertyListDebugPageBase<Gyroscope>
    {
        private object _targetObject;

        public override BindingFlags BindingFlags => BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance;
        public override object TargetObject => _targetObject;

        public override List<string> UpdateTargetPropertyNames { get; } = new List<string>
        {
            "rotationRate",
            "rotationRateUnbiased",
            "gravity",
            "userAcceleration",
            "attitude"
        };

        public void Setup(Gyroscope targetObject)
        {
            _targetObject = targetObject;
        }
    }
}
