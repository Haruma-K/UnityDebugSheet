using UnityEngine;
using UnityEngine.Events;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.MultiClicks
{
    public abstract class ClickEventDispatcherBase : MonoBehaviour
    {
        public UnityEvent onClicked = new UnityEvent();
    }
}
