using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interop;

namespace MudBlazor.Services
{
    public interface IPortal
    {
        Dictionary<Guid, PortalItem> Items { get; }
        PortalItem GetItem(Guid id);
        void AddOrUpdate(PortalItem item);
        void Remove(PortalItem item);

        event EventHandler OnChange;
    }
    public class Portal : IPortal
    {
        private readonly Dictionary<Guid, PortalItem> _items = new();
        private readonly object _lockObj = new();
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
    }

    public class PortalItem
    {
        public Guid Id { get; set; }

        public RenderFragment Fragment { get; set; }

        public BoundingClientRect ClientRect { get; set; }

        public bool AutoDirection { get; set; }

        public string Position { get; set; } = "absolute";

    }
}
