// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Manages the communication between <see cref="MudSelect{T}"/> and <see cref="MudSelectItem{T}"/> components.
/// </summary>
/// <typeparam name="T">The type of value being selected.</typeparam>
internal sealed class MudSelectContext<T>
{
    private readonly MudSelect<T> _select;
    private readonly List<MudSelectItem<T>> _items = [];
    private readonly List<Func<IReadOnlyCollection<T?>, Task>> _selectionObservers = [];
    private readonly Dictionary<NullableObject<T?>, MudSelectItem<T>> _valueLookup = new();
    private readonly Dictionary<NullableObject<T?>, MudSelectItem<T>> _shadowLookup = new();

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
    public IReadOnlyList<MudSelectItem<T>> Items => _items;

    /// <summary>
    /// Gets all shadow items (including both visible and hidden items).
    /// </summary>
    public IReadOnlyCollection<MudSelectItem<T>> ShadowItems => _shadowLookup.Values;

    /// <summary>
    /// Gets whether multi-selection is enabled.
    /// </summary>
    public bool MultiSelection => _select.MultiSelection;

    /// <summary>
    /// Gets the current selected values.
    /// </summary>
    public IReadOnlyCollection<T?> SelectedValues => _select.GetSelectedValues() ?? Array.Empty<T?>();

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

        // Note: Do NOT add to _shadowLookup here - that's only for shadow items
        // Shadow items are registered separately via RegisterShadowItem

        // Check if this item's value is currently selected
        return _select.MultiSelection switch
        {
            true => _select.GetSelectedValues()?.Contains(item.Value) == true,
            false => _select.ReadValue?.Equals(item.Value) == true
        };
    }

    /// <summary>
    /// Unregisters an item from the dropdown list.
    /// </summary>
    /// <param name="item">The item to unregister.</param>
    public void UnregisterItem(MudSelectItem<T> item)
    {
        _items.Remove(item);
        _valueLookup.Remove(item.Value);
        // Note: Do NOT remove from _shadowLookup - that's managed by shadow items
    }

    /// <summary>
    /// Registers an item for value-to-RenderFragment lookup only (not visible in dropdown).
    /// </summary>
    /// <remarks>
    /// Used for items with HideContent=true that provide RenderFragments for selected values
    /// that may not be in the visible dropdown list.
    /// </remarks>
    /// <param name="item">The item to register.</param>
    public void RegisterShadowItem(MudSelectItem<T>? item)
    {
        if (item is null)
        {
            return;
        }

        _shadowLookup[item.Value] = item;
    }

    /// <summary>
    /// Unregisters a shadow item.
    /// </summary>
    /// <param name="item">The item to unregister.</param>
    public void UnregisterShadowItem(MudSelectItem<T>? item)
    {
        if (item is null)
        {
            return;
        }

        _shadowLookup.Remove(item.Value);
    }

    /// <summary>
    /// Attempts to get an item by its value from visible items.
    /// </summary>
    public bool TryGetItemByValue(T? value, [NotNullWhen(true)] out MudSelectItem<T>? item)
    {
        return _valueLookup.TryGetValue(value, out item);
    }

    /// <summary>
    /// Attempts to get an item by its value from all items (including shadow items).
    /// </summary>
    public bool TryGetShadowItemByValue(T? value, [NotNullWhen(true)] out MudSelectItem<T>? item)
    {
        return _shadowLookup.TryGetValue(value, out item);
    }

    /// <summary>
    /// Subscribes to selection changes.
    /// </summary>
    /// <param name="observer">The callback to invoke when selection changes.</param>
    /// <returns>A disposable subscription that can be used to unsubscribe.</returns>
    public IDisposable SubscribeToSelectionChanges(Func<IReadOnlyCollection<T?>, Task> observer)
    {
        _selectionObservers.Add(observer);
        return new SelectionSubscription(this, observer);
    }

    /// <summary>
    /// Unsubscribes an observer from selection changes.
    /// </summary>
    private void Unsubscribe(Func<IReadOnlyCollection<T?>, Task> observer)
    {
        _selectionObservers.Remove(observer);
    }

    /// <summary>
    /// Notifies all observers of a selection change.
    /// </summary>
    public async Task NotifySelectionChangedAsync()
    {
        var selectedValues = SelectedValues;
        for (var i = _selectionObservers.Count - 1; i >= 0; i--)
        {
            await _selectionObservers[i](selectedValues);
        }
    }

    private sealed class SelectionSubscription(MudSelectContext<T> context, Func<IReadOnlyCollection<T?>, Task> observer)
        : IDisposable
    {
        private MudSelectContext<T>? _context = context;
        private Func<IReadOnlyCollection<T?>, Task>? _observer = observer;

        public void Dispose()
        {
            var contextCopy = _context;
            var observerCopy = _observer;

            if (contextCopy is null || observerCopy is null)
            {
                return;
            }

            _context = null;
            _observer = null;

            contextCopy.Unsubscribe(observerCopy);
        }
    }
}
