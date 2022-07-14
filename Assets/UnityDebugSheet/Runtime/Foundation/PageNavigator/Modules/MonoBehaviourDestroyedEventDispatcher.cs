using System;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules
{
    public class MonoBehaviourDestroyedEventDispatcher : MonoBehaviour
    {
        public void OnDestroy()
        {
            OnDispatch?.Invoke();
        }

        public event Action OnDispatch;
    }
}