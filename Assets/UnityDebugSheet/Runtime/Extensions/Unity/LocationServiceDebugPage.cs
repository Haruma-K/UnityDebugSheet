using System.Reflection;
using System.Text;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Extensions.Unity
{
    public sealed class LocationServiceDebugPage : PropertyListDebugPageBase<LocationService>
    {
        private object _targetObject;
        protected override string Title => "Location Service";

        public override BindingFlags BindingFlags => BindingFlags.Static | BindingFlags.Public | BindingFlags.Instance;
        public override object TargetObject => _targetObject;


        public void Setup(LocationService targetObject)
        {
            _targetObject = targetObject;
        }

        protected override bool TryGetOverridePropertyDescription(string propertyName, object value,
            out string description)
        {
            if (propertyName == "lastData")
            {
                var lastData = (LocationInfo)value;
                description = LocationInfoToString(lastData);
                return true;
            }

            description = null;
            return false;
        }

        private string LocationInfoToString(LocationInfo locationInfo)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("{ ");
            stringBuilder.Append($"{nameof(LocationInfo.latitude)}: {locationInfo.latitude}");
            stringBuilder.Append($", {nameof(LocationInfo.longitude)}: {locationInfo.longitude}");
            stringBuilder.Append($", {nameof(LocationInfo.altitude)}: {locationInfo.altitude}");
            stringBuilder.Append($", {nameof(LocationInfo.horizontalAccuracy)}: {locationInfo.horizontalAccuracy}");
            stringBuilder.Append($", {nameof(LocationInfo.verticalAccuracy)}: {locationInfo.verticalAccuracy}");
            stringBuilder.Append($", {nameof(LocationInfo.timestamp)}: {locationInfo.timestamp}");
            stringBuilder.Append(" }");
            return stringBuilder.ToString();
        }
    }
}
