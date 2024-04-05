// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor;

#nullable enable
public class MudDragAndDropTransactionFinishedEventArgs<T> : EventArgs
{
    public T? Item { get; }

    public bool Success { get; }

    public string OriginatedDropzoneIdentifier { get; }

    public string DestinationDropzoneIdentifier { get; }

    public int OriginIndex { get; }

    public int DestinationIndex { get; }

    public MudDragAndDropTransactionFinishedEventArgs(string destinationDropZoneIdentifier, bool success, MudDragAndDropItemTransaction<T> transaction)
    {
        Item = transaction.Item;
        Success = success;
        OriginatedDropzoneIdentifier = transaction.SourceZoneIdentifier;
        DestinationDropzoneIdentifier = destinationDropZoneIdentifier;
        OriginIndex = transaction.SourceIndex;
        DestinationIndex = transaction.Index;
    }

    public MudDragAndDropTransactionFinishedEventArgs(MudDragAndDropItemTransaction<T> transaction) :
        this(string.Empty, false, transaction)
    {
    }
}
