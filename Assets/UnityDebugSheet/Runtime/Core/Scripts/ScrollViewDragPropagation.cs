using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    [RequireComponent(typeof(ScrollView))]
    internal sealed class ScrollViewDragPropagation : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private bool _beginFromEdge;
        private IBeginDragHandler _parentBeginDragHandler;
        private IDragHandler _parentDragHandler;
        private IEndDragHandler _parentEndDragHandler;
        private ScrollRect _scrollRect;

        private void Start()
        {
            var parent = transform.parent;
            _scrollRect = GetComponent<ScrollRect>();
            _parentBeginDragHandler = parent.GetComponentInParent<IBeginDragHandler>();
            _parentDragHandler = parent.GetComponentInParent<IDragHandler>();
            _parentEndDragHandler = parent.GetComponentInParent<IEndDragHandler>();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            if (_scrollRect.verticalNormalizedPosition >= 1.0f && eventData.delta.y < 0)
            {
                _beginFromEdge = true;
                _parentBeginDragHandler?.OnBeginDrag(eventData);
            }

            if (_scrollRect.verticalNormalizedPosition <= 0.0f && eventData.delta.y > 0)
            {
                _beginFromEdge = true;
                _parentBeginDragHandler?.OnBeginDrag(eventData);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (!_beginFromEdge)
            {
                return;
            }

            _parentDragHandler?.OnDrag(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (!_beginFromEdge)
            {
                return;
            }

            _beginFromEdge = false;
            _parentEndDragHandler?.OnEndDrag(eventData);
        }
    }
}
