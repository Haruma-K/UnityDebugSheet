//#undef ENABLE_INPUT_SYSTEM // For Debug

#if ENABLE_INPUT_SYSTEM
using UnityTouch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using UnityTouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;

#else
using UnityTouch = UnityEngine.Touch;
using UnityTouchPhase = UnityEngine.TouchPhase;
#endif
using System;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.InputAdapters
{
    public static class InputAdapter
    {
        public static int TouchCount
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

        public static bool MousePresent
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current != null;
#else
                return Input.mousePresent;
#endif
            }
        }

        public static float MouseScrollDelta
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return Mouse.current.scroll != null ? Mouse.current.scroll.ReadValue().y : 0.0f;
#else
                return Input.mouseScrollDelta.y;
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

        public static Touch GetTouch(int index)
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

#if ENABLE_INPUT_SYSTEM
        private static ButtonControl GetMouseButtonControl(int index)
        {
            if (Mouse.current == null)
                return null;

            switch (index)
            {
                case 0: return Mouse.current.leftButton;
                case 1: return Mouse.current.rightButton;
                case 2: return Mouse.current.middleButton;
                case 3: return Mouse.current.forwardButton;
                case 4: return Mouse.current.backButton;
            }

            return null;
        }
#endif

        public static bool GetMouseButtonDown(int index)
        {
#if ENABLE_INPUT_SYSTEM
            var control = GetMouseButtonControl(index);
            return control?.wasPressedThisFrame ?? false;
#else
            return Input.GetMouseButtonDown(index);
#endif
        }

        public static bool GetMouseButton(int index)
        {
#if ENABLE_INPUT_SYSTEM
            var control = GetMouseButtonControl(index);
            return control?.isPressed ?? false;
#else
            return Input.GetMouseButton(index);
#endif
        }

        public static bool GetMouseButtonUp(int index)
        {
#if ENABLE_INPUT_SYSTEM
            var control = GetMouseButtonControl(index);
            return control?.wasReleasedThisFrame ?? false;
#else
            return Input.GetMouseButtonUp(index);
#endif
        }

        public static Vector2 GetMousePosition()
        {
#if ENABLE_INPUT_SYSTEM
			return Mouse.current != null ? Mouse.current.position.ReadValue() : default;
#else
            return Input.mousePosition;
#endif
        }

        public static bool GetKeyDown(KeyCode keyCode)
        {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current[keyCode.ToInputSystemKey()].wasPressedThisFrame;
#else
            return Input.GetKeyDown(keyCode);
#endif
        }

        public static bool GetKey(KeyCode keyCode)
        {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current[keyCode.ToInputSystemKey()].isPressed;
#else
            return Input.GetKey(keyCode);
#endif
        }

        public static bool GetMouseButtonUp(KeyCode keyCode)
        {
#if ENABLE_INPUT_SYSTEM
        return Keyboard.current[keyCode.ToInputSystemKey()].wasReleasedThisFrame;
#else
            return Input.GetKeyUp(keyCode);
#endif
        }
    }
}
