using UnityEngine;

namespace UnityDebugSheet.Runtime.Extensions
{
    public sealed class RuntimeSafeArea : MonoBehaviour
    {
        [SerializeField] private RectTransform[] _targets;

        private void Start()
        {
            if (_targets == null || _targets.Length == 0)
            {
                _targets = new RectTransform[transform.childCount];
                for (var i = 0; i < transform.childCount; i++)
                    _targets[i] = (RectTransform)transform.GetChild(i);
            }

            // Create SafeArea
            var safeAreaObj = new GameObject("SafeArea", typeof(RectTransform));
            safeAreaObj.transform.SetParent(transform, false);
            var safeAreaRectTrans = (RectTransform)safeAreaObj.transform;
            var anchorMin = Screen.safeArea.position;
            var anchorMax = Screen.safeArea.position + Screen.safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            safeAreaRectTrans.anchorMin = anchorMin;
            safeAreaRectTrans.anchorMax = anchorMax;
            safeAreaRectTrans.sizeDelta = Vector2.zero;

            // Add all children to SafeArea
            foreach (var target in _targets)
                target.SetParent(safeAreaRectTrans);
        }
    }
}
