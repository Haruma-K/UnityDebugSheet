using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts
{
    public sealed class CellTexts : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private Text _subText;

        public string Text
        {
            get => _text.text;
            set
            {
                if (_text.text == value)
                    return;

                _text.text = value;
                RefreshTransform();
            }
        }

        public string SubText
        {
            get => _subText.text;
            set
            {
                if (_subText.text == value)
                    return;

                _subText.text = value;
                RefreshTransform();
            }
        }

        public Color TextColor
        {
            get => _text.color;
            set => _text.color = value;
        }

        public Color SubTextColor
        {
            get => _subText.color;
            set => _subText.color = value;
        }

        private void Start()
        {
            RefreshTransform();
        }

        private void RefreshTransform()
        {
            var rectTrans = (RectTransform)transform;
            var hasSubText = !string.IsNullOrEmpty(_subText.text);
            var height = hasSubText ? 68 : 42;
            _subText.gameObject.SetActive(hasSubText);
            rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}
