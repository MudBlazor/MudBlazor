// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MudBlazor;

/// <summary>
/// Flattens a backing <see cref="ITreeItemData{T}"/> hierarchy into the visible rows rendered by a virtualized <see cref="MudTreeView{T}"/>.
/// </summary>
/// <typeparam name="T">The type of value associated with each item.</typeparam>
/// <remarks>
/// <see cref="Rebuild"/> projects the whole hierarchy so that check-box state and selection can account for collapsed descendants, and records only expanded, visible items as <see cref="Rows"/>.
/// Selection changes are applied to the projected nodes in place through <see cref="SetSelected"/>, so that selecting an item costs time proportional to the items it affects rather than to the whole hierarchy.
/// </remarks>
internal sealed class TreeViewProjection<T>
{
    private static readonly ReadOnlyCollection<TreeViewItemContext<T>> _emptyRows = new List<TreeViewItemContext<T>>().AsReadOnly();

    private readonly ConditionalWeakTable<ITreeItemData<T>, SelectionObservation> _selectionObservations = new();
    private IReadOnlyList<ProjectedNode> _roots = [];
    private List<ProjectedNode> _nodes = [];
    private Dictionary<TreeViewRowKey<T>, ProjectedNode> _rowNodes = [];
    // The first projected node of each value; further nodes with the same value are linked through ProjectedNode.NextWithSameValue.
    private Dictionary<ValueKey, ProjectedNode> _nodesByValue = [];
    private HashSet<ITreeItemData<T>>? _repeatedItems;
    private HashSet<T> _representedValues = [];
    private HashSet<T> _selectedValues = [];
    private IEqualityComparer<T?> _comparer = EqualityComparer<T?>.Default;
    private bool _selectionInitialized;

    /// <summary>
    /// The current flattened visible rows.
    /// </summary>
    public ReadOnlyCollection<TreeViewItemContext<T>> Rows { get; private set; } = _emptyRows;

    /// <summary>
    /// Rebuilds the visible rows and selection projection from the backing hierarchy.
    /// </summary>
    /// <param name="items">The root backing items.</param>
    /// <param name="selectedValues">The selected values, including values whose item is not in the hierarchy.</param>
    /// <param name="comparer">The comparer used for values.</param>
    public void Rebuild(
        IReadOnlyCollection<ITreeItemData<T>>? items,
        IReadOnlyCollection<T>? selectedValues,
        IEqualityComparer<T?> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        _comparer = comparer;
        _nodes = [];
        _rowNodes = [];
        _nodesByValue = new Dictionary<ValueKey, ProjectedNode>(new ValueKeyEqualityComparer(comparer));
        _repeatedItems = null;
        _representedValues = new HashSet<T>(comparer);
        // Values whose item is missing are kept, so that an item which leaves the hierarchy and returns is projected as selected again.
        _selectedValues = new HashSet<T>(selectedValues ?? [], comparer);
        var representedItems = new HashSet<ITreeItemData<T>>(ReferenceEqualityComparer.Instance);
        _roots = BuildNodes(items ?? [], null, _selectedValues, representedItems);

        var rows = new List<TreeViewItemContext<T>>();
        var rowOrdinals = _repeatedItems is null ? null : new Dictionary<ITreeItemData<T>, int>(ReferenceEqualityComparer.Instance);
        AddVisibleRows(_roots, 0, rows, rowOrdinals);
        Rows = rows.AsReadOnly();
    }

    /// <summary>
    /// Whether the backing hierarchy differs from what was last projected.
    /// </summary>
    /// <remarks>
    /// The application can change the backing items in place, so a parent render compares them with what was projected instead of rebuilding every time.
    /// The walk reads each item once and allocates nothing for list-backed children, which keeps it far cheaper than a rebuild.
    /// Children are compared by count and element identity rather than collection identity, because an implementation can return a new wrapper on every read.
    /// </remarks>
    /// <param name="items">The root backing items.</param>
    public bool HasChanged(IReadOnlyCollection<ITreeItemData<T>>? items) => !Matches(items, _roots);

    /// <summary>
    /// Whether an item in the projected hierarchy has a value.
    /// </summary>
    public bool Represents(T value) => _representedValues.Contains(value);

    /// <summary>
    /// The values which the projection last applied as selected, including values whose item is not in the hierarchy.
    /// </summary>
    public IReadOnlyCollection<T> SelectedValues => _selectedValues;

