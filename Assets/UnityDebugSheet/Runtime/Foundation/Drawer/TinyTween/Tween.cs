using System.Collections;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween
{
    public sealed class FloatTween : Tween<float>
    {
        public FloatTween()
        {
            To = 1.0f;
        }

        internal override float Lerp(float a, float b, float t)
        {
            return Mathf.Lerp(a, b, t);
        }
    }

    public sealed class Vector2Tween : Tween<Vector2>
    {
        public Vector2Tween()
        {
            To = Vector2.one;
        }

        internal override Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return Vector2.Lerp(a, b, t);
        }
    }

    public sealed class Vector3Tween : Tween<Vector3>
    {
        public Vector3Tween()
        {
            To = Vector3.one;
        }

        internal override Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            return Vector3.Lerp(a, b, t);
        }
    }

    public sealed class Vector4Tween : Tween<Vector4>
    {
        public Vector4Tween()
        {
            To = Vector4.one;
        }

        internal override Vector4 Lerp(Vector4 a, Vector4 b, float t)
        {
            return Vector4.Lerp(a, b, t);
        }
    }

    public sealed class QuaternionTween : Tween<Quaternion>
    {
        public bool SLerp { get; set; }

        internal override Quaternion Lerp(Quaternion a, Quaternion b, float t)
        {
            return SLerp ? Quaternion.Slerp(a, b, t) : Quaternion.Lerp(a, b, t);
        }
    }

    public sealed class ColorTween : Tween<Color>
    {
        public ColorTween()
        {
            From = Color.white;
            To = Color.white;
        }

        internal override Color Lerp(Color a, Color b, float t)
        {
            return Color.Lerp(a, b, t);
        }
    }

    public sealed class Color32Tween : Tween<Color32>
    {
        public Color32Tween()
        {
            From = Color.white;
            To = Color.white;
        }

        internal override Color32 Lerp(Color32 a, Color32 b, float t)
        {
            return Color32.Lerp(a, b, t);
        }
    }

    public abstract class Tween<T>
    {
        public T From { get; set; }

        public T To { get; set; }

        public float Duration { get; set; } = 1.0f;

        public IInterpolator Interpolator { get; set; }

        internal abstract T Lerp(T a, T b, float t);
    }

    public static class TweenExtensions
    {
        public static IEnumerator CreateRoutine<T>(this Tween<T> self,
            ValueChangedDelegate<T> valueChanged, CompletedDelegate completed = null,
            GetDeltaTimeDelegate getDeltaTime = null)
        {
            return TweenRoutineFactory.Create(self.From, self.To, self.Duration, valueChanged, self.Lerp,
                completed, self.Interpolator, getDeltaTime);
        }

        public static TweenPlayer<T> CreatePlayer<T>(this Tween<T> self,
            ValueChangedDelegate<T> valueChanged, CompletedDelegate completed = null)
        {
            var player = new TweenPlayer<T>(self);
            player.ValueChanged += valueChanged;
            player.Completed += completed;
            return player;
        }
    }
}
