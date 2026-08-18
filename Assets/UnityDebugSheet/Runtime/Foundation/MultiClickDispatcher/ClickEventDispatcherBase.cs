using UnityEngine;
using UnityEngine.Events;

namespace UnityDebugSheet
{
    public abstract class ClickEventDispatcherBase : MonoBehaviour
    {
        public UnityEvent onClicked = new UnityEvent();
    }
}