    /// <summary>
    /// Whether the projected items with a value are selected.
    /// </summary>
    public bool IsSelected(T value) => _selectedValues.Contains(value);

    /// <summary>
    /// Changes the projected selection to the specified values by applying only the difference.
    /// </summary>
    /// <param name="selectedValues">The values which should be selected; values without an item are ignored.</param>
    /// <returns>The values which became selected.</returns>
    public IReadOnlyList<T> ApplySelection(IReadOnlyCollection<T> selectedValues)
    {
        var target = selectedValues as HashSet<T> ?? new HashSet<T>(selectedValues, _comparer);
        foreach (var value in _selectedValues.Where(value => !target.Contains(value)).ToList())
        {
            SetSelected(value, false);
        }

        var added = target.Where(value => !_selectedValues.Contains(value)).ToList();
        foreach (var value in added)
        {
            SetSelected(value, true);
        }

        return added.Where(_representedValues.Contains).ToList();
    }

    /// <summary>
    /// Finds the visible row with a render key.
    /// </summary>
    /// <returns>The row, or <c>null</c> when no visible row has the key.</returns>
    public TreeViewItemContext<T>? FindRow(TreeViewRowKey<T> rowKey)
    {
        return _rowNodes.TryGetValue(rowKey, out var node) ? node.Row : null;
    }

    /// <summary>
    /// Reconciles externally changed backing selection with the selected-value state.
    /// </summary>
    /// <param name="multiSelection">Whether multiple values may be selected.</param>
    /// <param name="selectedValue">The current single selected value.</param>
    /// <param name="selectedValues">The current multiple selected values.</param>
    /// <returns>The reconciled selected-value state and change flags.</returns>
    public TreeViewSelectionResult<T> ReconcileSelection(
        bool multiSelection,
        T? selectedValue,
        IReadOnlyCollection<T>? selectedValues)
    {
        var currentSelection = new HashSet<T>(selectedValues ?? [], _comparer);
        // Values without a backing item are kept here, so that a bound value whose item has not been loaded yet survives a re-projection.
        // The next checkbox click still drops them, as it does in a standard tree.
        var reconciledSelection = new HashSet<T>(currentSelection, _comparer);
        var reconciledValue = selectedValue;
        var touchedValues = new HashSet<T>(_comparer);

        foreach (var node in _nodes)
        {
            if (node.Value is null)
            {
                continue;
            }

            var hasPreviousState = _selectionObservations.TryGetValue(node.Item, out var previousState);
            // A projected state which disagrees with the backing item has to be settled even when the item kept the state last written to it.
            if (_selectionInitialized && hasPreviousState && previousState!.Selected == node.Item.Selected && node.IsSelected == node.Item.Selected)
            {
                continue;
            }

            touchedValues.Add(node.Value);
            if (multiSelection)
            {
                if (node.Item.Selected)
                {
                    reconciledSelection.Add(node.Value);
                }
                else if (hasPreviousState)
                {
                    reconciledSelection.Remove(node.Value);
                }
            }
            else if (node.Item.Selected)
            {
                reconciledValue = node.Value;
            }
            else if (hasPreviousState && _comparer.Equals(reconciledValue, node.Value))
            {
                reconciledValue = default;
            }
        }

        _selectionInitialized = true;
        var backingItemsChanged = false;
        foreach (var value in touchedValues)
        {
            var selected = multiSelection
                ? reconciledSelection.Contains(value)
                : _comparer.Equals(reconciledValue, value);
            backingItemsChanged = SetSelected(value, selected) || backingItemsChanged;
        }

        var selectionChanged = multiSelection
            ? !currentSelection.SetEquals(reconciledSelection)
            : !_comparer.Equals(selectedValue, reconciledValue);

        return new TreeViewSelectionResult<T>(
            reconciledValue,
            reconciledSelection.ToList().AsReadOnly(),
            selectionChanged,
            backingItemsChanged);
    }

