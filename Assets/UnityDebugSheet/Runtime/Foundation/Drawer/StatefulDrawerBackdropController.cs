using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.Drawer
{
    [RequireComponent(typeof(StatefulDrawer))]
    [RequireComponent(typeof(StatefulDrawerController))]
    public class StatefulDrawerBackdropController : MonoBehaviour
    {
        [SerializeField] private DrawerBackdrop _backdrop;
        [SerializeField] private bool _closeWhenBackdropClicked;

        private StatefulDrawer _drawer;
        private StatefulDrawerController _drawerController;

        public DrawerBackdrop Backdrop
        {
            get => _backdrop;
            set => _backdrop = value;
        }

        private void OnEnable()
        {
            _drawer = GetComponent<StatefulDrawer>();
            _drawerController = GetComponent<StatefulDrawerController>();
            _drawer.ProgressUpdated += OnProgressUpdated;
            
            if (_backdrop != null)
                _backdrop.Clicked += OnBackdropClicked;
        }

        private void OnDisable()
        {
            _drawer.ProgressUpdated -= OnProgressUpdated;
            
            if (_backdrop != null)
                _backdrop.Clicked -= OnBackdropClicked;
        }

        private void OnProgressUpdated(float progress)
        {
            if (_backdrop == null)
                return;

            progress = Mathf.Clamp(progress, _drawer.MinProgress, _drawer.MaxProgress);
            progress = (progress - _drawer.MinProgress) / (_drawer.MaxProgress - _drawer.MinProgress);

            var displayProgress = 0.5f;
            if (_drawer.UseMiddleState)
            {
                displayProgress = Mathf.Min(displayProgress, _drawer.MiddleProgress);
                displayProgress = (displayProgress - _drawer.MinProgress) / (_drawer.MaxProgress - _drawer.MinProgress);
            }

            var backdropProgress = Mathf.Clamp01(progress / displayProgress);
            _backdrop.SetProgress(backdropProgress);
        }

        private void OnBackdropClicked()
        {
            if (_closeWhenBackdropClicked && !_drawer.IsInAnimation)
                _drawerController.SetStateWithAnimation(DrawerState.Min);
        }
    }
}
