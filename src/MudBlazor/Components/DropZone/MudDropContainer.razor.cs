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
    /// The container of a drag and drop zones
    /// </summary>
    /// <typeparam name="T">Type of items</typeparam>
    public partial class MudDropContainer<T> : MudComponentBase
    {
        private MudDragAndDropItemTransaction<T>? _transaction;
        private Dictionary<string, MudDropZone<T>> _mudDropZones = new();

        protected string Classname =>
        new CssBuilder("mud-drop-container")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Child content of component. This should include the drop zones
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The items that can be drag and dropped within the container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();

        /// <summary>
        /// The render fragment (template) that should be used to render the items within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public RenderFragment<T>? ItemRenderer { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public Func<T, string, bool>? ItemsSelector { get; set; }

        /// <summary>
        /// Callback that indicates that an item has been dropped on a drop zone. Should be used to update the "status" of the data item
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public EventCallback<MudItemDropInfo<T>> ItemDropped { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public Func<T, string, bool>? CanDrop { get; set; }

        /// <summary>
        /// The CSS class(es), that is applied to drop zones that are a valid target for drag and drop transaction
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string? CanDropClass { get; set; }

        /// <summary>
        /// The CSS class(es), that is applied to drop zones that are NOT valid target for drag and drop transaction
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string? NoDropClass { get; set; }

        /// <summary>
        /// If true, drop classes CanDropClass <see cref="CanDropClass"/>  or NoDropClass <see cref="NoDropClass"/> or applied as soon, as a transaction has started
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public bool ApplyDropClassesOnDragStarted { get; set; } = false;

        /// <summary>
        /// The method is used to determinate if an item should be disabled for dragging. Defaults to allow all items
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Disabled)]
        public Func<T, bool>? ItemIsDisabled { get; set; }

        /// <summary>
        /// If a drop item is disabled (determinate by <see cref="ItemIsDisabled"/>). This class is applied to the element
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Disabled)]
        public string DisabledClass { get; set; } = "disabled";

        /// <summary>
        /// An additional class that is applied to the drop zone where a drag operation started
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DraggingClass)]
        public string? DraggingClass { get; set; }

        /// <summary>
        /// An additional class that is applied to an drop item, when it is dragged
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DraggingClass)]
        public string? ItemDraggingClass { get; set; }

        public event EventHandler<MudDragAndDropItemTransaction<T>>? TransactionStarted;
        public event EventHandler<MudDragAndDropIndexChangedEventArgs>? TransactionIndexChanged;

        public event EventHandler<MudDragAndDropTransactionFinishedEventArgs<T>>? TransactionEnded;
        public event EventHandler? RefreshRequested;

        public void StartTransaction(T? item, string identifier, int index, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            var createTransaction = new MudDragAndDropItemTransaction<T>(item, identifier, index, commitCallback, cancelCallback);
            _transaction = createTransaction;
            TransactionStarted?.Invoke(this, createTransaction);
        }

        public T? GetTransactionItem()
        {
            if (_transaction is null)
            {
                return default;
            }

            var capturedTransaction = _transaction;
            return capturedTransaction.Item;

        }

        public bool TransactionInProgress() => _transaction is not null;

        public string GetTransactionOrignZoneIdentiifer() => _transaction?.SourceZoneIdentifier ?? string.Empty;

        public string GetTransactionCurrentZoneIdentiifer() => _transaction?.CurrentZone ?? string.Empty;

        public bool IsTransactionOriginatedFromInside(string identifier) => _transaction?.SourceZoneIdentifier == identifier;

        public int GetTransactionIndex() => _transaction?.Index ?? -1;

        public bool IsItemMovedDownwards() => _transaction?.Index > _transaction?.SourceIndex;

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

        public bool IsOrign(int index, string identifier)
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
                if (capturedTransaction.SourceZoneIdentifier == capturedTransaction.CurrentZone && IsItemMovedDownwards())
                {
                    index -= 1;
                }
            }

            await ItemDropped.InvokeAsync(new MudItemDropInfo<T>(capturedTransaction.Item, dropZoneIdentifier, index));
            var transactionFinishedEventArgs = new MudDragAndDropTransactionFinishedEventArgs<T>(dropZoneIdentifier, true, capturedTransaction);
            _transaction = null;
            TransactionEnded?.Invoke(this, transactionFinishedEventArgs);
        }

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
        /// Refreshes the drop zone and all items within. This is needed in case of adding items to the collection or changed values of items
        /// </summary>
        public void Refresh() => RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}