    /// <summary>
    /// Traverses the backing hierarchy while synchronizing every item's <see cref="ITreeItemData{T}.Selected"/> with the selected values.
    /// </summary>
    /// <param name="items">The root backing items.</param>
    /// <param name="selectedValues">The selected values.</param>
    /// <param name="comparer">The comparer used for values.</param>
    /// <returns>The selected values which are represented by the hierarchy.</returns>
    public IReadOnlyCollection<T> SynchronizeSelection(
        IReadOnlyCollection<ITreeItemData<T>>? items,
        IReadOnlyCollection<T> selectedValues,
        IEqualityComparer<T?> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        var selection = new HashSet<T>(selectedValues, comparer);
        var representedValues = new HashSet<T>(comparer);
        var visited = new HashSet<ITreeItemData<T>>(ReferenceEqualityComparer.Instance);
        SynchronizeSelection(items ?? [], selection, representedValues, visited);
        _selectionInitialized = true;
        return selectedValues.Where(representedValues.Contains).ToHashSet(comparer).ToList().AsReadOnly();
    }

    /// <summary>
    /// Toggles the selectable values of a row's subtree and optionally updates its ancestors.
    /// </summary>
    /// <param name="row">The row at the root of the subtree.</param>
    /// <param name="selection">The selected values, which are updated in place.</param>
    /// <param name="autoSelectParent">Whether ancestor selection follows direct children.</param>
    /// <returns>The values which became selected.</returns>
    public IReadOnlyList<T> ToggleSubtreeSelection(
        TreeViewItemContext<T> row,
        HashSet<T> selection,
        bool autoSelectParent)
    {
        ArgumentNullException.ThrowIfNull(row);
        ArgumentNullException.ThrowIfNull(selection);

        // A click drops values whose item is missing, as a standard tree does, so the projection must forget them too or they would return selected with their item.
        foreach (var value in selection.Where(value => !_representedValues.Contains(value)).ToList())
        {
            selection.Remove(value);
            SetSelected(value, false);
        }

        var newlySelected = new List<T>();
        if (!_rowNodes.TryGetValue(row.RowKey, out var node) || node.Row != row)
        {
            return newlySelected;
        }

        var subtreeValues = new List<T>();
        AddSubtreeValues(node, subtreeValues);
        var allSelected = subtreeValues.Count > 0 && subtreeValues.All(selection.Contains);

        foreach (var value in subtreeValues)
        {
            Select(value, !allSelected);
        }

        if (autoSelectParent)
        {
            for (var parent = node.Parent; parent is not null; parent = parent.Parent)
            {
                if (parent.Value is null)
                {
                    continue;
                }

                var parentSelected = parent.Children
                    .Where(child => child.Value is not null)
                    .All(child => selection.Contains(child.Value!));
                Select(parent.Value, parentSelected);
            }
        }

        return newlySelected;

        void Select(T value, bool selected)
        {
            if (selected ? selection.Add(value) : selection.Remove(value))
            {
                if (selected)
                {
                    newlySelected.Add(value);
                }
            }

            SetSelected(value, selected);
        }

        static void AddSubtreeValues(ProjectedNode node, List<T> values)
        {
            if (node.Value is not null)
            {
                values.Add(node.Value);
            }

            foreach (var child in node.Children)
            {
                AddSubtreeValues(child, values);
            }
        }
    }

    /// <summary>
    /// Expands the expandable ancestors of every projected item with one of the specified values.
    /// </summary>
    /// <remarks>
    /// Only the ancestors of the specified values are touched, so a branch the user collapsed stays collapsed when an unrelated value is selected.
    /// Every occurrence of a repeated item is followed, so each branch which contains it expands.
    /// </remarks>
    /// <returns>Whether any expansion changed.</returns>
    public bool ExpandAncestors(IEnumerable<T> values)
    {
        var changed = false;
        var visited = new HashSet<ProjectedNode>(ReferenceEqualityComparer.Instance);
        foreach (var value in values)
        {
            if (value is null || !_nodesByValue.TryGetValue(new ValueKey(value), out var firstNode))
            {
                continue;
            }

            for (var node = firstNode; node is not null; node = node.NextWithSameValue)
            {
                // An ancestor which was already visited had its own ancestors handled at the same time.
                for (var parent = node.Parent; parent is not null && visited.Add(parent); parent = parent.Parent)
                {
                    if (parent.Item.Expandable && !parent.Item.Expanded)
                    {
                        parent.Item.Expanded = true;
                        changed = true;
                    }
                }
            }
        }

        return changed;
    }

