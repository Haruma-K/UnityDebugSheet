using System.Collections;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween
{
    public static class TweenRoutineFactory
    {
        public static IEnumerator Create<T>(T from, T to, float duration, ValueChangedDelegate<T> valueChanged,
            LerpDelegate<T> lerp, CompletedDelegate completed = null, IInterpolator interpolator = null,
            GetDeltaTimeDelegate getDeltaTime = null)

        {
            var timeSec = 0.0f;

            valueChanged?.Invoke(from);

            while (true)
            {
                yield return null;

                if (getDeltaTime == null)
                    timeSec += Time.unscaledDeltaTime;
                else
                    timeSec += getDeltaTime.Invoke();

                var progress = Mathf.Min(1.0f, timeSec / duration);
                if (interpolator != null)
                    progress = interpolator.Interpolate(progress);
                var value = lerp.Invoke(from, to, progress);
                valueChanged?.Invoke(value);

                if (timeSec >= duration)
                    break;
            }

            completed?.Invoke();
        }
    }
}
