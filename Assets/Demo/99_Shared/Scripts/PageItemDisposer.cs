#if !EXCLUDE_UNITY_DEBUG_SHEET
using System;
using System.Collections.Generic;
using UnityDebugSheet.Runtime.Core.Scripts;
using UnityEngine.Assertions;

namespace Demo._99_Shared.Scripts
{
    /// <summary>
    ///     Remove registered debug sheet items when disabled.
    /// </summary>
    public sealed class PageItemDisposer : IDisposable
    {
        private readonly List<int> _itemIds = new List<int>();

        public PageItemDisposer(DebugPageBase page)
        {
            Assert.IsNotNull(page);

            Page = page;
        }

        private DebugPageBase Page { get; }

        public IReadOnlyList<int> ItemIds => _itemIds;

        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if (IsDisposed)
                return;

            Clear();
            IsDisposed = true;
        }

        public void AddItemId(int itemId)
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(PageItemDisposer));

            _itemIds.Add(itemId);
        }

        public void Clear()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(PageItemDisposer));

            foreach (var itemId in _itemIds)
            {
                if (Page == null)
                    return;
                Page.RemoveItem(itemId);
            }
        }
    }
}
#endif
