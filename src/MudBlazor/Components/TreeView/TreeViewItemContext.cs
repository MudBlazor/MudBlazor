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
    public required ITreeItemData<T> Item { get; init; }

    /// <summary>
    /// The zero-based depth of this row in the tree.
    /// </summary>
    public required int Depth { get; init; }

    /// <summary>
    /// The one-based position of this row among its visible siblings.
    /// </summary>
    public required int PositionInSet { get; init; }

    /// <summary>
    /// The number of visible rows in this row's sibling set.
    /// </summary>
    public required int SetSize { get; init; }

    /// <summary>
    /// Whether this row has any visible direct children.
    /// </summary>
    public required bool HasVisibleChildren { get; init; }

    /// <summary>
    /// Whether this row or any of its descendants has a selectable value.
    /// </summary>
    public required bool HasSelectableValues { get; init; }

    /// <summary>
    /// Whether this row's own selectable value is selected.
    /// </summary>
    public required bool IsSelected { get; init; }

    /// <summary>
    /// The checkbox state derived from this row and all of its descendants.
    /// </summary>
    public required bool? CheckState { get; init; }

    /// <summary>
    /// The key which identifies the rendered row across renders.
    /// </summary>
    /// <remarks>
    /// Rows are keyed by backing item reference so that a row component always stays bound to the same item.
    /// </remarks>
    public required TreeViewRowKey<T> RowKey { get; init; }
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
