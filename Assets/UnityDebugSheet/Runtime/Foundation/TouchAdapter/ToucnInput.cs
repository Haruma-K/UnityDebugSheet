//#undef ENABLE_INPUT_SYSTEM // For Debug

using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.EnhancedTouch;
using UnityTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityTouchPhase = UnityEngine.InputSystem.TouchPhase;

#else
using UnityTouch = UnityEngine.Touch;
using UnityTouchPhase = UnityEngine.TouchPhase;
#endif

namespace TouchAdapter
{
    public static class TouchInput
    {
        public static int Count
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return UnityTouch.activeTouches.Count;
#else
                return Input.touchCount;
#endif
            }
        }

#if ENABLE_INPUT_SYSTEM
        [RuntimeInitializeOnLoadMethod]
        private static void Enable()
        {
            EnhancedTouchSupport.Enable();
        }
#endif

        public static Touch Get(int index)
        {
#if ENABLE_INPUT_SYSTEM
            var unityTouch = UnityTouch.activeTouches[index];
#else
            var unityTouch = Input.GetTouch(index);
#endif
            return ConvertUnityTouch(unityTouch);
        }

        private static Touch ConvertUnityTouch(UnityTouch unityTouch)
        {
#if ENABLE_INPUT_SYSTEM
            var touchPhase = ConvertUnityTouchPhase(unityTouch.phase);
            return new Touch(unityTouch.finger.index, unityTouch.screenPosition, unityTouch.pressure, touchPhase);
#else
            var touchPhase = ConvertUnityTouchPhase(unityTouch.phase);
            return new Touch(unityTouch.fingerId, unityTouch.position, unityTouch.pressure, touchPhase);
#endif
        }

        private static TouchPhase ConvertUnityTouchPhase(UnityTouchPhase self)
        {
            return self switch
            {
#if ENABLE_INPUT_SYSTEM
                UnityTouchPhase.None => TouchPhase.None,
#endif
                UnityTouchPhase.Began => TouchPhase.Began,
                UnityTouchPhase.Moved => TouchPhase.Moved,
                UnityTouchPhase.Ended => TouchPhase.Ended,
                UnityTouchPhase.Canceled => TouchPhase.Canceled,
                UnityTouchPhase.Stationary => TouchPhase.Stationary,
                _ => throw new ArgumentOutOfRangeException(nameof(self), self, null)
            };
        }
    }
}
