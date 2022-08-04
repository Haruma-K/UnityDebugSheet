using UnityEngine;
using UnityEngine.EventSystems;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.Flicks
{
    public abstract class EventSystemBasedFlickListenerBase : FlickListenerBase, IBeginDragHandler, IDragHandler,
        IEndDragHandler
    {
        private bool _dragFinished;
        private Vector2? _dragPosition;
        private bool _dragStarted;

        protected void LateUpdate()
        {
            _dragStarted = false;
            _dragFinished = false;
            _dragPosition = null;
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _dragStarted = true;
            _dragPosition = eventData.position;
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _dragPosition = eventData.position;
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            _dragFinished = true;
            _dragPosition = eventData.position;
        }

        protected override bool ClickStarted()
        {
            return _dragStarted;
        }

        protected override bool ClickFinished()
        {
            return _dragFinished;
        }

        protected override bool TryGetClickedPosition(out Vector2 position)
        {
            if (_dragPosition.HasValue)
            {
                position = _dragPosition.Value;
                return true;
            }

            position = Vector2.zero;
            return false;
        }
    }
}
