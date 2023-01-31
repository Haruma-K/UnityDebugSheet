using System;
using UnityEngine;

namespace UnityDebugSheet.Runtime.Core.Scripts.DefaultImpl.CellParts
{
    public sealed class CollectionButtonModel
    {
        public string Text { get; set; }

        public Color TextColor { get; set; } = Color.black;

        public bool Interactable { get; set; } = true;

        public event Action Clicked;

        internal void InvokeClicked()
        {
            Clicked?.Invoke();
        }
    }

    public static class CollectionButtonExtensions
    {
        public static void Setup(this CollectionButton self, CollectionButtonModel model)
        {
            self.Text = model.Text;
            self.TextColor = model.TextColor;
            self.Interactable = model.Interactable;
            self.Clicked.RemoveAllListeners();
            self.Clicked.AddListener(model.InvokeClicked);
        }
    }
}