    /// <summary>
    /// Selects or deselects every projected item with a value, updating the backing items and the check-box state of their ancestors.
    /// </summary>
    /// <returns>Whether any backing item changed.</returns>
    public bool SetSelected(T value, bool selected)
    {
        if (selected)
        {
            _selectedValues.Add(value);
        }
        else
        {
            _selectedValues.Remove(value);
        }

        if (!_nodesByValue.TryGetValue(new ValueKey(value), out var firstNode))
        {
            return false;
        }

        var changed = false;
        for (var node = firstNode; node is not null; node = node.NextWithSameValue)
        {
            if (node.Item.Selected != selected)
            {
                node.Item.Selected = selected;
                changed = true;
            }

            _selectionObservations.GetOrCreateValue(node.Item).Selected = selected;
            if (node.IsSelected == selected)
            {
                continue;
            }

            node.IsSelected = selected;
            if (node.Row is not null)
            {
                node.Row.IsSelected = selected;
            }

            var delta = selected ? 1 : -1;
            for (var current = node; current is not null; current = current.Parent)
            {
                current.SelectedCount += delta;
                if (current.Row is not null)
                {
                    current.Row.CheckState = current.CheckState;
                }
            }
        }

        return changed;
    }

    private List<ProjectedNode> BuildNodes(
        IReadOnlyCollection<ITreeItemData<T>> items,
        ProjectedNode? parent,
        HashSet<T> selection,
        HashSet<ITreeItemData<T>> representedItems)
    {
        var nodes = new List<ProjectedNode>(items.Count);
        foreach (var item in items)
        {
            var value = TreeViewHierarchy<T>.GetItemValue(item);
            var node = new ProjectedNode(item, value, parent)
            {
                Visible = item.Visible,
                Expanded = item.Expanded,
                Expandable = item.Expandable
            };
            if (representedItems.Add(item))
            {
                _nodes.Add(node);
            }
            else
            {
                _repeatedItems ??= new HashSet<ITreeItemData<T>>(ReferenceEqualityComparer.Instance);
                _repeatedItems.Add(item);
            }

            // Every occurrence of a repeated item is registered, because each one contributes to the check-box state of its own ancestors.
            if (value is not null)
            {
                _representedValues.Add(value);
                ref var firstNode = ref CollectionsMarshal.GetValueRefOrAddDefault(_nodesByValue, new ValueKey(value), out _);
                node.NextWithSameValue = firstNode;
                firstNode = node;
            }

            var children = item.Children is { Count: > 0 }
                ? BuildNodes(item.Children, node, selection, representedItems)
                : [];
            node.Children = children;
            var isSelected = value is not null && selection.Contains(value);
            var selectableCount = value is not null ? 1 : 0;
            var selectedCount = isSelected ? 1 : 0;

            foreach (var child in children)
            {
                selectableCount += child.SelectableCount;
                selectedCount += child.SelectedCount;
            }

            node.IsSelected = isSelected;
            node.SelectableCount = selectableCount;
            node.SelectedCount = selectedCount;
            nodes.Add(node);
        }

        return nodes;
    }

    private void AddVisibleRows(
        IReadOnlyList<ProjectedNode> nodes,
        int depth,
        List<TreeViewItemContext<T>> rows,
        Dictionary<ITreeItemData<T>, int>? rowOrdinals)
    {
        // A rebuild runs over every row, so this avoids allocating per sibling group.
        var setSize = 0;
        foreach (var node in nodes)
        {
            if (node.Item.Visible)
            {
                setSize++;
            }
        }

        var position = 0;
        foreach (var node in nodes)
        {
            if (!node.Item.Visible)
            {
                continue;
            }

            // Only repeated references need an ordinal to keep their render keys distinct.
            var ordinal = 0;
            if (rowOrdinals is not null && _repeatedItems!.Contains(node.Item))
            {
                rowOrdinals.TryGetValue(node.Item, out ordinal);
                rowOrdinals[node.Item] = ordinal + 1;
            }

            var hasVisibleChildren = false;
            foreach (var child in node.Children)
            {
                if (child.Item.Visible)
                {
                    hasVisibleChildren = true;
                    break;
                }
            }

            var row = new TreeViewItemContext<T>
            {
                Item = node.Item,
                Depth = depth,
                PositionInSet = ++position,
                SetSize = setSize,
                HasVisibleChildren = hasVisibleChildren,
                HasSelectableValues = node.SelectableCount > 0,
                IsSelected = node.IsSelected,
                CheckState = node.CheckState,
                RowKey = new TreeViewRowKey<T>(node.Item, ordinal)
            };
            node.Row = row;
            rows.Add(row);
            _rowNodes.Add(row.RowKey, node);

            if (node.Item.Expanded)
            {
                AddVisibleRows(node.Children, depth + 1, rows, rowOrdinals);
            }
        }
    }

