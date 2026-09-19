// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace MudBlazor;

/// <summary>
/// Flattens a backing <see cref="ITreeItemData{T}"/> hierarchy into the visible rows rendered by a virtualized <see cref="MudTreeView{T}"/>.
/// </summary>
/// <typeparam name="T">The type of value associated with each item.</typeparam>
/// <remarks>
/// <see cref="Rebuild"/> projects the whole hierarchy so that check-box state and selection can account for collapsed descendants, and records only expanded, visible items as <see cref="Rows"/>.
/// </remarks>
internal sealed class TreeViewProjection<T>
{
    private static readonly ReadOnlyCollection<TreeViewItemContext<T>> _emptyRows = new List<TreeViewItemContext<T>>().AsReadOnly();

    private readonly ConditionalWeakTable<ITreeItemData<T>, SelectionObservation> _selectionObservations = new();
    private IReadOnlyList<ProjectedNode> _roots = [];
    private List<ProjectedNode> _nodes = [];
    private Dictionary<TreeViewItemContext<T>, ProjectedNode> _rowNodes = new(ReferenceEqualityComparer.Instance);
    private Dictionary<TreeViewRowKey<T>, TreeViewItemContext<T>> _rowsByKey = [];
    private Dictionary<ValueKey, List<ProjectedNode>> _nodesByValue = [];
    private HashSet<T> _representedValues = [];
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
    /// <param name="selectedValues">The currently selected values.</param>
    /// <param name="comparer">The comparer used for values.</param>
    public void Rebuild(
        IReadOnlyCollection<ITreeItemData<T>>? items,
        IReadOnlyCollection<T>? selectedValues,
        IEqualityComparer<T?> comparer)
    {
        ArgumentNullException.ThrowIfNull(comparer);

        _comparer = comparer;
        _nodes = [];
        _rowNodes = new Dictionary<TreeViewItemContext<T>, ProjectedNode>(ReferenceEqualityComparer.Instance);
        _rowsByKey = [];
        _nodesByValue = new Dictionary<ValueKey, List<ProjectedNode>>(new ValueKeyEqualityComparer(comparer));
        _representedValues = new HashSet<T>(comparer);
        var selection = new HashSet<T>(selectedValues ?? [], comparer);
        var representedItems = new HashSet<ITreeItemData<T>>(ReferenceEqualityComparer.Instance);
        _roots = BuildNodes(items ?? [], null, selection, representedItems);

        var rows = new List<TreeViewItemContext<T>>();
        var rowOrdinals = new Dictionary<ITreeItemData<T>, int>(ReferenceEqualityComparer.Instance);
        AddVisibleRows(_roots, 0, rows, rowOrdinals);
        Rows = rows.AsReadOnly();
    }

