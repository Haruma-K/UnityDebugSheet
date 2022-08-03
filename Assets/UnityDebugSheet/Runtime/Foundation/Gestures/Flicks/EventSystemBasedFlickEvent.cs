using UnityEngine.Events;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.Flicks
{
    public sealed class EventSystemBasedFlickEvent : EventSystemBasedFlickListenerBase
    {
        public UnityEvent<Flick> flicked;

        protected override void Flicked(Flick flick)
        {
            flicked?.Invoke(flick);
        }
    }
}
