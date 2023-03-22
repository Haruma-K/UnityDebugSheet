using System;
using UnityDebugSheet.Runtime.Foundation.Drawer;
using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public sealed class FloatingButton : MonoBehaviour
    {
        [SerializeField] private RectTransform drawerRectTrans;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform contentsRectTrans;
        [SerializeField] private CanvasGroup childCanvasGroup;
        [SerializeField] private Button button;
        [SerializeField] private Text text;
        [SerializeField] private StatefulDrawerController drawerController;

        private bool _isShown;

        public bool Interactable
        {
            get => canvasGroup.interactable;
            set => canvasGroup.interactable = value;
        }

        public string Text
        {
            get => text.text;
            set => text.text = value;
        }

        private void Start()
        {
            Hide(true);
        }

        private void OnEnable()
        {
            button.onClick.AddListener(OnButtonClicked);
            drawerController.OnResizingStateChanged += DrawerResizingStateChanged;
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(OnButtonClicked);
            drawerController.OnResizingStateChanged -= DrawerResizingStateChanged;
        }

        public event Action OnClicked;

        private void OnButtonClicked()
        {
            OnClicked?.Invoke();
        }

        public void Show(bool force = false)
        {
            if (!force && _isShown)
                return;

            // Sync transform to drawer.
            var rectTrans = (RectTransform)transform;
            rectTrans.anchorMin = drawerRectTrans.anchorMin;
            rectTrans.anchorMax = drawerRectTrans.anchorMax;
            rectTrans.pivot = drawerRectTrans.pivot;
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, drawerRectTrans.rect.width);
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.x = drawerRectTrans.anchoredPosition.x;
            rectTrans.anchoredPosition = anchoredPosition;

            // Set position to the bottom of the safe area.
            var canvasScaleFactor = GetComponentInParent<Canvas>().scaleFactor;
            var anchoredPosY = Screen.safeArea.y / canvasScaleFactor;
            contentsRectTrans.anchoredPosition = new Vector2(contentsRectTrans.anchoredPosition.x, anchoredPosY);


            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            _isShown = true;
        }

        public void Hide(bool force = false)
        {
            if (!force && !_isShown)
                return;

            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            _isShown = false;
        }

        private void DrawerResizingStateChanged(StatefulDrawerController.DrawerResizingState state)
        {
            switch (state)
            {
                case StatefulDrawerController.DrawerResizingState.None:
                    childCanvasGroup.alpha = 1.0f;
                    childCanvasGroup.interactable = true;
                    childCanvasGroup.blocksRaycasts = true;
                    break;
                case StatefulDrawerController.DrawerResizingState.Animation:
                case StatefulDrawerController.DrawerResizingState.Dragging:
                    childCanvasGroup.alpha = 0.0f;
                    childCanvasGroup.interactable = false;
                    childCanvasGroup.blocksRaycasts = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }
}
