// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace MudBlazor;

/// <summary>
/// Represents a visible row in a flattened <see cref="MudTreeView{T}"/>.
/// </summary>
/// <typeparam name="T">The type of value associated with each item.</typeparam>
internal sealed class TreeViewItemContext<T>
{
    /// <summary>
    /// The tree item being displayed.
    /// </summary>
    public ITreeItemData<T> Item { get; }

    /// <summary>
    /// The zero-based depth of this row in the tree.
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// The one-based position of this row among its visible siblings.
    /// </summary>
    public int PositionInSet { get; }

    /// <summary>
    /// The number of visible rows in this row's sibling set.
    /// </summary>
    public int SetSize { get; }

    /// <summary>
    /// Whether this row has any visible direct children.
    /// </summary>
    public bool HasVisibleChildren { get; }

    /// <summary>
    /// Whether this row or any of its descendants has a selectable value.
    /// </summary>
    public bool HasSelectableValues { get; }

    /// <summary>
    /// Whether this row's own selectable value is selected.
    /// </summary>
    public bool IsSelected { get; }

    /// <summary>
    /// The checkbox state derived from this row and all of its descendants.
    /// </summary>
    public bool? CheckState { get; }

    /// <summary>
    /// The key which identifies the rendered row across renders.
    /// </summary>
    /// <remarks>
    /// Rows are keyed by backing item reference so that a row component always stays bound to the same item.
    /// </remarks>
    public TreeViewRowKey<T> RowKey { get; }

    public TreeViewItemContext(
        ITreeItemData<T> item,
        int depth,
        int positionInSet,
        int setSize,
        bool hasVisibleChildren,
        bool hasSelectableValues,
        bool isSelected,
        bool? checkState,
        TreeViewRowKey<T> rowKey)
    {
        Item = item;
        Depth = depth;
        PositionInSet = positionInSet;
        SetSize = setSize;
        HasVisibleChildren = hasVisibleChildren;
        HasSelectableValues = hasSelectableValues;
        IsSelected = isSelected;
        CheckState = checkState;
        RowKey = rowKey;
    }
}

/// <summary>
/// Identifies a rendered row by backing item reference.
/// </summary>
/// <remarks>
/// <see cref="Ordinal"/> is zero for the first visible row of an item and increments for repeated references to the same instance, so that repeated references never produce duplicate render keys.
/// </remarks>
internal readonly record struct TreeViewRowKey<T>(ITreeItemData<T> Item, int Ordinal)
{
    public bool Equals(TreeViewRowKey<T> other) => ReferenceEquals(Item, other.Item) && Ordinal == other.Ordinal;

    public override int GetHashCode() => HashCode.Combine(RuntimeHelpers.GetHashCode(Item), Ordinal);
}
