using UnityEngine;

namespace UnityDebugSheet
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class CanvasGroupDrawerBackdrop : DrawerBackdrop
    {
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        protected override void OnStart()
        {
            base.OnStart();
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
