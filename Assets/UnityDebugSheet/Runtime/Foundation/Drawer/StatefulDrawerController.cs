using System;
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
        public enum DrawerResizingState
        {
            None,
            Dragging,
            Animation
        }
        
        private const int MaxPositionListSize = 3;
        private const float FlickDistanceThresholdInchPerSec = 0.025f;

        [SerializeField] private float _animationDuration = 0.25f;
        [SerializeField] private EaseType _animationType = EaseType.ExponentialEaseOut;

        private readonly List<(Vector2 screenPosition, float deltaTime)> _dragPositions =
            new List<(Vector2 screenPosition, float deltaTime)>(MaxPositionListSize + 1);

        private int _pointerId = -1;
        private Canvas _canvas;
        private StatefulDrawer _drawer;
        private DrawerResizingState _resizingState = DrawerResizingState.None;

        private float Dpi { get; set; }

        public DrawerResizingState ResizingState
        {
            get => _resizingState;
            private set
            {
                if (_resizingState == value)
                    return;

                _resizingState = value;
                OnResizingStateChanged?.Invoke(_resizingState);
            }
        }

        public event Action<DrawerResizingState> OnResizingStateChanged;
        
        private void Start()
        {
            _drawer = GetComponent<StatefulDrawer>();
            _canvas = GetComponentInParent<Canvas>();

            var dpi = Screen.dpi;
            if (dpi == 0)
                dpi = 326;
            Dpi = dpi;

            Assert.IsNotNull(_canvas);
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            // If it is already dragging with another finger , ignore it.
            if (_pointerId != -1)
                return;

            _pointerId = eventData.pointerId;
            
            _dragPositions.Clear();

            ResizingState = DrawerResizingState.Dragging;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            // If it is the event of another finger , ignore it.
            if (eventData.pointerId != _pointerId)
                return;
            
            var deltaScreenPos = eventData.delta;
            var deltaPos = deltaScreenPos / _canvas.scaleFactor;
            if (_drawer.IsInAnimation)
                _drawer.StopProgressAnimation();

            var isHorizontal = _drawer.Direction == DrawerDirection.LeftToRight
                               || _drawer.Direction == DrawerDirection.RightToLeft;
            var isInverse = _drawer.Direction == DrawerDirection.RightToLeft
                            || _drawer.Direction == DrawerDirection.TopToBottom;
            var delta = isHorizontal ? deltaPos.x : deltaPos.y;
            _drawer.Progress += _drawer.GetProgressFromDistance(delta) * (isInverse ? -1.0f : 1.0f);

            _dragPositions.Add((eventData.position, Time.unscaledDeltaTime));
            if (_dragPositions.Count > MaxPositionListSize)
                _dragPositions.RemoveAt(0);

            Assert.IsTrue(_dragPositions.Count <= MaxPositionListSize);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            // If it is the event of another finger , ignore it.
            if (eventData.pointerId != _pointerId)
                return;

            _pointerId = -1;
            
            if (_drawer.IsInAnimation)
                _drawer.StopProgressAnimation();

            if (_dragPositions.Count <= 1)
                SetStateWithAnimation(_drawer.GetNearestState());

            var startScreenPos = _dragPositions[0].screenPosition;
            var endScreenPos = _dragPositions[_dragPositions.Count - 1].screenPosition;

            // Check whether flicked or not.
            var totalTime = 0f;
            for (var i = 0; i < _dragPositions.Count; i++)
                totalTime += _dragPositions[i].deltaTime;
            var deltaPosInch = (endScreenPos - startScreenPos) / Dpi;
            var deltaInchPerSec = deltaPosInch / totalTime;
            var flicked = deltaInchPerSec.magnitude >= FlickDistanceThresholdInchPerSec;

            if (flicked)
                OnFlicked(startScreenPos, endScreenPos, deltaPosInch);
            else
                SetStateWithAnimation(_drawer.GetNearestState());
        }

        private void OnFlicked(Vector2 startScreenPosition, Vector2 endScreenPosition, Vector2 deltaInchPosition)
        {
            var horizontalFlick = Mathf.Abs(deltaInchPosition.x) > Mathf.Abs(deltaInchPosition.y);
            var positiveFlick = horizontalFlick ? deltaInchPosition.x >= 0 : deltaInchPosition.y >= 0;
            var drawerIsHorizontal = _drawer.Direction == DrawerDirection.LeftToRight
                                     || _drawer.Direction == DrawerDirection.RightToLeft;
            var drawerIsVertical = _drawer.Direction == DrawerDirection.BottomToTop
                                   || _drawer.Direction == DrawerDirection.TopToBottom;

            // If flick direction is not same as drawer direction, transition to the nearest state.
            if ((horizontalFlick && drawerIsVertical) || (!horizontalFlick && drawerIsHorizontal))
            {
                SetStateWithAnimation(_drawer.GetNearestState());
                return;
            }

            // Transition to the upper or lower state if flick direction is same as drawer direction.
            var drawerDirectionIsInversed = _drawer.Direction == DrawerDirection.RightToLeft
                                            || _drawer.Direction == DrawerDirection.TopToBottom;
            
            var positiveTransition = drawerDirectionIsInversed ? !positiveFlick : positiveFlick;
            var targetState = positiveTransition ? _drawer.GetUpperState() : _drawer.GetLowerState();
            SetStateWithAnimation(targetState);
        }

        public void SetStateWithAnimation(DrawerState state)
        {
            if (_drawer.IsInAnimation)
                return;

            ResizingState = DrawerResizingState.Animation;
            _drawer.SetStateWithAnimation(state, _animationDuration, _animationType,
                () => ResizingState = DrawerResizingState.None);
        }
    }
}
