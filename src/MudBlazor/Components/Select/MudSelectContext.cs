// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using MudBlazor.Interfaces;
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
    private readonly Dictionary<NullableObject<T?>, bool> _expansion = new();
    private int _nestedShadowItemCount;
    private readonly List<MudSelectItem<T>> _shadowRoots = new();
    private readonly Dictionary<MudSelectItem<T>, List<MudSelectItem<T>>> _shadowChildren = new();

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
    /// Gets the icon for selected items when multi-selection is enabled.
    /// </summary>
    public string CheckedIcon => _select.CheckedIcon;

    /// <summary>
    /// Gets the icon for unselected items when multi-selection is enabled.
    /// </summary>
    public string UncheckedIcon => _select.UncheckedIcon;

    /// <summary>
    /// Gets the current selected values.
    /// </summary>
    public IReadOnlyCollection<T?> SelectedValues => _select.GetSelectedValues() ?? Array.Empty<T?>();

    /// <summary>
    /// Gets the user-supplied value comparer from the parent <see cref="MudSelect{T}"/>, if any.
    /// </summary>
    public IEqualityComparer<T?>? Comparer => _select.Comparer;

    /// <summary>
    /// Registers an item as visible in the dropdown list.
    /// </summary>
    /// <param name="item">The item to register.</param>
    /// <returns><c>true</c> if the item is currently selected; otherwise <c>false</c>.</returns>
    public bool RegisterItem(MudSelectItem<T> item)
    {
        // An item registers once per select and unregisters before moving to another, so checking for a duplicate would only make opening n options cost n².
        if (item.ParentItem is { } parent && _items.IndexOf(parent) is var parentIndex and >= 0)
        {
            // A branch can expand after its siblings registered, so a nested item goes to the end of its parent's subtree to keep the list in display order.
            var insertAt = parentIndex + 1;
            while (insertAt < _items.Count && _items[insertAt].Level > parent.Level)
            {
                insertAt++;
            }

            _items.Insert(insertAt, item);
        }
        else
        {
            _items.Add(item);
        }

        // Add to value lookup for fast access by value
        _valueLookup[item.Value] = item;

        // Note: Do NOT add to _shadowLookup here - that's only for shadow items
        // Shadow items are registered separately via RegisterShadowItem

        return IsItemSelected(item);
    }

    /// <summary>
    /// Check if this item's value is currently selected using the select comparer if specified
    /// </summary>
    /// <param name="item">The item to check</param>
    /// <returns></returns>
    public bool IsItemSelected(MudSelectItem<T> item)
    {
        var comparer = _select.Comparer ?? EqualityComparer<T?>.Default;
        if (!_select.MultiSelection)
        {
            return comparer.Equals(_select.ReadValue, item.Value);
        }

        return _select.GetSelectedValues() switch
        {
            null => false,
            // MudSelect keeps its selection in a HashSet built with the select's comparer, so its O(1) lookup gives the same answer.
            HashSet<T?> set when set.Comparer.Equals(comparer) => set.Contains(item.Value),
            // A caller-supplied collection can be a list or use a different comparer.
            var values => values.Contains(item.Value, comparer),
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
        if (item.HasNestedItems)
        {
            _nestedShadowItemCount++;
        }

        GetShadowSiblings(item, create: true)?.Add(item);

        _select.InvalidateFitContent();
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

        if (_shadowLookup.Remove(item.Value) && item.HasNestedItems)
        {
            _nestedShadowItemCount--;
        }

        GetShadowSiblings(item, create: false)?.Remove(item);
        _shadowChildren.Remove(item);

        _select.InvalidateFitContent();
    }

    /// <summary>
    /// Gets whether any item contains nested items, which makes the popup a tree instead of a list.
    /// </summary>
    public bool HasNestedItems => _nestedShadowItemCount > 0;

    /// <summary>
    /// Gets the shadow items that share a parent with the given item, in markup order.
    /// </summary>
    private List<MudSelectItem<T>>? GetShadowSiblings(MudSelectItem<T> item, bool create)
    {
        if (item.ParentItem is not { } parent)
        {
            return _shadowRoots;
        }

        if (_shadowChildren.TryGetValue(parent, out var siblings))
        {
            return siblings;
        }

        return create ? _shadowChildren[parent] = new List<MudSelectItem<T>>() : null;
    }

    /// <summary>
    /// Gets whether the item is the last one under its parent, which decides how the tree lines are drawn.
    /// </summary>
    /// <remarks>
    /// The hidden copy always renders the whole tree, so it knows every sibling even while a branch is collapsed or still rendering.
    /// </remarks>
    public bool IsLastChild(MudSelectItem<T> item)
    {
        if (!_shadowLookup.TryGetValue(item.Value, out var shadow)
            || GetShadowSiblings(shadow, create: false) is not { Count: > 0 } siblings)
        {
            return true;
        }

        return ReferenceEquals(siblings[^1], shadow);
    }

    /// <summary>
    /// Updates the visible-item lookup when a registered item's <see cref="MudSelectItem{T}.Value"/> parameter changes.
    /// </summary>
    /// <remarks>
    /// The lookup is keyed by value captured at registration time. When the value parameter is later mutated
    /// (e.g. the parent swaps the underlying data collection while reusing the same component instances),
    /// the old key would otherwise point to a stale <see cref="MudSelectItem{T}"/> reference and the new value
    /// would not be found at all.
    /// </remarks>
    public void OnItemValueChanged(MudSelectItem<T> item, T? oldValue, T? newValue)
    {
        var oldKey = new NullableObject<T?>(oldValue);
        if (_valueLookup.TryGetValue(oldKey, out var existing) && ReferenceEquals(existing, item))
        {
            _valueLookup.Remove(oldKey);
        }

        _valueLookup[newValue] = item;
    }

    /// <summary>
    /// Updates the shadow lookup when a registered shadow item's <see cref="MudSelectItem{T}.Value"/> parameter changes.
    /// </summary>
    /// <remarks>
    /// Also requests a re-render of the parent <see cref="MudSelect{T}"/>: <c>GetSelectedValuePresenter</c> runs
    /// during the parent's render pass, before child <c>SetParametersAsync</c> has propagated the new value,
    /// so a second render is needed for the correct <see cref="MudSelectItem{T}.ChildContent"/> to be emitted.
    /// </remarks>
    public void OnShadowItemValueChanged(MudSelectItem<T> item, T? oldValue, T? newValue)
    {
        var oldKey = new NullableObject<T?>(oldValue);
        if (_shadowLookup.TryGetValue(oldKey, out var existing) && ReferenceEquals(existing, item))
        {
            _shadowLookup.Remove(oldKey);
        }

        _shadowLookup[newValue] = item;
        _select.InvalidateFitContent();

        // Re-render only when the changed value is the one shown by the selected-value presenter
        // (single selection only). An unconditional StateHasChanged loops forever when an item's
        // Value is a fresh reference each render, e.g. an inline Value="@(new ...)" (#13281).
        if (!_select.MultiSelection)
        {
            var comparer = Comparer ?? EqualityComparer<T?>.Default;
            if (comparer.Equals(oldValue, _select.ReadValue) || comparer.Equals(newValue, _select.ReadValue))
            {
                ((IMudStateHasChanged)_select).StateHasChanged();
            }
        }
    }

    /// <summary>
    /// Attempts to get an item by its value from visible items.
    /// </summary>
    /// <remarks>
    /// When a custom <see cref="Comparer"/> is set the lookup honors it via a linear scan;
    /// otherwise the fast dictionary lookup (default equality) is used. This keeps value-to-item
    /// resolution consistent with the comparer-aware selection state.
    /// </remarks>
    public bool TryGetItemByValue(T? value, [NotNullWhen(true)] out MudSelectItem<T>? item)
    {
        var comparer = Comparer;
        if (comparer is null)
        {
            return _valueLookup.TryGetValue(value, out item);
        }

        foreach (var candidate in _items)
        {
            if (comparer.Equals(candidate.Value, value))
            {
                item = candidate;
                return true;
            }
        }

        item = null;
        return false;
    }

    /// <summary>
    /// Attempts to get an item by its value from all items (including shadow items).
    /// </summary>
    /// <remarks>
    /// When a custom <see cref="Comparer"/> is set the lookup honors it via a linear scan;
    /// otherwise the fast dictionary lookup (default equality) is used. This keeps value-to-item
    /// resolution consistent with the comparer-aware selection state.
    /// </remarks>
    public bool TryGetShadowItemByValue(T? value, [NotNullWhen(true)] out MudSelectItem<T>? item)
    {
        var comparer = Comparer;
        if (comparer is null)
        {
            return _shadowLookup.TryGetValue(value, out item);
        }

        foreach (var candidate in _shadowLookup.Values)
        {
            if (comparer.Equals(candidate.Value, value))
            {
                item = candidate;
                return true;
            }
        }

        item = null;
        return false;
    }

    /// <summary>
    /// Gets whether the user or the select has expanded or collapsed the item with this value.
    /// </summary>
    /// <remarks>
    /// The popup unmounts its items when it closes, so the expansion is kept here to survive reopening.
    /// </remarks>
    public bool TryGetExpansion(T? value, out bool expanded) => _expansion.TryGetValue(value, out expanded);

    /// <summary>
    /// Remembers whether the item with this value is expanded.
    /// </summary>
    public void SetExpansion(T? value, bool expanded) => _expansion[value] = expanded;

    /// <summary>
    /// Expands every ancestor of the item with this value, so the item is visible when the popup opens.
    /// </summary>
    /// <remarks>
    /// Items already in the popup are expanded directly.
    /// The rest pick the remembered expansion up when they are created.
    /// </remarks>
    public async Task RevealItemAsync(T? value)
    {
        if (!TryGetShadowItemByValue(value, out var item))
        {
            return;
        }

        for (var ancestor = item.ParentItem; ancestor is not null; ancestor = ancestor.ParentItem)
        {
            SetExpansion(ancestor.Value, true);
            if (TryGetItemByValue(ancestor.Value, out var visible))
            {
                await visible.SetExpandedAsync(true);
            }
        }
    }

    /// <summary>
    /// Gets every item of the select, including those under collapsed branches.
    /// </summary>
    /// <remarks>
    /// Flat selects keep using the registered items so their behavior does not change.
    /// Only a select with nested items needs the hidden copy, because collapsed branches are not rendered in the popup.
    /// </remarks>
    public IEnumerable<MudSelectItem<T>> AllItems => HasNestedItems ? _shadowLookup.Values : _items;

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
