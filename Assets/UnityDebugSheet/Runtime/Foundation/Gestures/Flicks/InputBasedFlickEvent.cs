using UnityEngine.Events;

namespace UnityDebugSheet
{
    public sealed class InputBasedFlickEvent : InputBasedFlickListenerBase
    {
        public UnityEvent<Flick> flicked;

        protected override void Flicked(Flick flick)
        {
            flicked?.Invoke(flick);
        }
    }
}
