// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace MudBlazor;

#nullable enable

/// <summary>
/// The information related to a <see cref="MudDropZone{T}"/> drag-and-drop transaction.
/// </summary>
/// <typeparam name="T">The type of item being dragged and dropped.</typeparam>
public class MudDragAndDropItemTransaction<T>
{
    private readonly Func<Task> _commitCallback;
    private readonly Func<Task> _cancelCallback;

    /// <summary>
    /// The item being dragged.
    /// </summary>
    public T? Item { get; init; }

    /// <summary>
    /// The index of the item in the current drop zone.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// The index of the item when the transaction started.
    /// </summary>
    public int SourceIndex { get; }

    /// <summary>
    /// The unique ID of the zone where the transaction started.
    /// </summary>
    public string SourceZoneIdentifier { get; init; }

    /// <summary>
    /// The unique ID of the current destination zone.
    /// </summary>
    public string CurrentZone { get; private set; }

    /// <summary>
    /// Creates a new instance of a drag and drop transaction encapsulating the item and source
    /// </summary>
    /// <param name="item">The item being dragged.</param>
    /// <param name="identifier">The unique ID of the zone where the transaction started.</param>
    /// <param name="index">The index of the item when the transaction started.</param>
    /// <param name="commitCallback">Occurs when the item was successfully dropped.</param>
    /// <param name="cancelCallback">Occurs when the drag-and-drop operation was canceled.</param>
    public MudDragAndDropItemTransaction(T? item, string identifier, int index, Func<Task> commitCallback, Func<Task> cancelCallback)
    {
        Item = item;
        SourceZoneIdentifier = identifier;
        CurrentZone = identifier;
        Index = index;
        SourceIndex = index;

        _commitCallback = commitCallback;
        _cancelCallback = cancelCallback;
    }

    /// <summary>
    /// Cancels this transaction.
    /// </summary>
    public Task Cancel() => _cancelCallback.Invoke();

    /// <summary>
    /// Commits this transaction.
    /// </summary>
    public Task Commit() => _commitCallback.Invoke();

    internal bool UpdateIndex(int index)
    {
        if (Index == index)
        {
            return false;
        }

        Index = index;

        return true;
    }

    internal bool UpdateZone(string identifier)
    {
        if (CurrentZone == identifier)
        {
            return false;
        }

        CurrentZone = identifier;
        Index = -1;

        return true;
    }
}
