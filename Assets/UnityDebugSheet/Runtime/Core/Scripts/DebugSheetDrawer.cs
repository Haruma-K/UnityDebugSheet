using UnityDebugSheet.Runtime.Foundation.Drawer;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public sealed class DebugSheetDrawer : StatefulDrawer
    {
        private bool _didFirstUpdate;
        private bool _isPortrait;
        
        protected override void Update()
        {
            base.Update();
            
            var scaleFactor = Canvas.scaleFactor;
            var safeArea = Screen.safeArea;
            safeArea.position /= scaleFactor;
            safeArea.size /= scaleFactor;
            var isPortrait = safeArea.height > safeArea.width;
            var rectTrans = (RectTransform)transform;
            
            if (!_didFirstUpdate || _isPortrait != isPortrait)
            {
                var anchorMin = rectTrans.anchorMin;
                anchorMin.x = isPortrait ? 0.5f : 0.0f;
                rectTrans.anchorMin = anchorMin;

                var anchorMax = rectTrans.anchorMax;
                anchorMax.x = isPortrait ? 0.5f : 0.0f;
                rectTrans.anchorMax = anchorMax;

                var pivot = rectTrans.pivot;
                pivot.x = isPortrait ? 0.5f : 0.0f;
                rectTrans.pivot = pivot;

                var anchoredPosition = rectTrans.anchoredPosition;
                anchoredPosition.x = isPortrait ? 0.0f : 60.0f;
                anchoredPosition.x += safeArea.xMin;
                rectTrans.anchoredPosition = anchoredPosition;
                
                UseMiddleState = isPortrait;
                
                _isPortrait = isPortrait;
            }
            
            _didFirstUpdate = true;
        }
    }
}
