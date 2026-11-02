// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace MudBlazor;


/// <summary>
/// Provides the contract for data that can be rendered inside a <see cref="MudTreeView{T}"/>.
/// </summary>
/// <typeparam name="T">The type of value associated with each item.</typeparam>
public interface ITreeItemData<T>
{
    /// <summary>
    /// The text of this item.
    /// </summary>
    string? Text { get; set; }

    /// <summary>
    /// The icon for this item.
    /// </summary>
    string? Icon { get; set; }

    /// <summary>
    /// The value associated with this item.
    /// </summary>
    T? Value { get; }

    /// <summary>
    /// Whether this item is expanded.
    /// </summary>
    bool Expanded { get; set; }

    /// <summary>
    /// Whether this item can be expanded.
    /// </summary>
    bool Expandable { get; set; }

    /// <summary>
    /// Whether this item is selected.
    /// </summary>
    bool Selected { get; set; }

    /// <summary>
    /// Whether this item is displayed.
    /// </summary>
    bool Visible { get; set; }

    /// <summary>
    /// The child items underneath this item.
    /// </summary>
    IReadOnlyCollection<ITreeItemData<T>>? Children { get; set; }

    /// <summary>
    /// Whether this item contains child items.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Children))]
    bool HasChildren { get; }
}