    private bool Matches(IReadOnlyCollection<ITreeItemData<T>>? items, IReadOnlyList<ProjectedNode> nodes)
    {
        if ((items?.Count ?? 0) != nodes.Count)
        {
            return false;
        }

        if (nodes.Count == 0)
        {
            return true;
        }

        // Enumerating through the interface would allocate an enumerator for every item with children.
        if (items is IReadOnlyList<ITreeItemData<T>> list)
        {
            for (var index = 0; index < nodes.Count; index++)
            {
                if (!ReferenceEquals(list[index], nodes[index].Item) || !Matches(nodes[index]))
                {
                    return false;
                }
            }

            return true;
        }

        var position = 0;
        foreach (var item in items!)
        {
            if (position == nodes.Count || !ReferenceEquals(item, nodes[position].Item) || !Matches(nodes[position]))
            {
                return false;
            }

            position++;
        }

        return position == nodes.Count;
    }

    private bool Matches(ProjectedNode node)
    {
        var item = node.Item;
        if (item.Visible != node.Visible || item.Expanded != node.Expanded || item.Expandable != node.Expandable)
        {
            return false;
        }

        var value = TreeViewHierarchy<T>.GetItemValue(item);
        if (value is null || node.Value is null ? value is not null || node.Value is not null : !_comparer.Equals(value, node.Value))
        {
            return false;
        }

        // An item without a value can never be selected, so its flag is not part of the selection.
        if (value is not null && item.Selected != node.IsSelected)
        {
            return false;
        }

        return Matches(item.Children, node.Children);
    }

    private void SynchronizeSelection(
        IReadOnlyCollection<ITreeItemData<T>> items,
        HashSet<T> selection,
        HashSet<T> representedValues,
        HashSet<ITreeItemData<T>> visited)
    {
        foreach (var item in items)
        {
            if (!visited.Add(item))
            {
                continue;
            }

            var value = TreeViewHierarchy<T>.GetItemValue(item);
            var selected = value is not null && selection.Contains(value);
            if (value is not null)
            {
                representedValues.Add(value);
            }

            if (item.Selected != selected)
            {
                item.Selected = selected;
            }

            _selectionObservations.GetOrCreateValue(item).Selected = selected;
            if (item.Children is { Count: > 0 })
            {
                SynchronizeSelection(item.Children, selection, representedValues, visited);
            }
        }
    }

    private sealed class ProjectedNode(ITreeItemData<T> item, T? value, ProjectedNode? parent)
    {
        public ITreeItemData<T> Item { get; } = item;

        public T? Value { get; } = value;

        public ProjectedNode? Parent { get; } = parent;

        public IReadOnlyList<ProjectedNode> Children { get; set; } = [];

        public bool IsSelected { get; set; }

        public bool Visible { get; init; }

        public bool Expanded { get; init; }

        public bool Expandable { get; init; }

        // The number of items with a value in this subtree, including this item.
        public int SelectableCount { get; set; }

        // The number of selected items in this subtree, including this item.
        public int SelectedCount { get; set; }

        public TreeViewItemContext<T>? Row { get; set; }

        public ProjectedNode? NextWithSameValue { get; set; }

        public bool? CheckState => SelectableCount == 0 || (SelectedCount > 0 && SelectedCount < SelectableCount)
            ? null
            : SelectedCount > 0;
    }

    private sealed class SelectionObservation
    {
        public bool Selected { get; set; }
    }

    private readonly record struct ValueKey(T Value);

    private sealed class ValueKeyEqualityComparer(IEqualityComparer<T?> comparer) : IEqualityComparer<ValueKey>
    {
        public bool Equals(ValueKey x, ValueKey y) => comparer.Equals(x.Value, y.Value);

        public int GetHashCode(ValueKey obj) => obj.Value is null ? 0 : comparer.GetHashCode(obj.Value);
    }
}

/// <summary>
/// Describes selected-value state reconciled from backing items.
/// </summary>
/// <typeparam name="T">The type of value associated with each item.</typeparam>
internal readonly record struct TreeViewSelectionResult<T>(
    T? SelectedValue,
    IReadOnlyCollection<T> SelectedValues,
    bool SelectionChanged,
    bool BackingItemsChanged)
{
    /// <summary>
    /// Whether selected-value state or backing selection changed.
    /// </summary>
    public bool Changed => SelectionChanged || BackingItemsChanged;
}
