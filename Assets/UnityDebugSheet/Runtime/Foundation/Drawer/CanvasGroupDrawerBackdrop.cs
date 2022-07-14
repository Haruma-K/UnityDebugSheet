using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Drawer
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class CanvasGroupDrawerBackdrop : DrawerBackdrop
    {
        private CanvasGroup _canvasGroup;
        
        protected override void OnStart()
        {
            base.OnStart();
            _canvasGroup = GetComponent<CanvasGroup>();
        }
        
        protected override void SetProgressInternal(float visibility)
        {
            _canvasGroup.alpha = visibility;
        }

        protected override void SetInteractable(bool interactable)
        {
            _canvasGroup.interactable = interactable;
        }
    }
}
