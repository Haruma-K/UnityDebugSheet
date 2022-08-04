#if (UNITY_IOS || UNITY_ANDROID) && !UNITY_EDITOR
#define IS_SMARTPHONE
#endif
//#define IS_SMARTPHONE // For Debug

using UnityDebugSheet.Runtime.Foundation.Gestures.InputAdapters;
using UnityEngine;
#if IS_SMARTPHONE
using TouchPhase = UnityDebugSheet.Runtime.Foundation.Gestures.InputAdapters.TouchPhase;
#endif

namespace UnityDebugSheet.Runtime.Foundation.Gestures.Flicks
{
    public abstract class InputBasedFlickListenerBase : FlickListenerBase
    {
        protected override bool ClickStarted()
        {
#if IS_SMARTPHONE
            if (InputAdapter.TouchCount >= 1)
                return InputAdapter.GetTouch(0).TouchPhase == TouchPhase.Began;

            return false;
#else
            return InputAdapter.GetMouseButtonDown(0);
#endif
        }

        protected override bool ClickFinished()
        {
#if IS_SMARTPHONE
            if (InputAdapter.TouchCount >= 1)
                return InputAdapter.GetTouch(0).TouchPhase == TouchPhase.Ended;

            return false;
#else
            return InputAdapter.GetMouseButtonUp(0);
#endif
        }

        protected override bool TryGetClickedPosition(out Vector2 position)
        {
#if IS_SMARTPHONE
            if (InputAdapter.TouchCount >= 1)
            {
                position = InputAdapter.GetTouch(0).Position;
                return true;
            }
#else
            if (InputAdapter.GetMouseButton(0))
            {
                position = InputAdapter.GetMousePosition();
                return true;
            }
#endif

            position = default;
            return false;
        }
    }
}
