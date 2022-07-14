using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Foundation.Drawer
{
    [RequireComponent(typeof(Button))]
    public abstract class DrawerBackdrop : MonoBehaviour
    {
        [SerializeField] private bool _blockInteraction = true;

        private Button _button;

        public float Progress { get; private set; }

        public bool Interactable { get; private set; }

        private void Start()
        {
            OnStart();
            SetProgress(0.0f, true);
        }

        protected virtual void OnEnable()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnClicked);
        }

        protected void OnDisable()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        public event UnityAction Clicked;

        protected virtual void OnStart()
        {
        }

        public void SetProgress(float progress, bool force = false)
        {
            if (!gameObject.activeSelf)
                return;
            
            SetProgressInternal(progress);
            Progress = progress;
            var interactable = Mathf.Approximately(Progress, 1.0f);
            if (!_blockInteraction)
                interactable = false;
            if (force ||  Interactable != interactable)
            {
                _button.image.raycastTarget = interactable;
                SetInteractable(interactable);
                Interactable = interactable;
            }
        }

        private void OnClicked()
        {
            Clicked?.Invoke();
        }

        protected abstract void SetProgressInternal(float visibility);

        protected abstract void SetInteractable(bool interactable);
    }
}
