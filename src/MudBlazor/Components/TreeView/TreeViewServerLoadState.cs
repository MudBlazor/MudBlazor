// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace MudBlazor;

/// <summary>
/// The <see cref="MudTreeView{T}.ServerData"/> load state of one tree item.
/// </summary>
/// <remarks>
/// <see cref="Version"/> increments whenever a new load or reload is started, so a completion whose version no longer matches has been superseded and must be ignored.
/// </remarks>
internal sealed class TreeViewServerLoadEntry
{
    /// <summary>
    /// Whether children have been loaded successfully.
    /// </summary>
    public bool IsLoaded { get; set; }

    /// <summary>
    /// Whether a load is in progress.
    /// </summary>
    public bool IsLoading { get; set; }

    /// <summary>
    /// The generation of the most recent load request.
    /// </summary>
    public long Version { get; set; }

    /// <summary>
    /// Marks the entry as unloaded and supersedes any pending load.
    /// </summary>
    /// <returns>The reserved generation for the next load.</returns>
    public long Reset()
    {
        IsLoaded = false;
        IsLoading = false;
        return ++Version;
    }
}

/// <summary>
/// Tracks server-load state per backing item instance.
/// </summary>
/// <typeparam name="T">The type of value associated with each item.</typeparam>
/// <remarks>
/// State belongs to the backing item rather than to a rendered component, so it survives virtualization, mode switches and re-rendering, and disappears together with the item.
/// </remarks>
internal sealed class TreeViewServerLoadState<T>
{
    private readonly ConditionalWeakTable<ITreeItemData<T>, TreeViewServerLoadEntry> _entries = new();

    /// <summary>
    /// Returns the load entry for an item, creating it on first use.
    /// </summary>
    public TreeViewServerLoadEntry GetEntry(ITreeItemData<T> item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return _entries.GetOrCreateValue(item);
    }
}
