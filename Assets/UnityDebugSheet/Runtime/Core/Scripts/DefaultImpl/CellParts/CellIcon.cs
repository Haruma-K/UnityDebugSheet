using UnityEngine;
using UnityEngine.UI;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts
{
    public sealed class CellIcon : MonoBehaviour
    {
        [SerializeField] private Image _image;

        public Sprite Sprite
        {
            get => _image.sprite;
            set => _image.sprite = value;
        }

        public Color Color
        {
            get => _image.color;
            set => _image.color = value;
        }
    }
}
