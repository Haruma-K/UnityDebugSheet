using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.MultiClicks
{
    [RequireComponent(typeof(Button))]
    public sealed class ButtonClickEventDispatcher : ClickEventDispatcherBase
    {
        public Button button;

        private void Reset()
        {
            button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            button.onClick.AddListener(ButtonClicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(ButtonClicked);
        }

        private void ButtonClicked()
        {
            onClicked.Invoke();
        }
    }
}
