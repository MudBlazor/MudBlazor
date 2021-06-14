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

        event EventHandler OnChange;
    }
    internal class Portal : IPortal, IDisposable
    {
        private readonly Dictionary<Guid, PortalItem> _items = new();
        private readonly object _lockObj = new();
        private bool _disposedValue;

        public event EventHandler OnChange;

        public void AddOrUpdate(PortalItem newItem)
        {
            lock (_lockObj)
            {
                _items.TryGetValue(newItem.Id, out var item);
                if (item == null)
                    _items.Add(newItem.Id, newItem);
                else
                    item.ClientRect = newItem.ClientRect;
            }
            OnChange?.Invoke(this, null);
        }

        public PortalItem GetItem(Guid id)
        {
            lock (_lockObj)
            {
                return _items[id];
            }
        }

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

        public void Remove(PortalItem item)
        {
            lock (_lockObj)
            {
                _items.Remove(item.Id);
            }
            OnChange?.Invoke(this, null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _items.Clear();
                }
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