    /// <summary>
    /// Finds the visible row with a render key.
    /// </summary>
    /// <returns>The row, or <c>null</c> when no visible row has the key.</returns>
    public TreeViewItemContext<T>? FindRow(TreeViewRowKey<T> rowKey)
    {
        return _rowsByKey.TryGetValue(rowKey, out var row) ? row : null;
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
        // Values without a backing item are kept, because a standard tree also keeps a bound value whose item has not been loaded yet.
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
            if (_selectionInitialized && hasPreviousState && previousState!.Selected == node.Item.Selected)
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
            backingItemsChanged = SetValueSelection(value, selected) || backingItemsChanged;
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
    /// <param name="selectedValues">The current selected values.</param>
    /// <param name="autoSelectParent">Whether ancestor selection follows direct children.</param>
    /// <returns>The updated selected values.</returns>
    public IReadOnlyCollection<T> ToggleSubtreeSelection(
        TreeViewItemContext<T> row,
        IReadOnlyCollection<T> selectedValues,
        bool autoSelectParent)
    {
        ArgumentNullException.ThrowIfNull(row);

        if (!_rowNodes.TryGetValue(row, out var node))
        {
            return selectedValues.Where(_representedValues.Contains).ToHashSet(_comparer).ToList().AsReadOnly();
        }

        var selection = new HashSet<T>(selectedValues.Where(_representedValues.Contains), _comparer);
        var subtreeValues = new List<T>();
        AddSubtreeValues(node, subtreeValues);
        var allSelected = subtreeValues.Count > 0 && subtreeValues.All(selection.Contains);

        foreach (var value in subtreeValues)
        {
            if (allSelected)
            {
                selection.Remove(value);
            }
            else
            {
                selection.Add(value);
            }

            SetValueSelection(value, !allSelected);
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
                if (parentSelected)
                {
                    selection.Add(parent.Value);
                }
                else
                {
                    selection.Remove(parent.Value);
                }

                SetValueSelection(parent.Value, parentSelected);
            }
        }

        return selection.ToList().AsReadOnly();

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
    /// Expands the expandable ancestors of every selected item in the projected hierarchy.
    /// </summary>
    /// <returns>Whether any expansion changed.</returns>
    public bool AutoExpand(IReadOnlyCollection<T> selectedValues)
    {
        var selection = new HashSet<T>(selectedValues, _comparer);
        var changed = false;
        foreach (var root in _roots)
        {
            ExpandSelectedAncestors(root, selection, ref changed);
        }

        return changed;

        static bool ExpandSelectedAncestors(ProjectedNode node, HashSet<T> selection, ref bool changed)
        {
            var childContainsSelection = false;
            foreach (var child in node.Children)
            {
                childContainsSelection = ExpandSelectedAncestors(child, selection, ref changed) || childContainsSelection;
            }

            if (childContainsSelection && node.Item.Expandable && !node.Item.Expanded)
            {
                node.Item.Expanded = true;
                changed = true;
            }

            return childContainsSelection || (node.Value is not null && selection.Contains(node.Value));
        }
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
            var node = new ProjectedNode(item, value, parent);
            if (representedItems.Add(item))
            {
                _nodes.Add(node);
                if (value is not null)
                {
                    _representedValues.Add(value);
                    var key = new ValueKey(value);
                    if (!_nodesByValue.TryGetValue(key, out var valueNodes))
                    {
                        valueNodes = [];
                        _nodesByValue.Add(key, valueNodes);
                    }

                    valueNodes.Add(node);
                }
            }

            var children = item.Children is { Count: > 0 }
                ? BuildNodes(item.Children, node, selection, representedItems)
                : [];
            node.Children = children;
            var isSelected = value is not null && selection.Contains(value);
            var hasSelectableValues = value is not null;
            var hasSelectedValues = isSelected;
            var hasUnselectedValues = value is not null && !isSelected;

            foreach (var child in children)
            {
                hasSelectableValues |= child.HasSelectableValues;
                hasSelectedValues |= child.HasSelectedValues;
                hasUnselectedValues |= child.HasUnselectedValues;
            }

            bool? checkState = null;
            if (hasSelectableValues && hasSelectedValues != hasUnselectedValues)
            {
                checkState = hasSelectedValues;
            }

            node.IsSelected = isSelected;
            node.HasSelectableValues = hasSelectableValues;
            node.HasSelectedValues = hasSelectedValues;
            node.HasUnselectedValues = hasUnselectedValues;
            node.CheckState = checkState;
            nodes.Add(node);
        }

        return nodes;
    }

    private void AddVisibleRows(
        IReadOnlyList<ProjectedNode> nodes,
        int depth,
        List<TreeViewItemContext<T>> rows,
        Dictionary<ITreeItemData<T>, int> rowOrdinals)
    {
        var visibleNodes = nodes.Where(node => node.Item.Visible).ToList();
        for (var index = 0; index < visibleNodes.Count; index++)
        {
            var node = visibleNodes[index];
            rowOrdinals.TryGetValue(node.Item, out var ordinal);
            rowOrdinals[node.Item] = ordinal + 1;
            var row = new TreeViewItemContext<T>(
                node.Item,
                depth,
                index + 1,
                visibleNodes.Count,
                node.Children.Any(child => child.Item.Visible),
                node.HasSelectableValues,
                node.IsSelected,
                node.CheckState,
                new TreeViewRowKey<T>(node.Item, ordinal));
            rows.Add(row);
            _rowNodes.Add(row, node);
            _rowsByKey.Add(row.RowKey, row);

            if (node.Item.Expanded)
            {
                AddVisibleRows(node.Children, depth + 1, rows, rowOrdinals);
            }
        }
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

    private bool SetValueSelection(T value, bool selected)
    {
        if (!_nodesByValue.TryGetValue(new ValueKey(value), out var nodes))
        {
            return false;
        }

        var changed = false;
        foreach (var node in nodes)
        {
            if (node.Item.Selected != selected)
            {
                node.Item.Selected = selected;
                changed = true;
            }

            _selectionObservations.GetOrCreateValue(node.Item).Selected = selected;
        }

        return changed;
    }

    private sealed class ProjectedNode(ITreeItemData<T> item, T? value, ProjectedNode? parent)
    {
        public ITreeItemData<T> Item { get; } = item;

        public T? Value { get; } = value;

        public ProjectedNode? Parent { get; } = parent;

        public IReadOnlyList<ProjectedNode> Children { get; set; } = [];

        public bool IsSelected { get; set; }

        public bool HasSelectableValues { get; set; }

        public bool HasSelectedValues { get; set; }

        public bool HasUnselectedValues { get; set; }

        public bool? CheckState { get; set; }
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
