using System;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Foundation.PageNavigator
{
    public enum PageAlignment
    {
        Left,
        Top,
        Right,
        Bottom,
        Center
    }

    internal static class PageAlignmentExtensions
    {
        public static Vector3 ToPosition(this PageAlignment self, RectTransform rectTransform)
        {
            Vector3 position;
            var rect = rectTransform.rect;
            var width = rect.width;
            var height = rect.height;
            var z = rectTransform.localPosition.z;
            switch (self)
            {
                case PageAlignment.Left:
                    position = new Vector3(-width, 0, z);
                    break;
                case PageAlignment.Top:
                    position = new Vector3(0, height, z);
                    break;
                case PageAlignment.Right:
                    position = new Vector3(width, 0, z);
                    break;
                case PageAlignment.Bottom:
                    position = new Vector3(0, -height, z);
                    break;
                case PageAlignment.Center:
                    position = new Vector3(0, 0, z);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(self), self, null);
            }

            return position;
        }
    }
}
