using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts
{
    public sealed class CellIconModel
    {
        public Sprite Sprite { get; set; }

        public Color Color { get; set; } = Color.white;
    }

    public static class CellIconExtensions
    {
        public static void Setup(this CellIcon self, CellIconModel model)
        {
            self.Sprite = model.Sprite;
            self.Color = model.Color;
        }
    }
}
