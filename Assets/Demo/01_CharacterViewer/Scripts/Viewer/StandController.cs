#if !EXCLUDE_UNITY_DEBUG_SHEET
using UnityEngine;

namespace Demo._01_CharacterViewer.Scripts.Viewer
{
    public sealed class StandController : MonoBehaviour
    {
        public const float RotationMin = 0.0f;
        public const float RotationMax = 1.0f;
        public const float AutoRotationSpeedMin = -1.0f;
        public const float AutoRotationSpeedMax = 1.0f;
        public const float PositionXMin = -5.0f;
        public const float PositionXMax = 5.0f;

        [SerializeField] [Range(RotationMin, RotationMax)]
        private float _rotation;

        [SerializeField] private bool _autoRotation;

        [SerializeField] [Range(AutoRotationSpeedMin, AutoRotationSpeedMax)]
        private float _autoRotationSpeed = 0.2f;

        [SerializeField] [Range(PositionXMin, PositionXMax)]
        private float _positionX;

        private Vector3 _initialPosition;
        private Quaternion _initialRotation;

        public float Rotation
        {
            get => _rotation;
            set => _rotation = Mathf.Repeat(value, RotationMax);
        }

        public bool AutoRotation
        {
            get => _autoRotation;
            set => _autoRotation = value;
        }

        public float AutoRotationSpeed
        {
            get => _autoRotationSpeed;
            set => _autoRotationSpeed = value;
        }

        public float PositionX
        {
            get => _positionX;
            set => _positionX = Mathf.Clamp(value, PositionXMin, PositionXMax);
        }

        private void Awake()
        {
            var trans = transform;
            _initialRotation = trans.rotation;
            _initialPosition = trans.position;
        }

        private void Update()
        {
            var trans = transform;

            // Apply auto rotation.
            if (AutoRotation)
                Rotation += Time.deltaTime * AutoRotationSpeed;

            // Update rotation.
            trans.rotation = _initialRotation * Quaternion.Euler(0.0f, Rotation * 360.0f, 0.0f);

            // Update positions.
            trans.position = _initialPosition + new Vector3(-PositionX, 0.0f, 0.0f);
        }
    }
}
#endif
