using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Foundation
{
    [ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class UITextEllipsisOverflow : UIBehaviour
    {
        private const string Ellipsis = "...";

        [SerializeField] [HideInInspector] private Text _text;

        [SerializeField] [Tooltip("If set to true, the text will actually be overwritten in EditMode.")]
        private bool _applyInEditMode = true;

        private string _textValue = string.Empty;
        private string _originalTextValue = string.Empty;

        protected override void Awake()
        {
            base.Awake();
            if (!Application.isPlaying)
            {
                _text = GetComponent<Text>();
                _textValue = _text.text;
                _originalTextValue = _text.text;
            }
        }

        private void Update()
        {
            if (_text.text == _textValue) 
                return;
            
            _originalTextValue = _text.text;
            Apply();
            _textValue = _text.text;
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            Apply();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (!Application.isPlaying) Apply();
        }
#endif

        private void Apply()
        {
            if (!_applyInEditMode && !Application.isPlaying) return;

            if (!IsActive() || _text == null) return;

            var text = _originalTextValue;

            var rectTransform = _text.rectTransform;
            var generator = _text.cachedTextGenerator;
            var settings = _text.GetGenerationSettings(rectTransform.rect.size);
            generator.Populate(text, settings);

            if (rectTransform.rect.width <= 0 || rectTransform.rect.height <= 0)
                // Do nothing because the layout seems not to have been built yet.
                return;

            if (text.Length == 0) return;

            if (_text.horizontalOverflow == HorizontalWrapMode.Wrap)
            {
                var height = generator.GetPreferredHeight(text, settings) / settings.scaleFactor;

                if (rectTransform.rect.size.y >= height) return;

                while (true)
                {
                    text = text.Remove(text.Length - 1);
                    height = generator.GetPreferredHeight(text + Ellipsis, settings) / settings.scaleFactor;

                    if (text.Length == 0) break;

                    if (rectTransform.rect.size.y >= height)
                    {
                        text += Ellipsis;
                        break;
                    }
                }
            }

            if (_text.horizontalOverflow == HorizontalWrapMode.Overflow)
            {
                var width = generator.GetPreferredWidth(text, settings) / settings.scaleFactor;

                if (rectTransform.rect.size.x >= width) return;

                while (true)
                {
                    text = text.Remove(text.Length - 1);
                    width = generator.GetPreferredWidth(text + Ellipsis, settings) / settings.scaleFactor;

                    if (text.Length == 0) break;

                    if (rectTransform.rect.size.x >= width)
                    {
                        text += Ellipsis;
                        break;
                    }
                }
            }

            _text.text = text;
        }
    }
}
