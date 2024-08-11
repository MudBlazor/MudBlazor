// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{

#nullable enable

    /// <summary>
    /// A container of <see cref="MudDropZone{T}"/> components for drag-and-drop operations.
    /// </summary>
    /// <typeparam name="T">The type of item dragged and dropped within this container.</typeparam>
    public partial class MudDropContainer<T> : MudComponentBase where T : notnull
    {
        private MudDragAndDropItemTransaction<T>? _transaction;
        private Dictionary<string, MudDropZone<T>> _mudDropZones = new();

        protected string Classname =>
        new CssBuilder("mud-drop-container")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// The content within this container.
        /// </summary>
        /// <remarks>
        /// The content should include at least two <see cref="MudDropZone{T}"/> components.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The items that can be dragged and dropped within this container.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// The template used to render items within a drop zone.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public RenderFragment<T>? ItemRenderer { get; set; }

        /// <summary>
        /// The function which determines whether an item can be dropped within a drop zone.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public Func<T, string, bool>? ItemsSelector { get; set; }

        /// <summary>
        /// Occurs when an item has been dropped into a <see cref="MudDropZone{T}"/>.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public EventCallback<MudItemDropInfo<T>> ItemDropped { get; set; }

        /// <summary>
        /// Occurs when an item starts being dragged.
        /// </summary>
        /// <remarks>
        /// A new <see cref="MudDragAndDropItemTransaction{T}"/> is started which tracks the drag-and-drop operation until it is completed or canceled.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public EventCallback<MudDragAndDropItemTransaction<T>> ItemPicked { get; set; }

        /// <summary>
        /// The function which determines whether an item can be dropped within a drop zone.
        /// </summary>
        /// <remarks>
        /// When a drop zone is allowed, the <see cref="CanDropClass"/> is applied, otherwise <see cref="NoDropClass"/> is applied.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public Func<T, string, bool>? CanDrop { get; set; }

        /// <summary>
        /// The CSS classes applied to valid drop zones.
        /// </summary>
        /// <remarks>
        /// This class is applied when <see cref="CanDrop"/> returns <c>true</c> for an item.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string? CanDropClass { get; set; }

        /// <summary>
        /// The CSS classes applied to invalid drop zones.
        /// </summary>
        /// <remarks>
        /// This class is applied when <see cref="CanDrop"/> returns <c>false</c> for an item.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string? NoDropClass { get; set; }

        /// <summary>
        /// Applies either <see cref="CanDropClass"/> or <see cref="NoDropClass"/> to drop zones during a drag-and-drop transaction.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  The <see cref="CanDrop"/> function determines which classes are applied.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public bool ApplyDropClassesOnDragStarted { get; set; } = false;

        /// <summary>
        /// The function which determines whether an item cannot be dragged.
        /// </summary>
        /// <remarks>
        /// If no value is given, all items can be dragged by default.  When an item is disabled, the <see cref="DisabledClass"/> is applied.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Disabled)]
        public Func<T, bool>? ItemDisabled { get; set; }

        /// <summary>
        /// The CSS classes applied to disabled drop items.
        /// </summary>
        /// <remarks>
        /// This class is applied when <see cref="ItemDisabled"/> returns <c>true</c> for an item.  Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.Disabled)]
        public string DisabledClass { get; set; } = "disabled";

        /// <summary>
        /// The CSS classes applied to drop zones during a drag-and-drop operation.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DraggingClass)]
        public string? DraggingClass { get; set; }

        /// <summary>
        /// The CSS classes applied to items during a drag-and-drop operation.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.DropZone.DraggingClass)]
        public string? ItemDraggingClass { get; set; }

        /// <summary>
        /// The function which determines the CSS classes for each item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public Func<T, string, string>? ItemsClassSelector { get; set; }

        /// <summary>
        /// Occurs when a new drag-and-drop operation has started.
        /// </summary>
        public event EventHandler<MudDragAndDropItemTransaction<T>>? TransactionStarted;

        /// <summary>
        /// Occurs when an ongoing drop-and-drop has changed destinations.
        /// </summary>
        public event EventHandler<MudDragAndDropIndexChangedEventArgs>? TransactionIndexChanged;

        /// <summary>
        /// Occurs when a drag-and-drop operation has completed or canceled.
        /// </summary>
        public event EventHandler<MudDragAndDropTransactionFinishedEventArgs<T>>? TransactionEnded;

        /// <summary>
        /// Occurs when a refresh for this component has been requested.
        /// </summary>
        public event EventHandler? RefreshRequested;

        /// <summary>
        /// Starts a new drag-and-drop operation.
        /// </summary>
        /// <param name="item">The item to drag.</param>
        /// <param name="identifier">The unique identifier for the item.</param>
        /// <param name="index">The index of the zone where the drag started.</param>
        /// <param name="commitCallback">Occurs when the drag-and-drop has finished successfully.</param>
        /// <param name="cancelCallback">Occurs when the drag-and-drop operation has been canceled.</param>
        public void StartTransaction(T? item, string identifier, int index, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            var createTransaction = new MudDragAndDropItemTransaction<T>(item, identifier, index, commitCallback, cancelCallback);
            _transaction = createTransaction;
            TransactionStarted?.Invoke(this, createTransaction);
            ItemPicked.InvokeAsync(createTransaction);
        }

        /// <summary>
        /// Gets the item currently being dragged.
        /// </summary>
        /// <returns>The item being dragged, or <c>null</c> of no drag is in progress.</returns>
        public T? GetTransactionItem()
        {
            if (_transaction is null)
            {
                return default;
            }

            var capturedTransaction = _transaction;
            return capturedTransaction.Item;

        }

        /// <summary>
        /// Gets whether a drag-and-drop is in progress.
        /// </summary>
        public bool TransactionInProgress() => _transaction is not null;

        /// <summary>
        /// Gets the unique ID of the zone where the drag-and-drop started.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="GetTransactionOriginZoneIdentifier()"/> method with the corrected spelling instead.
        /// </remarks>
        /// <returns>The unique ID of the zone.</returns>
        [Obsolete("Use the GetTransactionOriginZoneIdentifier method instead.  This will be removed in a future release.")]
        public string GetTransactionOrignZoneIdentiifer() => GetTransactionOriginZoneIdentifier();

        /// <summary>
        /// Gets the unique ID of the zone where the drag-and-drop started.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="GetTransactionOriginZoneIdentifier()"/> method with the corrected spelling instead.
        /// </remarks>
        /// <returns>The unique ID of the zone.</returns>
        [Obsolete("Use the GetTransactionOriginZoneIdentifier method instead.  This will be removed in a future release.")]
        public string GetTransactionOrignZoneIdentifier() => GetTransactionOriginZoneIdentifier();

        /// <summary>
        /// Gets the unique ID of the zone where the drag-and-drop started.
        /// </summary>
        /// <returns>The unique ID of the zone.</returns>
        public string GetTransactionOriginZoneIdentifier() => _transaction?.SourceZoneIdentifier ?? string.Empty;

        /// <summary>
        /// Gets the unique ID of the zone where the item is currently hovering.
        /// </summary>
        /// <remarks>
        /// Use the <see cref="GetTransactionCurrentZoneIdentifier()"/> method with the corrected spelling instead.
        /// </remarks>
        /// <returns>The unique ID of the zone.</returns>
        [Obsolete("Use the GetTransactionCurrentZoneIdentifier method instead.  This will be removed in a future release.")]
        public string GetTransactionCurrentZoneIdentiifer() => GetTransactionCurrentZoneIdentifier();

        /// <summary>
        /// Gets the unique ID of the zone where the item is currently hovering.
        /// </summary>
        /// <returns>The unique ID of the zone.</returns>
        public string GetTransactionCurrentZoneIdentifier() => _transaction?.CurrentZone ?? string.Empty;

        /// <summary>
        /// Gets whether the item being dragged originated from the specified zone.
        /// </summary>
        /// <param name="identifier">The unique ID of the zone to find.</param>
        /// <returns>when <c>true</c>, the item was dragged from the specified zone.</returns>
        public bool IsTransactionOriginatedFromInside(string identifier) => _transaction?.SourceZoneIdentifier == identifier;

        /// <summary>
        /// Gets the index of the zone the item is currently hovering over.
        /// </summary>
        /// <returns>The index of the zone.</returns>
        public int GetTransactionIndex() => _transaction?.Index ?? -1;

        /// <summary>
        /// Gets whether the current index of the zone is greater than the zone where the transaction started.
        /// </summary>
        /// <returns>When <c>true</c>, the item's current zone index is higher than the original zone.</returns>
        public bool IsItemMovedDownwards() => _transaction?.Index > _transaction?.SourceIndex;

        /// <summary>
        /// Gets whether the item is located over a different zone than the original zone.
        /// </summary>
        /// <returns>When <c>true</c>, the item has moved from the original zone.</returns>
        public bool HasTransactionIndexChanged()
        {
            if (_transaction is null)
            {
                return false;
            }

            var capturedTransaction = _transaction;
            if (capturedTransaction.CurrentZone != capturedTransaction.SourceZoneIdentifier)
            {
                return true;
            }

            return capturedTransaction.Index != capturedTransaction.SourceIndex;
        }

        /// <summary>
        /// Gets whether the specified zone is where the drag-and-drop transaction started.
        /// </summary>
        /// <param name="index">The index of the zone to find.</param>
        /// <param name="identifier">The unique ID of the zone to fine.</param>
        /// <remarks>
        /// Use the <see cref="IsOrigin(int, string)"/> method instead.
        /// </remarks>
        /// <returns>When <c>true</c>, the zone is where the drag-and-drop transaction started.</returns>
        [Obsolete("Use the IsOrigin method instead.  This will be removed in a future release.")]
        public bool IsOrign(int index, string identifier) => IsOrigin(index, identifier);

        /// <summary>
        /// Gets whether the specified zone is where the drag-and-drop transaction started.
        /// </summary>
        /// <param name="index">The index of the zone to find.</param>
        /// <param name="identifier">The unique ID of the zone to fine.</param>
        /// <returns>When <c>true</c>, the zone is where the drag-and-drop transaction started.</returns>
        public bool IsOrigin(int index, string identifier)
        {
            if (_transaction is null)
            {
                return false;
            }

            var capturedTransaction = _transaction;
            if (identifier != capturedTransaction.SourceZoneIdentifier)
            {
                return false;
            }

            return capturedTransaction.SourceIndex == index || capturedTransaction.SourceIndex - 1 == index;
        }

        /// <summary>
        /// Completes a drag-and-drop transaction.
        /// </summary>
        /// <param name="dropZoneIdentifier">The unique ID of the zone the item was dropped onto.</param>
        /// <param name="reorderIsAllowed">When <c>true</c>, items can be reordered.</param>
        public async Task CommitTransaction(string dropZoneIdentifier, bool reorderIsAllowed)
        {
            if (_transaction is null)
            {
                return;
            }

            //We need to capture this variable because there is race condition when the value can turn null in the middle of transaction
            //There are multiple methods that can manipulate _transaction variable at same time
            //https://github.com/MudBlazor/MudBlazor/issues/6551
            var capturedTransaction = _transaction;
            await capturedTransaction.Commit();
            var index = -1;
            if (reorderIsAllowed)
            {
                index = GetTransactionIndex() + 1;
                if (capturedTransaction.SourceZoneIdentifier == capturedTransaction.CurrentZone && index > capturedTransaction.SourceIndex)
                {
                    index -= 1;
                }
            }

            await ItemDropped.InvokeAsync(new MudItemDropInfo<T>(capturedTransaction.Item, dropZoneIdentifier, index));
            var transactionFinishedEventArgs = new MudDragAndDropTransactionFinishedEventArgs<T>(dropZoneIdentifier, true, capturedTransaction);
            _transaction = null;
            TransactionEnded?.Invoke(this, transactionFinishedEventArgs);
        }

        /// <summary>
        /// Cancels a drag-and-drop transaction.
        /// </summary>
        public async Task CancelTransaction()
        {
            if (_transaction is null)
            {
                return;
            }

            var capturedTransaction = _transaction;
            await capturedTransaction.Cancel();
            var transactionFinishedEventArgs = new MudDragAndDropTransactionFinishedEventArgs<T>(capturedTransaction);
            _transaction = null;
            TransactionEnded?.Invoke(this, transactionFinishedEventArgs);
        }

        /// <summary>
        /// Updates the index of the zone the item is being dragged over.
        /// </summary>
        /// <param name="index">The index of the current zone.</param>
        public void UpdateTransactionIndex(int index)
        {
            if (_transaction is null)
            {
                return;
            }

            var capturedTransaction = _transaction;
            var changed = _transaction.UpdateIndex(index);
            if (changed)
            {
                TransactionIndexChanged?.Invoke(this, new MudDragAndDropIndexChangedEventArgs(capturedTransaction.CurrentZone, capturedTransaction.CurrentZone, capturedTransaction.Index));
            }
        }

        internal void UpdateTransactionZone(string identifier)
        {
            if (_transaction is null)
            {
                return;
            }

            var capturedTransaction = _transaction;
            var oldValue = capturedTransaction.CurrentZone;
            var changed = capturedTransaction.UpdateZone(identifier);
            if (changed)
            {
                TransactionIndexChanged?.Invoke(this, new MudDragAndDropIndexChangedEventArgs(capturedTransaction.CurrentZone, oldValue, capturedTransaction.Index));
            }
        }

        internal bool RegisterDropZone(MudDropZone<T> dropZone)
        {
            return _mudDropZones.TryAdd(dropZone.Identifier, dropZone);
        }
        internal void RemoveDropZone(string identifier)
        {
            _mudDropZones.Remove(identifier);
        }
        internal MudDropZone<T>? GetDropZone(string identifier)
        {
            return _mudDropZones.TryGetValue(identifier, out var dropZone) ? dropZone : null;
        }

        /// <summary>
        /// Refreshes the drop zone and all items within.
        /// </summary>
        /// <remarks>
        /// When called, the <see cref="RefreshRequested"/> event is raised.  This is typically used when adding items to the collection or changing values of items.
        /// </remarks>
        public void Refresh() => RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}
