// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Represents a visible item in a flattened <see cref="MudTreeView{T}"/>.
/// </summary>
/// <typeparam name="T">The type of value associated with each item.</typeparam>
public sealed class TreeViewItemContext<T>
{
    /// <summary>
    /// The tree item being displayed.
    /// </summary>
    public ITreeItemData<T> Item { get; }

    /// <summary>
    /// The zero-based depth of this item in the tree.
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeViewItemContext{T}"/> class.
    /// </summary>
    /// <param name="item">The tree item being displayed.</param>
    /// <param name="depth">The zero-based depth of this item in the tree.</param>
    public TreeViewItemContext(ITreeItemData<T> item, int depth)
    {
        Item = item;
        Depth = depth;
    }
}
