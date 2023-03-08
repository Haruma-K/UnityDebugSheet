#if !EXCLUDE_UNITY_DEBUG_SHEET
using System.Collections;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine;

namespace Demo._99_Shared.Scripts
{
    public sealed class FlickHere : MonoBehaviour
    {
        [SerializeField] private float _blinkIntervalSec = 0.75f;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _leftGUI;
        [SerializeField] private RectTransform _rightGUI;
        private Coroutine _blinkCoroutine;

        private void Start()
        {
            var debugSheet = DebugSheet.Instance;
            if (debugSheet != null)
            {
                var flickToOpen = debugSheet.FlickToOpen;
                _leftGUI.gameObject.SetActive(flickToOpen == FlickToOpenMode.Edge
                                              || flickToOpen == FlickToOpenMode.LeftEdge);
                _rightGUI.gameObject.SetActive(flickToOpen == FlickToOpenMode.Edge
                                               || flickToOpen == FlickToOpenMode.RightEdge);
            }

            var canvas = GetComponentInParent<Canvas>();
            var scaleFactor = canvas.scaleFactor;
            var leftWidth = _leftGUI.sizeDelta.x;
            leftWidth += Screen.safeArea.x / scaleFactor;
            _leftGUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, leftWidth);
            var rightWidth = _rightGUI.sizeDelta.x;
            rightWidth += (Screen.width - Screen.safeArea.xMax) / scaleFactor;
            _rightGUI.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rightWidth);
        }

        private void OnEnable()
        {
            _blinkCoroutine = StartCoroutine(BlinkRoutine());
        }

        private void OnDisable()
        {
            StopCoroutine(_blinkCoroutine);
        }

        private IEnumerator BlinkRoutine()
        {
            while (true)
            {
                yield return CrossFadeAlpha(0.5f, _blinkIntervalSec, false);
                yield return CrossFadeAlpha(1.0f, _blinkIntervalSec, false);
            }
        }

        private IEnumerator CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
        {
            var time = 0.0f;
            var startAlpha = _canvasGroup.alpha;
            while (time < duration)
            {
                time += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                _canvasGroup.alpha = Mathf.Lerp(startAlpha, alpha, time / duration);
                yield return null;
            }

            _canvasGroup.alpha = alpha;
        }
    }
}
#endif
