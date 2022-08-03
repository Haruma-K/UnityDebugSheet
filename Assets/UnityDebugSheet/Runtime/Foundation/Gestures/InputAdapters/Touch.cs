using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.InputAdapters
{
    public readonly struct Touch
    {
        public int Id { get; }
        public Vector2 Position { get; }
        public float Pressure { get; }
        public TouchPhase TouchPhase { get; }
        
        public Touch(int id, Vector2 position, float pressure, TouchPhase touchPhase)
        {
            Id = id;
            Position = position;
            Pressure = pressure;
            TouchPhase = touchPhase;
        }
    }
}
