using UnityEngine.Events;

namespace UnityDebugSheet
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
