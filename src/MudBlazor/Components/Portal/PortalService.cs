using System;
using System.Collections.Generic;

namespace MudBlazor.Services
{
    internal interface IPortal
    {
        Dictionary<Guid, PortalItem> Items { get; }
        PortalItem GetItem(Guid id);
        void AddOrUpdate(PortalItem item);
        void Remove(PortalItem item);

        event EventHandler<PortalEventsArg> OnChange;
    }
    internal class Portal : IPortal, IDisposable
    {
        private readonly Dictionary<Guid, PortalItem> _items = new();
        private readonly object _lockObj = new();
        private bool _disposed;

        /// <summary>
        /// Invoked when the Portal adds or removes an item
        /// </summary>
        public event EventHandler<PortalEventsArg> OnChange;

        /// <summary>
        /// Adds a PortalItem. If the item already exists, then updates it
        /// </summary>
        /// <param name="newItem"></param>
        public void AddOrUpdate(PortalItem newItem)
        {
            lock (_lockObj)
            {
                _items.TryGetValue(newItem.Id, out var item);

                if (item == null)
                    _items.Add(newItem.Id, newItem);
                else
                    item.AnchorRect = newItem.AnchorRect;
            }

            OnChange?.Invoke(this, new PortalEventsArg(newItem));
        }

        /// <summary>
        /// Removes a PortalItem
        /// </summary>
        /// <param name="item"></param>
        public void Remove(PortalItem item)
        {
            if (_items.Count == 0) return;
            lock (_lockObj)
            {
                _items.Remove(item.Id);
            }

            OnChange?.Invoke(this, new PortalEventsArg(item));
        }

        /// <summary>
        /// Returns a PortalItem by Id
        /// </summary>
        public PortalItem GetItem(Guid id)
        {
            lock (_lockObj)
            {
                return _items[id];
            }
        }

        /// <summary>
        /// The dictionary of PortalItems that the Portal contains
        /// </summary>
        public Dictionary<Guid, PortalItem> Items
        {
            get
            {
                lock (_lockObj)
                {
                    return _items;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_disposed) return;
            _disposed = true;
            lock (_lockObj)
            {
                _items.Clear();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// The PortalEventsArg passed to OnChange when an item is added, removed or updated
    /// </summary>
    public class PortalEventsArg : EventArgs
    {
        public PortalEventsArg(PortalItem item)
        {
            Item = item;
        }
        public PortalItem Item { get; set; }
    }
}
