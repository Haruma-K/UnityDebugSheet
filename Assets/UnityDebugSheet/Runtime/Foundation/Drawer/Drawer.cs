using System;
using UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Drawer
{
    [ExecuteAlways]
    public class Drawer : MonoBehaviour
    {
        [SerializeField] private DrawerDirection _direction;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        [Tooltip("Size of the drawer. When 1, it is displayed on the full screen.")]
        private float _size = 0.5f;

        [SerializeField]
        [Tooltip("If true, the drawer will be located at the edge of the Safe Area when progress is zero. " +
                 "Otherwise, it will be located at the edge of the screen.")]
        private bool _moveInsideSafeArea;

        [SerializeField] private bool _openOnStart;

        protected Canvas Canvas;
        private bool _isProgressDirty;
        private bool _isTransformDirty;
        private Vector2 _lastFullSize;
        private float _progress = 1.0f;
        private Coroutine _progressCoroutine;

        public DrawerDirection Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                _isTransformDirty = true;
            }
        }

        /// <summary>
        ///     Size of the drawer. When 1, it is displayed on the full screen.
        /// </summary>
        public float Size
        {
            get => _size;
            set
            {
                _size = value;
                _isTransformDirty = true;
            }
        }

        public bool MoveInsideSafeArea
        {
            get => _moveInsideSafeArea;
            set
            {
                _moveInsideSafeArea = value;
                _isTransformDirty = true;
            }
        }

        public bool OpenOnStart
        {
            get => _openOnStart;
            set => _openOnStart = value;
        }

        public float Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                _isProgressDirty = true;
            }
        }

        public bool IsInAnimation => _progressCoroutine != null;

        protected virtual void Start()
        {
            if (Application.isPlaying)
                Progress = _openOnStart ? 1.0f : 0.0f;
        }

        protected virtual void Update()
        {
            if (!Application.isPlaying)
            {
                Canvas = GetComponentInParent<Canvas>();
            }
            else
            {
                if (Canvas == null)
                    Canvas = GetComponentInParent<Canvas>();
            }

            var scaleFactor = Canvas.scaleFactor;
            var fullSize = new Vector2(Screen.width, Screen.height) / scaleFactor;
            var safeArea = Screen.safeArea;
            safeArea.position /= scaleFactor;
            safeArea.size /= scaleFactor;

            // Update Transform because the screen size or canvas scale has been changed.
            if (_lastFullSize != fullSize)
            {
                _isTransformDirty = true;
                _lastFullSize = fullSize;
            }

            if (!Application.isPlaying)
            {
                // In EditMode, update in every frame.
                UpdateTransform(fullSize, safeArea);
                UpdateProgress(fullSize, safeArea);
            }
            else
            {
                // In PlayMode, update as needed.
                if (_isTransformDirty)
                {
                    UpdateTransform(fullSize, safeArea);
                    UpdateProgress(fullSize, safeArea);
                    ProgressUpdated?.Invoke(Progress);
                    _isTransformDirty = false;
                    _isProgressDirty = false;
                }

                if (_isProgressDirty)
                {
                    UpdateProgress(fullSize, safeArea);
                    ProgressUpdated?.Invoke(Progress);
                    _isProgressDirty = false;
                }
            }
        }

        protected virtual void OnEnable()
        {
            _isTransformDirty = true;
        }

        public event Action<float> ProgressUpdated;

        public YieldInstruction PlayProgressAnimation(float toProgress, float durationSec, EaseType easeType,
            GetDeltaTimeDelegate getDeltaTime = null, Action completed = null)
        {
            return PlayProgressAnimation(Progress, toProgress, durationSec, easeType, getDeltaTime, completed);
        }

        public YieldInstruction PlayProgressAnimation(float fromProgress, float toProgress, float durationSec,
            EaseType easeType, GetDeltaTimeDelegate getDeltaTime = null, Action completed = null)
        {
            return PlayProgressAnimation(fromProgress, toProgress, durationSec,
                new EasingInterpolator { EaseType = easeType }, getDeltaTime, completed);
        }

        public YieldInstruction PlayProgressAnimation(float toProgress, float durationSec,
            IInterpolator interpolator, GetDeltaTimeDelegate getDeltaTime = null, Action completed = null)
        {
            return PlayProgressAnimation(Progress, toProgress, durationSec, interpolator, getDeltaTime, completed);
        }

        public YieldInstruction PlayProgressAnimation(float fromProgress, float toProgress, float durationSec,
            IInterpolator interpolator, GetDeltaTimeDelegate getDeltaTime = null, Action completed = null)
        {
            if (_progressCoroutine != null)
                throw new Exception(
                    "Progress Animation is now playing. If you want to play new animation, call StopProgressAnimation first.");

            void Completed()
            {
                _progressCoroutine = null;
                completed?.Invoke();
            }

            void ValueChanged(float value)
            {
                Progress = value;
            }

            var routine = TweenRoutineFactory.Create(fromProgress, toProgress, durationSec, ValueChanged, Mathf.Lerp,
                Completed, interpolator, getDeltaTime);
            _progressCoroutine = StartCoroutine(routine);
            return _progressCoroutine;
        }

        public void StopProgressAnimation()
        {
            if (_progressCoroutine != null)
            {
                StopCoroutine(_progressCoroutine);
                _progressCoroutine = null;
            }
        }

        public float GetProgressFromDistance(float distance)
        {
            var scaleFactor = Canvas.scaleFactor;
            var fullSize = new Vector2(Screen.width, Screen.height) / scaleFactor;
            var safeArea = Screen.safeArea;
            safeArea.position /= scaleFactor;
            safeArea.size /= scaleFactor;
            var minPos = GetStartPos(fullSize, safeArea, _direction, _moveInsideSafeArea);
            var maxPos = GetEndPos(fullSize, safeArea, _direction);
            var length = Mathf.Abs(maxPos - minPos);
            return distance / length;
        }

        private void UpdateTransform(Vector2 fullSize, Rect safeArea)
        {
            var rectTransform = (RectTransform)transform;
            rectTransform.anchorMin = GetAnchorMin(_direction, rectTransform.anchorMin);
            rectTransform.anchorMax = GetAnchorMax(_direction, rectTransform.anchorMax);
            rectTransform.pivot = GetPivot(_direction, rectTransform.pivot);
            rectTransform.sizeDelta = GetSizeDelta(rectTransform.sizeDelta, fullSize, safeArea, _size,
                _moveInsideSafeArea, _direction);
        }

        private void UpdateProgress(Vector2 fullSize, Rect safeArea)
        {
            var rectTransform = (RectTransform)transform;
            var minPos = GetStartPos(fullSize, safeArea, _direction, _moveInsideSafeArea);
            var maxPos = GetEndPos(fullSize, safeArea, _direction);
            var normalizedSize = Mathf.Lerp(0.0f, _size, _progress);
            var anchoredPosition = rectTransform.anchoredPosition;
            if (_direction == DrawerDirection.LeftToRight || _direction == DrawerDirection.RightToLeft)
                anchoredPosition.x = Mathf.Lerp(minPos, maxPos, normalizedSize);
            else
                anchoredPosition.y = Mathf.Lerp(minPos, maxPos, normalizedSize);
            rectTransform.anchoredPosition = anchoredPosition;
        }

        private static Vector2 GetAnchorMin(DrawerDirection direction, Vector2 source)
        {
            switch (direction)
            {
                case DrawerDirection.LeftToRight:
                    source.x = 0.0f;
                    break;
                case DrawerDirection.RightToLeft:
                    source.x = 1.0f;
                    break;
                case DrawerDirection.BottomToTop:
                    source.y = 0.0f;
                    break;
                case DrawerDirection.TopToBottom:
                    source.y = 1.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return source;
        }

        private static Vector2 GetAnchorMax(DrawerDirection direction, Vector2 source)
        {
            switch (direction)
            {
                case DrawerDirection.LeftToRight:
                    source.x = 0.0f;
                    break;
                case DrawerDirection.RightToLeft:
                    source.x = 1.0f;
                    break;
                case DrawerDirection.BottomToTop:
                    source.y = 0.0f;
                    break;
                case DrawerDirection.TopToBottom:
                    source.y = 1.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return source;
        }

        private static Vector2 GetPivot(DrawerDirection direction, Vector2 source)
        {
            switch (direction)
            {
                case DrawerDirection.LeftToRight:
                    source.x = 1.0f;
                    break;
                case DrawerDirection.RightToLeft:
                    source.x = 0.0f;
                    break;
                case DrawerDirection.BottomToTop:
                    source.y = 1.0f;
                    break;
                case DrawerDirection.TopToBottom:
                    source.y = 0.0f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            return source;
        }

        private static Vector2 GetSizeDelta(Vector2 source, Vector2 fullAreaSize, Rect safeArea, float drawerScale,
            bool moveInsideSafeArea, DrawerDirection direction)
        {
            float maxSize;
            float minSize;
            switch (direction)
            {
                case DrawerDirection.LeftToRight:
                    minSize = moveInsideSafeArea ? safeArea.xMin : 0.0f;
                    maxSize = safeArea.xMax;
                    break;
                case DrawerDirection.RightToLeft:
                    minSize = moveInsideSafeArea ? fullAreaSize.x - safeArea.xMax : 0.0f;
                    maxSize = fullAreaSize.x - safeArea.xMin;
                    break;
                case DrawerDirection.BottomToTop:
                    minSize = moveInsideSafeArea ? safeArea.yMin : 0.0f;
                    maxSize = safeArea.yMax;
                    break;
                case DrawerDirection.TopToBottom:
                    minSize = moveInsideSafeArea ? fullAreaSize.y - safeArea.yMax : 0.0f;
                    maxSize = fullAreaSize.y - safeArea.yMin;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            var value = Mathf.Lerp(minSize, maxSize, drawerScale);
            if (direction == DrawerDirection.LeftToRight || direction == DrawerDirection.RightToLeft)
                source.x = value;
            else
                source.y = value;

            return source;
        }

        private static float GetStartPos(Vector2 fullSize, Rect safeArea, DrawerDirection direction,
            bool insideSafeArea)
        {
            switch (direction)
            {
                case DrawerDirection.LeftToRight:
                    return insideSafeArea ? safeArea.xMin : 0.0f;
                case DrawerDirection.RightToLeft:
                    return insideSafeArea ? (fullSize.x - safeArea.width - safeArea.xMin) * -1.0f : 0.0f;
                case DrawerDirection.BottomToTop:
                    return insideSafeArea ? safeArea.yMin : 0.0f;
                case DrawerDirection.TopToBottom:
                    return insideSafeArea ? (fullSize.y - safeArea.height - safeArea.yMin) * -1.0f : 0.0f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private static float GetEndPos(Vector2 fullSize, Rect localSafeArea, DrawerDirection direction)
        {
            switch (direction)
            {
                case DrawerDirection.LeftToRight:
                    return localSafeArea.xMax;
                case DrawerDirection.RightToLeft:
                    return (fullSize.x - localSafeArea.xMin) * -1.0f;
                case DrawerDirection.BottomToTop:
                    return localSafeArea.yMax;
                case DrawerDirection.TopToBottom:
                    return (fullSize.y - localSafeArea.yMin) * -1.0f;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}
