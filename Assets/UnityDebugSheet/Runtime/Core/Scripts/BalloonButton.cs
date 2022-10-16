using System;
using System.Collections;
using UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public class BalloonButton : MonoBehaviour
    {
        [SerializeField] private RectTransform _balloonRectTrans;
        [SerializeField] private Button _balloonButton;
        [SerializeField] private Button _backdropButton;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _animationDuration = 0.25f;
        [SerializeField] private EaseType _animationType = EaseType.ExponentialEaseOut;
        [SerializeField] private Text _balloonText;

        private Canvas _canvas;
        private Action _clicked;
        private bool _isInitialized;
        private RectTransform _rectTrans;

        public bool IsAnimating { get; private set; }

        private void Awake()
        {
            _canvasGroup.alpha = 0.0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        private void OnEnable()
        {
            _balloonButton.onClick.AddListener(OnBalloonButtonClicked);
            _backdropButton.onClick.AddListener(OnBackdropButtonClicked);
        }

        private void OnDisable()
        {
            _balloonButton.onClick.RemoveListener(OnBalloonButtonClicked);
            _backdropButton.onClick.RemoveListener(OnBackdropButtonClicked);
        }

        public void Initialize(Canvas canvas)
        {
            _rectTrans = (RectTransform)transform;
            _canvas = canvas;
            _isInitialized = true;
        }

        private void OnBalloonButtonClicked()
        {
            if (IsAnimating)
                return;

            _clicked?.Invoke();
            Hide();
        }

        private void OnBackdropButtonClicked()
        {
            if (IsAnimating)
                return;

            Hide();
        }

        public void Show(RectTransform targetRectTrans, string text, Action clicked)
        {
            Assert.IsTrue(_isInitialized);

            var worldCorners = new Vector3[4];
            targetRectTrans.GetWorldCorners(worldCorners);
            var worldPos = worldCorners[1] + (worldCorners[2] - worldCorners[1]) * 0.5f;
            var canvasCamera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
            var screenPoint = RectTransformUtility.WorldToScreenPoint(canvasCamera, worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTrans, screenPoint, canvasCamera,
                out var localPosition);
            _balloonRectTrans.anchoredPosition = localPosition;

            _balloonText.text = text;
            _clicked = clicked;
            StartCoroutine(ShowRoutine());
        }

        public void Hide()
        {
            _clicked = null;
            StartCoroutine(HideRoutine());
        }

        private IEnumerator ShowRoutine(CompletedDelegate completed = null)
        {
            if (IsAnimating)
                throw new InvalidOperationException($"{GetType().Name} is already animating.");

            var tween = new FloatTween
            {
                Duration = _animationDuration,
                Interpolator = new EasingInterpolator
                {
                    EaseType = _animationType
                },
                From = 0.0f,
                To = 1.0f
            };

            IsAnimating = true;
            _canvasGroup.alpha = 0.0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = true;
            yield return tween.CreateRoutine(x => _canvasGroup.alpha = x, completed);
            _canvasGroup.alpha = 1.0f;
            _canvasGroup.interactable = true;
            IsAnimating = false;
        }

        private IEnumerator HideRoutine(CompletedDelegate completed = null)
        {
            if (IsAnimating)
                throw new InvalidOperationException($"{GetType().Name} is already animating.");

            var tween = new FloatTween
            {
                Duration = _animationDuration,
                Interpolator = new EasingInterpolator
                {
                    EaseType = _animationType
                },
                From = 1.0f,
                To = 0.0f
            };

            IsAnimating = true;
            _canvasGroup.alpha = 1.0f;
            _canvasGroup.interactable = true;
            yield return tween.CreateRoutine(x => _canvasGroup.alpha = x, completed);
            _canvasGroup.alpha = 0.0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            IsAnimating = false;
        }
    }
}
