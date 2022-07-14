using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts
{
    public sealed class CellTextsModel
    {
        public string Text { get; set; }

        public string SubText { get; set; }

        public Color TextColor { get; set; } = Color.black;

        public Color SubTextColor { get; set; } = Color.gray;
    }

    public static class CellTextsExtensions
    {
        public static void Setup(this CellTexts self, CellTextsModel model)
        {
            self.Text = model.Text;
            self.SubText = model.SubText;
            self.TextColor = model.TextColor;
            self.SubTextColor = model.SubTextColor;
        }
    }
}
