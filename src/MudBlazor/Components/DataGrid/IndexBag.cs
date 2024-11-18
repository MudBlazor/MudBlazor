// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a container that holds an item and its corresponding index in a virtualized list.
/// </summary>
/// <typeparam name="T">The type of the item.</typeparam>
/// <remarks>
/// This struct is used to maintain the correct index of items in a virtualized component.
/// It can be removed once the Blazor virtualization component provides row index support.
/// See: <see href="https://github.com/dotnet/aspnetcore/issues/26943"/>
/// </remarks>
internal readonly struct IndexBag<T>
{
    /// <summary>
    /// Gets the virtualized row index.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// Gets the user item.
    /// </summary>
    public T Item { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexBag{T}"/> struct.
    /// </summary>
    /// <param name="index">The virtualized row index.</param>
    /// <param name="item">The user item.</param>
    public IndexBag(int index, T item)
    {
        Index = index;
        Item = item;
    }
}
