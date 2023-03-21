using System;
using System.Collections;
using UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public sealed class FloatingButton : MonoBehaviour
    {
        //TODO: SafeArea対応をする必要があるか確認

        public CanvasGroup canvasGroup;
        public Button button;
        public Text text;

        [SerializeField] private float animationDuration = 0.25f;
        [SerializeField] private EaseType animationType = EaseType.ExponentialEaseOut;

        public bool Interactable
        {
            get => canvasGroup.interactable;
            set
            {
                if (IsAnimating)
                    throw new InvalidOperationException(
                        $"Cannot set {nameof(Interactable)} while {GetType()} is animating.");
                canvasGroup.interactable = value;
            }
        }

        public string Text
        {
            get => text.text;
            set => text.text = value;
        }

        public float AnimationDuration
        {
            get => animationDuration;
            set => animationDuration = value;
        }

        public EaseType AnimationType
        {
            get => animationType;
            set => animationType = value;
        }

        public bool IsAnimating { get; private set; }

        private void Awake()
        {
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }

        public event Action OnClicked;

        private void OnButtonClicked()
        {
            if (IsAnimating)
                return;

            OnClicked?.Invoke();
            Hide();
        }

        public YieldInstruction Show(CompletedDelegate completed = null)
        {
            return StartCoroutine(ShowRoutine(completed));
        }

        public YieldInstruction Hide(CompletedDelegate completed = null)
        {
            return StartCoroutine(HideRoutine(completed));
        }

        private IEnumerator ShowRoutine(CompletedDelegate completed = null)
        {
            if (IsAnimating)
                throw new InvalidOperationException($"{GetType().Name} is already animating.");

            var tween = new FloatTween
            {
                Duration = animationDuration,
                Interpolator = new EasingInterpolator
                {
                    EaseType = animationType
                },
                From = 0.0f,
                To = 1.0f
            };

            IsAnimating = true;
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = true;
            yield return tween.CreateRoutine(x => canvasGroup.alpha = x, completed);
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
            IsAnimating = false;
        }

        private IEnumerator HideRoutine(CompletedDelegate completed = null)
        {
            if (IsAnimating)
                throw new InvalidOperationException($"{GetType().Name} is already animating.");

            var tween = new FloatTween
            {
                Duration = animationDuration,
                Interpolator = new EasingInterpolator
                {
                    EaseType = animationType
                },
                From = 1.0f,
                To = 0.0f
            };

            IsAnimating = true;
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
            yield return tween.CreateRoutine(x => canvasGroup.alpha = x, completed);
            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            IsAnimating = false;
        }
    }
}
