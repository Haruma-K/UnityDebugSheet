using UnityEngine.Events;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.Flicks
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
