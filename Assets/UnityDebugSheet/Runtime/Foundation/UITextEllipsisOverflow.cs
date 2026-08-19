using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UnityDebugSheet
{
    [ExecuteAlways]
    [RequireComponent(typeof(Text))]
    public class UITextEllipsisOverflow : UIBehaviour
    {
        private const string Ellipsis = "...";

        [SerializeField] [HideInInspector] private Text _text;

        [SerializeField] [Tooltip("If set to true, the text will actually be overwritten in EditMode.")]
        private bool _applyInEditMode = true;

        private string _sourceText;
        private string _displayedText;

        protected override void Awake()
        {
            base.Awake();
            if (_text == null)
                _text = GetComponent<Text>();

            _sourceText = _text.text;
            _displayedText = _text.text;
        }

        private void Update()
        {
            if (_text == null || _text.text == _displayedText)
                return;

            Apply();
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

            if (_text.text != _displayedText)
                _sourceText = _text.text;

            var rectTransform = _text.rectTransform;

            if (rectTransform.rect.width <= 0 || rectTransform.rect.height <= 0)
                // Do nothing because the layout seems not to have been built yet.
            {
                _displayedText = _text.text;
                return;
            }

            var generator = _text.cachedTextGenerator;
            var settings = _text.GetGenerationSettings(rectTransform.rect.size);
            generator.Populate(_sourceText, settings);

            var text = _sourceText;

            if (text.Length == 0)
            {
                SetDisplayedText(text);
                return;
            }

            if (_text.horizontalOverflow == HorizontalWrapMode.Wrap)
            {
                var height = generator.GetPreferredHeight(text, settings) / settings.scaleFactor;

                if (rectTransform.rect.size.y >= height)
                {
                    SetDisplayedText(text);
                    return;
                }

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

                if (rectTransform.rect.size.x >= width)
                {
                    SetDisplayedText(text);
                    return;
                }

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

            SetDisplayedText(text);
        }

        private void SetDisplayedText(string text)
        {
            if (_text.text != text)
                _text.text = text;

            _displayedText = text;
        }
    }
}
