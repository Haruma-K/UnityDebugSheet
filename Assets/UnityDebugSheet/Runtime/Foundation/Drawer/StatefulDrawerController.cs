using System.Collections.Generic;
using UnityDebugSheet.Runtime.Foundation.Drawer.TinyTween;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace UnityDebugSheet.Runtime.Foundation.Drawer
{
    [RequireComponent(typeof(StatefulDrawer))]
    public sealed class StatefulDrawerController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private const float FlickTimeThreshold = 0.1f;
        private const float FlickDeltaThreshold = 10.0f;
        [SerializeField] private float _animationDuration = 0.25f;
        [SerializeField] private EaseType _animationType = EaseType.ExponentialEaseOut;
        private readonly Queue<DragData> _dragDataList = new Queue<DragData>(100);

        private Canvas _canvas;
        private StatefulDrawer _drawer;

        private void Start()
        {
            _drawer = GetComponent<StatefulDrawer>();
            _canvas = GetComponentInParent<Canvas>();

            Assert.IsNotNull(_canvas);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragDataList.Clear();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_drawer.IsInAnimation)
                _drawer.StopProgressAnimation();

            var deltaPos = eventData.delta / _canvas.scaleFactor;
            var isHorizontal = _drawer.Direction == DrawerDirection.LeftToRight
                               || _drawer.Direction == DrawerDirection.RightToLeft;
            var isInverse = _drawer.Direction == DrawerDirection.RightToLeft
                            || _drawer.Direction == DrawerDirection.TopToBottom;
            var delta = isHorizontal ? deltaPos.x : deltaPos.y;
            _drawer.Progress += _drawer.GetProgressFromDistance(delta) * (isInverse ? -1.0f : 1.0f);
            _dragDataList.Enqueue(new DragData { Time = Time.time, Delta = delta });
            while (_dragDataList.Count > 100)
                _dragDataList.Dequeue();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_drawer.IsInAnimation)
                _drawer.StopProgressAnimation();

            var delta = 0.0f;
            while (true)
            {
                if (_dragDataList.Count == 0)
                    break;

                var dragData = _dragDataList.Dequeue();

                if (Time.time - dragData.Time >= FlickTimeThreshold)
                    break;

                delta += dragData.Delta;
            }

            var isInverse = _drawer.Direction == DrawerDirection.RightToLeft
                            || _drawer.Direction == DrawerDirection.TopToBottom;
            delta *= isInverse ? -1.0f : 1.0f;

            if (delta > FlickDeltaThreshold)
                SetStateWithAnimation(_drawer.GetUpperState());
            else if (delta < -FlickDeltaThreshold)
                SetStateWithAnimation(_drawer.GetLowerState());
            else
                SetStateWithAnimation(_drawer.GetNearestState());
        }

        public void SetStateWithAnimation(DrawerState state)
        {
            _drawer.SetStateWithAnimation(state, _animationDuration, _animationType);
        }

        private struct DragData
        {
            public float Time { get; set; }
            public float Delta { get; set; }
        }
    }
}
