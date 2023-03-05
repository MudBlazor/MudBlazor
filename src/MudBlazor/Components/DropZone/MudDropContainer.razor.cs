// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class MudDragAndDropIndexChangedEventArgs : EventArgs
    {
        public MudDragAndDropIndexChangedEventArgs(string zoneIdentifier, string oldZoneIdentifier, int index)
        {
            ZoneIdentifier = zoneIdentifier;
            Index = index;
            OldZoneIdentifier = oldZoneIdentifier;
        }

        public string ZoneIdentifier { get; }
        public int Index { get; }
        public string OldZoneIdentifier { get; }
    }

    /// <summary>
    /// Used to encapsulate data for a drag and drop transaction
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MudDragAndDropItemTransaction<T>
    {
        private Func<Task> _commitCallback;
        private Func<Task> _cancelCallback;

        /// <summary>
        /// The Item that is dragged during the transaction
        /// </summary>
        public T Item { get; init; }

        /// <summary>
        /// The index of the item in the current drop zone
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The index of the item when the transaction started
        /// </summary>
        public int SourceIndex { get; private set; }

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
        /// <param name="commitCallback">A callback that is invokde when the transaction has been successful</param>
        /// <param name="cancelCallback">A callback that is inviked when the transaction has been canceled</param>
        public MudDragAndDropItemTransaction(T item, string identifier, int index, Func<Task> commitCallback, Func<Task> cancelCallback)
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
        public async Task Cancel() => await _cancelCallback.Invoke();

        /// <summary>
        /// Commit this transaction as succesful
        /// </summary>
        /// <returns></returns>
        public async Task Commit() => await _commitCallback.Invoke();

        internal bool UpdateIndex(int index)
        {
            if (Index == index) { return false; }

            Index = index;
            return true;
        }

        internal bool UpdateZone(string idenfifer)
        {
            if (CurrentZone == idenfifer) { return false; }

            CurrentZone = idenfifer;
            Index = -1;
            return true;
        }
    }

    /// <summary>
    /// Record encaplusalting data regaring a completed transaction
    /// </summary>
    /// <typeparam name="T">Type of dragged item</typeparam>
    /// <param name="Item">The dragged item during the transaction</param>
    /// <param name="DropzoneIdentifier">Identifier of the zone where the transaction started</param>
    /// <param name="IndexInZone">The index of the item within in the dropzone</param>
    public record MudItemDropInfo<T>(T Item, string DropzoneIdentifier, int IndexInZone);

    public class MudDragAndDropTransactionFinishedEventArgs<T> : EventArgs
    {
        public MudDragAndDropTransactionFinishedEventArgs(MudDragAndDropItemTransaction<T> transaction) :
            this(string.Empty, false, transaction)
        {

        }

        public MudDragAndDropTransactionFinishedEventArgs(string destinationDropzoneIdentifier, bool success, MudDragAndDropItemTransaction<T> transaction)
        {
            Item = transaction.Item;
            Success = success;
            OriginatedDropzoneIdentifier = transaction.SourceZoneIdentifier;
            DestinationDropzoneIdentifier = destinationDropzoneIdentifier;
            OriginIndex = transaction.SourceIndex;
            DestinationIndex = transaction.Index;
        }

        public T Item { get; }
        public bool Success { get; }
        public string OriginatedDropzoneIdentifier { get; }
        public string DestinationDropzoneIdentifier { get; }
        public int OriginIndex { get; }
        public int DestinationIndex { get; }
    }


    /// <summary>
    /// The container of a drag and drop zones
    /// </summary>
    /// <typeparam name="T">Datetype of items</typeparam>
    public partial class MudDropContainer<T> : MudComponentBase
    {
        private MudDragAndDropItemTransaction<T> _transaction;

        internal Dictionary<string, MudDropZone<T>> MudDropZones { get; set; } = new Dictionary<string, MudDropZone<T>>();

        protected string Classname =>
        new CssBuilder("mud-drop-container")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Child content of component. This should include the drop zones
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The items that can be drag and dropped within the container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// The render fragment (template) that should be used to render the items within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public RenderFragment<T> ItemRenderer { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public Func<T, string, bool> ItemsSelector { get; set; }

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
        public Func<T, string, bool> CanDrop { get; set; }

        /// <summary>
        /// The CSS class(es), that is applied to drop zones that are a valid target for drag and drop transaction
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string CanDropClass { get; set; }

        /// <summary>
        /// The CSS class(es), that is applied to drop zones that are NOT valid target for drag and drop transaction
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string NoDropClass { get; set; }

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
        public Func<T, bool> ItemIsDisabled { get; set; }

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
        public string DraggingClass { get; set; }

        /// <summary>
        /// An additional class that is applied to an drop item, when it is dragged
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DraggingClass)]
        public string ItemDraggingClass { get; set; }

        public event EventHandler<MudDragAndDropItemTransaction<T>> TransactionStarted;
        public event EventHandler<MudDragAndDropIndexChangedEventArgs> TransactionIndexChanged;

        public event EventHandler<MudDragAndDropTransactionFinishedEventArgs<T>> TransactionEnded;
        public event EventHandler RefreshRequested;

        public void StartTransaction(T item, string identifier, int index, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            _transaction = new MudDragAndDropItemTransaction<T>(item, identifier, index, commitCallback, cancelCallback);
            TransactionStarted?.Invoke(this, _transaction);
        }

        public T GetTransactionItem() => _transaction.Item;

        public bool TransactionInProgress() => _transaction != null;
        public string GetTransactionOrignZoneIdentiifer() => _transaction?.SourceZoneIdentifier ?? string.Empty;
        public string GetTransactionCurrentZoneIdentiifer() => _transaction?.CurrentZone ?? string.Empty;
        public bool IsTransactionOriginatedFromInside(string identifier) => _transaction.SourceZoneIdentifier == identifier;

        public int GetTransactionIndex() => _transaction?.Index ?? -1;
        public bool IsItemMovedDownwards() => _transaction.Index > _transaction.SourceIndex;
        public bool HasTransactionIndexChanged()
        {
            if (_transaction == null)
            {
                return false;
            }

            if (_transaction.CurrentZone != _transaction.SourceZoneIdentifier)
            {
                return true;
            }

            return _transaction.Index != _transaction.SourceIndex;
        }

        public bool IsOrign(int index, string identifier)
        {
            if (_transaction == null)
            {
                return false;
            }

            if (identifier != _transaction.SourceZoneIdentifier)
            {
                return false;
            }

            return _transaction.SourceIndex == index || _transaction.SourceIndex - 1 == index;
        }

        public async Task CommitTransaction(string dropzoneIdentifier, bool reorderIsAllowed)
        {
            await _transaction.Commit();
            var index = -1;
            if (reorderIsAllowed == true)
            {
                index = GetTransactionIndex() + 1;
                if (_transaction.SourceZoneIdentifier == _transaction.CurrentZone && IsItemMovedDownwards() == true)
                {
                    index -= 1;
                }
            }

            await ItemDropped.InvokeAsync(new MudItemDropInfo<T>(_transaction.Item, dropzoneIdentifier, index));
            var transactionFinishedEventArgs = new MudDragAndDropTransactionFinishedEventArgs<T>(dropzoneIdentifier, true, _transaction);
            _transaction = null;
            TransactionEnded?.Invoke(this, transactionFinishedEventArgs);
        }

        public async Task CancelTransaction()
        {
            await _transaction.Cancel();
            var transactionFinishedEventArgs = new MudDragAndDropTransactionFinishedEventArgs<T>(_transaction);
            _transaction = null;
            TransactionEnded?.Invoke(this, transactionFinishedEventArgs);
        }

        public void UpdateTransactionIndex(int index)
        {
            var changed = _transaction.UpdateIndex(index);
            if (changed == false) { return; }

            TransactionIndexChanged?.Invoke(this, new MudDragAndDropIndexChangedEventArgs(_transaction.CurrentZone, _transaction.CurrentZone, _transaction.Index));
        }

        internal void UpdateTransactionZone(string identifier)
        {
            var oldValue = _transaction.CurrentZone;
            var changed = _transaction.UpdateZone(identifier);
            if (changed == false) { return; }

            TransactionIndexChanged?.Invoke(this, new MudDragAndDropIndexChangedEventArgs(_transaction.CurrentZone, oldValue, _transaction.Index));
        }

        internal bool RegisterDropZone(MudDropZone<T> dropZone)
        {
            return MudDropZones.TryAdd(dropZone.Identifier, dropZone);
        }
        internal void RemoveDropZone(string identifier)
        {
            MudDropZones.Remove(identifier);
        }
        internal MudDropZone<T> GetDropZone(string identifier)
        {
            return MudDropZones.TryGetValue(identifier, out var dropZone) ? dropZone : null;
        }

        /// <summary>
        /// Refreshes the dropzone and all items within. This is neded in case of adding items to the collection or changed values of items
        /// </summary>
        public void Refresh() => RefreshRequested?.Invoke(this, EventArgs.Empty);


    }
}
