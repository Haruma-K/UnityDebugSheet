using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts
{
    public enum ClickToOpenMode
    {
        Left,
        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Off
    }

    public static class ClickToOpenModeExtensions
    {
        public static void ApplyToRectTransform(this ClickToOpenMode self, RectTransform target)
        {
            switch (self)
            {
                case ClickToOpenMode.Left:
                    target.anchorMin = new Vector2(0, 0.5f);
                    target.anchorMax = new Vector2(0, 0.5f);
                    target.pivot = new Vector2(0, 0.5f);
                    break;
                case ClickToOpenMode.TopLeft:
                    target.anchorMin = new Vector2(0, 1);
                    target.anchorMax = new Vector2(0, 1);
                    target.pivot = new Vector2(0, 1);
                    break;
                case ClickToOpenMode.Top:
                    target.anchorMin = new Vector2(0.5f, 1);
                    target.anchorMax = new Vector2(0.5f, 1);
                    target.pivot = new Vector2(0.5f, 1);
                    break;
                case ClickToOpenMode.TopRight:
                    target.anchorMin = new Vector2(1, 1);
                    target.anchorMax = new Vector2(1, 1);
                    target.pivot = new Vector2(1, 1);
                    break;
                case ClickToOpenMode.Right:
                    target.anchorMin = new Vector2(1, 0.5f);
                    target.anchorMax = new Vector2(1, 0.5f);
                    target.pivot = new Vector2(1, 0.5f);
                    break;
                case ClickToOpenMode.BottomRight:
                    target.anchorMin = new Vector2(1, 0);
                    target.anchorMax = new Vector2(1, 0);
                    target.pivot = new Vector2(1, 0);
                    break;
                case ClickToOpenMode.Bottom:
                    target.anchorMin = new Vector2(0.5f, 0);
                    target.anchorMax = new Vector2(0.5f, 0);
                    target.pivot = new Vector2(0.5f, 0);
                    break;
                case ClickToOpenMode.BottomLeft:
                    target.anchorMin = new Vector2(0, 0);
                    target.anchorMax = new Vector2(0, 0);
                    target.pivot = new Vector2(0, 0);
                    break;
                case ClickToOpenMode.Off:
                    break;
            }
        }
    }
}
