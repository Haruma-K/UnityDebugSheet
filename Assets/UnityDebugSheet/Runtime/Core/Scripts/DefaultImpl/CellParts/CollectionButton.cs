using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts
{
    public sealed class CollectionButton : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Text _text;

        public bool Interactable
        {
            get => _button.interactable;
            set => _button.interactable = value;
        }

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        public Color TextColor
        {
            get => _text.color;
            set => _text.color = value;
        }

        public Button.ButtonClickedEvent Clicked => _button.onClick;
    }
}
