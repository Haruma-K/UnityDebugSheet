using System;
using UnityEngine;

namespace Demo._99_Shared.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public sealed class SafeAreaRect : MonoBehaviour
    {
        [Flags]
        public enum Edge
        {
            Left = 1 << 0,
            Right = 1 << 1,
            Top = 1 << 2,
            Bottom = 1 << 3
        }

        [SerializeField] private Edge _controlEdges = (Edge)~0;

#if UNITY_EDITOR
        private DrivenRectTransformTracker drivenRectTransformTracker;
#endif
        private Edge lastControlEgdes;
        private Vector2Int lastResolution;
        private Rect lastSafeArea;
        public Edge ControlEdges => _controlEdges;

        private void Update()
        {
            Apply();
        }

        private void OnEnable()
        {
            Apply(true);
        }

#if UNITY_EDITOR
        private void OnDisable()
        {
            drivenRectTransformTracker.Clear();
        }
#endif

        public void Apply(bool force = false)
        {
            var rectTransform = (RectTransform)transform;
            var safeArea = Screen.safeArea;
            var resolution = new Vector2Int(Screen.width, Screen.height);
            if (resolution.x == 0 || resolution.y == 0) return;
            if (!force)
            {
                if (rectTransform.anchorMax == Vector2.zero)
                {
                    // Do apply.
                    // ※Undoすると0になるので再適用させる
                }
                else if (lastSafeArea == safeArea && lastResolution == resolution && lastControlEgdes == _controlEdges)
                {
                    return;
                }
            }

            lastSafeArea = safeArea;
            lastResolution = resolution;
            lastControlEgdes = _controlEdges;

#if UNITY_EDITOR
            drivenRectTransformTracker.Clear();
            drivenRectTransformTracker.Add(
                this,
                rectTransform,
                DrivenTransformProperties.AnchoredPosition
                | DrivenTransformProperties.SizeDelta
                | DrivenTransformProperties.AnchorMin
                | DrivenTransformProperties.AnchorMax
            );
#endif

            var normalizedMin = new Vector2(safeArea.xMin / resolution.x, safeArea.yMin / resolution.y);
            var normalizedMax = new Vector2(safeArea.xMax / resolution.x, safeArea.yMax / resolution.y);
            if ((_controlEdges & Edge.Left) == 0) normalizedMin.x = 0;
            if ((_controlEdges & Edge.Right) == 0) normalizedMax.x = 1;
            if ((_controlEdges & Edge.Top) == 0) normalizedMax.y = 1;
            if ((_controlEdges & Edge.Bottom) == 0) normalizedMin.y = 0;

            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.anchorMin = normalizedMin;
            rectTransform.anchorMax = normalizedMax;
        }
    }
}
