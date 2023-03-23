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
        [SerializeField] private StatefulDrawer drawer;

        private bool _isPortrait;
        
        public bool IsShown { get; private set; }

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
            if (!force && IsShown)
                return;

            SetupTransform();

            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            IsShown = true;
        }

        private void Update()
        {
            var isPortrait = Screen.height >= Screen.width;
#if !UNITY_EDITOR
            if (_isPortrait == isPortrait)
                return;
#endif
            SetupTransform();
            _isPortrait = isPortrait;
        }

        private void SetupTransform()
        {
            var rectTrans = (RectTransform)transform;

            // 1. Match anchors and pivot to the drawer.
            rectTrans.anchorMin = drawerRectTrans.anchorMin;
            rectTrans.anchorMax = drawerRectTrans.anchorMax;
            rectTrans.pivot = drawerRectTrans.pivot;

            // 2. Set width and position.
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, drawerRectTrans.rect.width);
            var anchoredPosition = rectTrans.anchoredPosition;
            anchoredPosition.x = drawerRectTrans.anchoredPosition.x;
            rectTrans.anchoredPosition = anchoredPosition;

            // 3. Set bottom position to the bottom of the safe area.
            var canvasScaleFactor = GetComponentInParent<Canvas>().scaleFactor;
            var anchoredPosY = Screen.safeArea.y / canvasScaleFactor;
            contentsRectTrans.anchoredPosition = new Vector2(contentsRectTrans.anchoredPosition.x, anchoredPosY);
        }

        public void Hide(bool force = false)
        {
            if (!force && !IsShown)
                return;

            canvasGroup.alpha = 0.0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            IsShown = false;
        }

        private void DrawerResizingStateChanged(StatefulDrawerController.DrawerResizingState resizingState)
        {
            switch (resizingState)
            {
                case StatefulDrawerController.DrawerResizingState.None:
                    var drawerState = drawer.GetNearestState();
                    if (drawerState == DrawerState.Min)
                    {
                        childCanvasGroup.alpha = 0.0f;
                        childCanvasGroup.interactable = false;
                        childCanvasGroup.blocksRaycasts = false;
                    }
                    else
                    {
                        childCanvasGroup.alpha = 1.0f;
                        childCanvasGroup.interactable = true;
                        childCanvasGroup.blocksRaycasts = true;
                    }

                    break;
                case StatefulDrawerController.DrawerResizingState.Animation:
                case StatefulDrawerController.DrawerResizingState.Dragging:
                    childCanvasGroup.alpha = 0.0f;
                    childCanvasGroup.interactable = false;
                    childCanvasGroup.blocksRaycasts = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resizingState), resizingState, null);
            }
        }
    }
}
