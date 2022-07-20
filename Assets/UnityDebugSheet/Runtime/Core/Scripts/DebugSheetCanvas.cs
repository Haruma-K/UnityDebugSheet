using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    [ExecuteAlways]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [DefaultExecutionOrder(-10)]
    internal sealed class DebugSheetCanvas : MonoBehaviour
    {
        [SerializeField] private CanvasScaler _canvasScaler;
        private bool _isPortrait;

        private void Awake()
        {
#if UNITY_EDITOR
            _canvasScaler = GetComponent<CanvasScaler>();
#endif
            Apply(true);
        }

        private void Update()
        {
            Apply();
        }

        private void Apply(bool force = false)
        {
            var isPortrait = Screen.height >= Screen.width;
            if (!force && _isPortrait == isPortrait)
            {
                return;
            }

            var referenceResolution = isPortrait ? new Vector2(750, 1334) : new Vector2(1334, 750);
            _canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _canvasScaler.referenceResolution = referenceResolution;
            _canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _canvasScaler.matchWidthOrHeight = isPortrait ? 0.0f : 1.0f;

            _isPortrait = isPortrait;
        }
    }
}
