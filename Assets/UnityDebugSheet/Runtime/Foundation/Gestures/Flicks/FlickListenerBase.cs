using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.Flicks
{
    public abstract class FlickListenerBase : MonoBehaviour
    {
        private const int MaxListSize = 3;
        private const float FlickDistanceThresholdInchPerSec = 0.025f;

        private readonly List<(Vector2 screenPosition, float deltaTime)> _positions =
            new List<(Vector2 screenPosition, float deltaTime)>(MaxListSize + 1);

        private Vector2 _touchStartScreenPosition = Vector2.zero;

        protected float Dpi { get; private set; }

        protected virtual void Start()
        {
            var dpi = Screen.dpi;
            if (dpi == 0)
                dpi = 326;
            Dpi = dpi;
        }

        protected virtual void Update()
        {
            if (ClickStarted())
            {
                Begin();
                return;
            }

            if (ClickFinished())
            {
                End();
                return;
            }

            if (TryGetClickedPosition(out var position))
                Move(position);
        }

        protected abstract bool ClickStarted();

        protected abstract bool ClickFinished();

        protected abstract bool TryGetClickedPosition(out Vector2 position);

        private void Begin()
        {
            _positions.Clear();
            _touchStartScreenPosition = Vector2.zero;
        }

        private void Move(Vector2 screenPosition)
        {
            if (_positions.Count == 0)
            {
                _touchStartScreenPosition = screenPosition;
            }
            _positions.Add((screenPosition, Time.unscaledDeltaTime));
            if (_positions.Count > MaxListSize)
                _positions.RemoveAt(0);

            Assert.IsTrue(_positions.Count <= MaxListSize);
        }

        private void End()
        {
            if (_positions.Count <= 1)
                return;

            var startScreenPos = _positions[0].screenPosition;
            var endScreenPos = _positions[_positions.Count - 1].screenPosition;

            // Calculate the delta inch per second.
            var totalTime = 0f;
            for (var i = 0; i < _positions.Count; i++)
                totalTime += _positions[i].deltaTime;
            var deltaPosInch = (endScreenPos - startScreenPos) / Dpi;
            var deltaInchPerSec = deltaPosInch / totalTime;

            // If the delta inch per second is greater than the threshold, it's a flick.
            if (deltaInchPerSec.magnitude < FlickDistanceThresholdInchPerSec)
                return;
            var flick = new Flick(_touchStartScreenPosition, startScreenPos, endScreenPos, deltaPosInch);
            Flicked(flick);
        }

        protected abstract void Flicked(Flick flick);
    }
}
