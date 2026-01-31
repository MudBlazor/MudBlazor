// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable

    /// <summary>
    /// Manages the communication between <see cref="MudSelect{T}"/> and <see cref="MudSelectItem{T}"/> components.
    /// </summary>
    /// <remarks>
    /// This context provides a clean, explicit communication model:
    /// <list type="bullet">
    /// <item>Items register and unregister explicitly via Add/Remove methods</item>
    /// <item>Selection state is managed centrally in the context</item>
    /// <item>Items observe selection changes via a subscription pattern</item>
    /// <item>No hidden side effects or event-based synchronization</item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">The type of value being selected.</typeparam>
    internal sealed class MudSelectContext<T>
    {
        private readonly MudSelect<T> _select;
        private readonly List<MudSelectItem<T>> _items = new();
        private readonly Dictionary<NullableObject<T?>, MudSelectItem<T>> _valueLookup = new();
        private readonly Dictionary<NullableObject<T?>, MudSelectItem<T>> _shadowLookup = new();
        private readonly List<Action<IReadOnlyCollection<T?>>> _selectionObservers = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="MudSelectContext{T}"/> class.
        /// </summary>
        /// <param name="select">The parent select component.</param>
        public MudSelectContext(MudSelect<T> select)
        {
            _select = select;
        }

        /// <summary>
        /// Gets the ordered list of all visible items in the dropdown.
        /// </summary>
        /// <remarks>
        /// Exposed as List to support FindIndex operations in the parent component.
        /// </remarks>
        public List<MudSelectItem<T>> Items => _items;

        /// <summary>
        /// Gets all shadow items (including both visible and hidden items).
        /// </summary>
        public IEnumerable<MudSelectItem<T>> ShadowItems => _shadowLookup.Values;

        /// <summary>
        /// Gets whether multi-selection is enabled.
        /// </summary>
        public bool MultiSelection => _select.MultiSelection;

        /// <summary>
        /// Gets the current selected values.
        /// </summary>
        public IReadOnlyCollection<T?> SelectedValues
        {
            get
            {
                var values = _select.GetSelectedValues();
                return (values as IReadOnlyCollection<T?>) ?? values?.ToList() ?? (IReadOnlyCollection<T?>)Array.Empty<T?>();
            }
        }

        /// <summary>
        /// Registers an item as visible in the dropdown list.
        /// </summary>
        /// <param name="item">The item to register.</param>
        /// <returns><c>true</c> if the item is currently selected; otherwise <c>false</c>.</returns>
        public bool RegisterItem(MudSelectItem<T> item)
        {
            // Add to the ordered list of visible items
            if (!_items.Contains(item))
            {
                _items.Add(item);
            }

            // Add to value lookup for fast access by value
            _valueLookup[item.Value] = item;

            // Also add to shadow lookup (visible items are also accessible via shadow lookup)
            _shadowLookup[item.Value] = item;

            // Notify parent that an item was added (for UpdateSelectAllChecked and similar)
            _select.OnItemRegistered();

            // Check if this item's value is currently selected
            var currentValue = _select.GetCurrentValue();
            var selectedValues = _select.GetSelectedValues();
            return currentValue?.Equals(item.Value) == true ||
                   (selectedValues?.Contains(item.Value) == true);
        }

        /// <summary>
        /// Unregisters an item from the dropdown list.
        /// </summary>
        /// <param name="item">The item to unregister.</param>
        public void UnregisterItem(MudSelectItem<T> item)
        {
            _items.Remove(item);
            _valueLookup.Remove(item.Value);
            _shadowLookup.Remove(item.Value);
        }

        /// <summary>
        /// Registers an item for value-to-RenderFragment lookup only (not visible in dropdown).
        /// </summary>
        /// <remarks>
        /// Used for items with HideContent=true that provide RenderFragments for selected values
        /// that may not be in the visible dropdown list.
        /// </remarks>
        /// <param name="item">The item to register.</param>
        public void RegisterShadowItem(MudSelectItem<T> item)
        {
            _shadowLookup[item.Value] = item;
        }

        /// <summary>
        /// Unregisters a shadow item.
        /// </summary>
        /// <param name="item">The item to unregister.</param>
        public void UnregisterShadowItem(MudSelectItem<T> item)
        {
            _shadowLookup.Remove(item.Value);
        }

        /// <summary>
        /// Attempts to get an item by its value from visible items.
        /// </summary>
        public bool TryGetItemByValue(T? value, out MudSelectItem<T>? item)
        {
            return _valueLookup.TryGetValue(value, out item);
        }

        /// <summary>
        /// Attempts to get an item by its value from all items (including shadow items).
        /// </summary>
        public bool TryGetShadowItemByValue(T? value, out MudSelectItem<T>? item)
        {
            return _shadowLookup.TryGetValue(value, out item);
        }

        /// <summary>
        /// Subscribes to selection changes.
        /// </summary>
        /// <param name="observer">The callback to invoke when selection changes.</param>
        /// <returns>A disposable subscription that can be used to unsubscribe.</returns>
        public IDisposable SubscribeToSelectionChanges(Action<IReadOnlyCollection<T?>> observer)
        {
            _selectionObservers.Add(observer);
            return new SelectionSubscription(this, observer);
        }

        /// <summary>
        /// Notifies all observers of a selection change.
        /// </summary>
        public void NotifySelectionChanged()
        {
            var selectedValues = SelectedValues;
            foreach (var observer in _selectionObservers.ToArray())
            {
                observer(selectedValues);
            }
        }

        /// <summary>
        /// Unsubscribes an observer from selection changes.
        /// </summary>
        private void Unsubscribe(Action<IReadOnlyCollection<T?>> observer)
        {
            _selectionObservers.Remove(observer);
        }

        /// <summary>
        /// Represents a subscription to selection changes that can be disposed to unsubscribe.
        /// </summary>
        private sealed class SelectionSubscription : IDisposable
        {
            private MudSelectContext<T>? _context;
            private Action<IReadOnlyCollection<T?>>? _observer;

            public SelectionSubscription(MudSelectContext<T> context, Action<IReadOnlyCollection<T?>> observer)
            {
                _context = context;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_context != null && _observer != null)
                {
                    _context.Unsubscribe(_observer);
                    _context = null;
                    _observer = null;
                }
            }
        }
    }
}
