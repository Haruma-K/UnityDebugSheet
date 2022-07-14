using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween
{
    public sealed class TweenPlayer<T>
    {
        private readonly Tween<T> _tween;

        public TweenPlayer(Tween<T> tween)
        {
            _tween = tween;
        }

        public float Time { get; private set; }
        public bool IsCompleted => Time >= _tween.Duration;

        public bool IsPlaying { get; set; }

        public event ValueChangedDelegate<T> ValueChanged;

        public event CompletedDelegate Completed;

        public void Play()
        {
            IsPlaying = true;
            Evaluate();
        }

        public void Stop()
        {
            IsPlaying = false;
        }

        public void Reset()
        {
            SetTime(0.0f);
        }

        public void Update(float deltaTime)
        {
            SetTime(Time + deltaTime);
        }

        private void SetTime(float time)
        {
            if (!IsPlaying)
                return;

            if (IsCompleted)
                return;

            time = Mathf.Max(0.0f, Mathf.Min(_tween.Duration, time));
            Time = time;
            Evaluate();
        }

        public void Evaluate()
        {
            var progress = Mathf.Min(1.0f, Time / _tween.Duration);
            if (_tween.Interpolator != null)
                progress = _tween.Interpolator.Interpolate(progress);
            var value = _tween.Lerp(_tween.From, _tween.To, progress);
            ValueChanged?.Invoke(value);

            if (IsCompleted)
                Completed?.Invoke();
        }
    }
}
