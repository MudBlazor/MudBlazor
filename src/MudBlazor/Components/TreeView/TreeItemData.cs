// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor;

#nullable enable

/// <summary>
/// The current state of a <see cref="MudTreeViewItem{T}"/>.
/// </summary>
/// <typeparam name="T">The type of item to display.</typeparam>
public class TreeItemData<T> : IEquatable<TreeItemData<T>>
{
    public TreeItemData() : this(default) { }

    protected TreeItemData(T? value)
    {
        Value = value;
    }

    /// <summary>
    /// The text of this item.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    public virtual string? Text { get; set; }

    /// <summary>
    /// The icon for this item.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    public virtual string? Icon { get; set; }

    /// <summary>
    /// The value associated with this item.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    public T? Value { get; init; }

    /// <summary>
    /// Whether this item is expanded.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public virtual bool Expanded { get; set; }

    /// <summary>
    /// Whether this item can be expanded.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public virtual bool Expandable { get; set; } = true;

    /// <summary>
    /// Whether this item is selected.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public virtual bool Selected { get; set; }

    /// <summary>
    /// Whether this item is displayed.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public virtual bool Visible { get; set; } = true;

    /// <summary>
    /// The child items underneath this item.
    /// </summary>
    public virtual List<TreeItemData<T>>? Children { get; set; }

    /// <summary>
    /// Whether this item contains child items.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Children))]
    public virtual bool HasChildren => Children is not null && Children.Count > 0;

    /// <summary>
    /// Whether this item is the same as the specified item.
    /// </summary>
    /// <param name="other">The item to compare.</param>
    /// <returns>When <c>true</c>, the items are equivalent.</returns>
    public virtual bool Equals(TreeItemData<T>? other)
    {
        if (ReferenceEquals(null, other))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EqualityComparer<T>.Default.Equals(Value, other.Value);
    }

    /// <summary>
    /// Whether this item is the same as the specified item.
    /// </summary>
    /// <param name="obj">The object to compare.</param>
    /// <returns>When <c>true</c>, the items are equivalent.</returns>
    public override bool Equals(object? obj) => obj is TreeItemData<T> treeItemData && Equals(treeItemData);

    /// <summary>
    /// The unique hash code for this item.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        if (Value is null)
        {
            return 0;
        }

        return EqualityComparer<T>.Default.GetHashCode(Value);
    }
}
