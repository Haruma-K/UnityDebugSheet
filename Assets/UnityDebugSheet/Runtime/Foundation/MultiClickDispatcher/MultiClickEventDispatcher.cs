using UnityEngine;
using UnityEngine.Events;

namespace UnityDebugSheet.Runtime.Foundation.Gestures.MultiClicks
{
    public sealed class MultiClickEventDispatcher : MonoBehaviour
    {
        public UnityEvent onMultiClicked = new UnityEvent();
        public ClickEventDispatcherBase clickEventDispatcher;
        [Range(1, 5)] public int clickCountThreshold = 3;
        public float resetTimeThresholdSec = 1.0f;

        private int _currentClickCount;
        private float _timeFromLastClickSec;

        private void Reset()
        {
            clickEventDispatcher = GetComponent<ClickEventDispatcherBase>();
        }

        private void Update()
        {
            _timeFromLastClickSec += Time.unscaledDeltaTime;
            if (_currentClickCount >= 1 && _timeFromLastClickSec >= resetTimeThresholdSec)
                _currentClickCount = 0;
        }

        private void OnEnable()
        {
            clickEventDispatcher.onClicked.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            clickEventDispatcher.onClicked.RemoveListener(OnClicked);
        }

        private void OnValidate()
        {
            resetTimeThresholdSec = Mathf.Max(0.1f, resetTimeThresholdSec);
        }

        private void OnClicked()
        {
            _currentClickCount++;
            _timeFromLastClickSec = 0.0f;
            if (_currentClickCount < clickCountThreshold)
                return;

            onMultiClicked.Invoke();
            _currentClickCount = 0;
        }
    }
}
