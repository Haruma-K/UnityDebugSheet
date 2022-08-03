using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.Flicks
{
    public readonly struct Flick
    {
        public Flick(Vector2 startScreenPosition, Vector2 endScreenPosition, Vector2 deltaInchPosition)
        {
            StartScreenPosition = startScreenPosition;
            EndScreenPosition = endScreenPosition;
            DeltaInchPosition = deltaInchPosition;
        }

        public Vector2 StartScreenPosition { get; }
        public Vector2 EndScreenPosition { get; }
        public Vector2 DeltaInchPosition { get; }
    }
}
