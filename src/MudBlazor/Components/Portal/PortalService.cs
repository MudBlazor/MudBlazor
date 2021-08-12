using System;
using System.Collections.Generic;
using System.Linq;

namespace MudBlazor.Services
{
    internal interface IPortal
    {
        IList<PortalItem> Items { get; }
        PortalItem GetItem(Guid id);
        void Add(PortalItem item);
        void Update(PortalItem item);
        void Remove(Guid id);
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
        /// Adds a PortalItem in case it doesn't exist
        /// </summary>
        /// <param name="newItem"></param>
        public void Add(PortalItem newItem)
        {
            lock (_lockObj)
            {
                _items.TryGetValue(newItem.Id, out var item);

                if (item == null)
                {
                    _items.Add(newItem.Id, newItem);
                    OnChange?.Invoke(this, new PortalEventsArg(newItem));
                }
            }
        }

        /// <summary>
        /// Updates the PortalItem
        /// </summary>
        /// <param name="newItem"></param>
        public void Update(PortalItem newItem)
        {
            lock (_lockObj)
            {
                _items.TryGetValue(newItem.Id, out var item);

                if (item == null)
                {
                    throw new Exception("Portal: You can't update a non existing item");
                }
                else
                {
                    item = newItem.Clone();
                    _items[item.Id] = item;
                    OnChange?.Invoke(this, new PortalEventsArg(item));
                }
            }
        }

        /// <summary>
        /// Removes a PortalItem
        /// </summary>
        /// <param name="id"></param>
        public void Remove(Guid id)
        {
            if (_items.Count == 0) return;

            lock (_lockObj)
            {
                if (_items.TryGetValue(id, out var item))
                {
                    _items.Remove(id);
                    OnChange?.Invoke(this, new PortalEventsArg(item));
                }
            }
        }

        /// <summary>
        /// Returns a PortalItem by Id
        /// </summary>
        public PortalItem GetItem(Guid id)
        {
            lock (_lockObj)
            {
                _items.TryGetValue(id, out var item);
                return item;
            }
        }

        /// <summary>
        /// The List of PortalItems that the Portal contains
        /// </summary>
        public IList<PortalItem> Items
        {
            get
            {
                lock (_lockObj)
                {
                    return _items.Values.ToList();
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
