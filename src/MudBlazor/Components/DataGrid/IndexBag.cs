// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Keeping correct index with virtualize component
/// </summary>
/// <remarks>
/// Until blazor virtualization component did not provide row index, we need to keep it
/// it can be remove when it'll be provided : https://github.com/dotnet/aspnetcore/issues/26943
/// </remarks>
internal readonly struct IndexBag<T>
{
    /// <summary>
    /// Virtualized row index
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// User item
    /// </summary>
    public T Item { get; }

    public IndexBag(int index, T item)
    {
        Index = index;
        Item = item;
    }
}
