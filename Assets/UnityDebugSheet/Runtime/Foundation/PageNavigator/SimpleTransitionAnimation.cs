using UnityDebugSheet.Runtime.Foundation.PageNavigator.Modules;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.PageNavigator
{
    public sealed class SimpleTransitionAnimation : ITransitionAnimation
    {
        private float _delay;
        private float _duration = 0.3f;
        private EaseType _easeType = EaseType.QuarticEaseOut;
        private PageAlignment _beforeAlignment = PageAlignment.Center;
        private Vector3 _beforeScale = Vector3.one;
        private float _beforeAlpha = 1.0f;
        private PageAlignment _afterAlignment = PageAlignment.Center;
        private Vector3 _afterScale = Vector3.one;
        private float _afterAlpha = 1.0f;

        private Vector3 _afterPosition;
        private Vector3 _beforePosition;
        private CanvasGroup _canvasGroup;

        public RectTransform RectTransform { get; private set; }
        public RectTransform PartnerRectTransform { get; private set; }

        void ITransitionAnimation.SetPartner(RectTransform partnerRectTransform)
        {
            PartnerRectTransform = partnerRectTransform;
        }

        void ITransitionAnimation.Setup(RectTransform rectTransform)
        {
            RectTransform = rectTransform;
            Setup();
            SetTime(0.0f);
        }

        public float Duration => _duration;

        public static SimpleTransitionAnimation CreateInstance(float? duration = null, EaseType? easeType = null,
            PageAlignment? beforeAlignment = null, Vector3? beforeScale = null, float? beforeAlpha = null,
            PageAlignment? afterAlignment = null, Vector3? afterScale = null, float? afterAlpha = null)
        {
            var anim = new SimpleTransitionAnimation();
            anim.SetParams(duration, easeType, beforeAlignment, beforeScale, beforeAlpha, afterAlignment, afterScale,
                afterAlpha);
            return anim;
        }

        public void Setup()
        {
            _beforePosition = _beforeAlignment.ToPosition(RectTransform);
            _afterPosition = _afterAlignment.ToPosition(RectTransform);
            if (!RectTransform.gameObject.TryGetComponent<CanvasGroup>(out var canvasGroup))
            {
                canvasGroup = RectTransform.gameObject.AddComponent<CanvasGroup>();
            }

            _canvasGroup = canvasGroup;
        }

        public void SetTime(float time)
        {
            time = Mathf.Max(0, time - _delay);
            var progress = _duration <= 0.0f ? 1.0f : Mathf.Clamp01(time / _duration);
            progress = Easings.Interpolate(progress, _easeType);
            var position = Vector3.Lerp(_beforePosition, _afterPosition, progress);
            var scale = Vector3.Lerp(_beforeScale, _afterScale, progress);
            var alpha = Mathf.Lerp(_beforeAlpha, _afterAlpha, progress);
            RectTransform.anchoredPosition = position;
            RectTransform.localScale = scale;
            _canvasGroup.alpha = alpha;
        }

        public void SetParams(float? duration = null, EaseType? easeType = null, PageAlignment? beforeAlignment = null,
            Vector3? beforeScale = null, float? beforeAlpha = null, PageAlignment? afterAlignment = null,
            Vector3? afterScale = null, float? afterAlpha = null)
        {
            if (duration.HasValue)
            {
                _duration = duration.Value;
            }

            if (easeType.HasValue)
            {
                _easeType = easeType.Value;
            }

            if (beforeAlignment.HasValue)
            {
                _beforeAlignment = beforeAlignment.Value;
            }

            if (beforeScale.HasValue)
            {
                _beforeScale = beforeScale.Value;
            }

            if (beforeAlpha.HasValue)
            {
                _beforeAlpha = beforeAlpha.Value;
            }

            if (afterAlignment.HasValue)
            {
                _afterAlignment = afterAlignment.Value;
            }

            if (afterScale.HasValue)
            {
                _afterScale = afterScale.Value;
            }

            if (afterAlpha.HasValue)
            {
                _afterAlpha = afterAlpha.Value;
            }
        }
    }
}
