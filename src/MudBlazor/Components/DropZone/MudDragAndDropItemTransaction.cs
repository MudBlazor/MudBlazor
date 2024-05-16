// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Used to encapsulate data for a drag and drop transaction
/// </summary>
/// <typeparam name="T"></typeparam>
public class MudDragAndDropItemTransaction<T>
{
    private readonly Func<Task> _commitCallback;
    private readonly Func<Task> _cancelCallback;

    /// <summary>
    /// The Item that is dragged during the transaction
    /// </summary>
    public T? Item { get; init; }

    /// <summary>
    /// The index of the item in the current drop zone
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// The index of the item when the transaction started
    /// </summary>
    public int SourceIndex { get; }

    /// <summary>
    /// Identifier for drop zone where the transaction started
    /// </summary>
    public string SourceZoneIdentifier { get; init; }

    public string CurrentZone { get; private set; }

    /// <summary>
    /// create a new instance of a drag and drop transaction encapsulating the item and source
    /// </summary>
    /// <param name="item">The item of this transaction</param>
    /// <param name="identifier">The identifier of the drop zone, where the transaction started</param>
    /// <param name="index">The source index</param>
    /// <param name="commitCallback">A callback that is invoked when the transaction has been successful</param>
    /// <param name="cancelCallback">A callback that is invoked when the transaction has been canceled</param>
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
    /// Cancel the transaction 
    /// </summary>
    /// <returns></returns>
    public Task Cancel() => _cancelCallback.Invoke();

    /// <summary>
    /// Commit this transaction as successful
    /// </summary>
    /// <returns></returns>
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
