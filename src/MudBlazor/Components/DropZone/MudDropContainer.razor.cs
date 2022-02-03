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
        /// Identifier for drop zone where the transaction started
        /// </summary>
        public string SourceZoneIdentifier { get; init; }

        /// <summary>
        /// create a new instance of a drag and drop transaction encapsulating the item and source
        /// </summary>
        /// <param name="item">The item of this transaction</param>
        /// <param name="identifier">The identifier of the drop zone, where the transaction started</param>
        /// <param name="commitCallback">A callback that is invokde when the transaction has been successful</param>
        /// <param name="cancelCallback">A callback that is inviked when the transaction has been cancelled</param>
        public MudDragAndDropItemTransaction(T item, string identifier, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            Item = item;
            SourceZoneIdentifier = identifier;

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
    }

    /// <summary>
    /// Record encaplusalting data regaring a completed transaction
    /// </summary>
    /// <typeparam name="T">Type of dragged item</typeparam>
    /// <param name="Item">The dragged item during the transaction</param>
    /// <param name="DropzoneIdentifier">Identifier of the zone where the transaction started</param>
    public record MudItemDropInfo<T>(T Item, string DropzoneIdentifier);

    /// <summary>
    /// The container of a drag and drop zones
    /// </summary>
    /// <typeparam name="T">Datetype of items</typeparam>
    public partial class MudDropContainer<T> : MudComponentBase
    {
        private MudDragAndDropItemTransaction<T> _transaction;

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
        public event EventHandler TransactionEnded;
        public event EventHandler RefreshRequested;

        public void StartTransaction(T item, string identifier, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            _transaction = new MudDragAndDropItemTransaction<T>(item, identifier, commitCallback, cancelCallback);
            TransactionStarted?.Invoke(this, _transaction);
        }

        public T GetTransactionItem() => _transaction.Item;

        public bool TransactionInProgress() => _transaction != null;
        public string GetTransactionOrignZoneIdentiifer() => _transaction?.SourceZoneIdentifier ?? string.Empty;

        public async Task CommitTransaction(string dropzoneIdentifier)
        {
            await _transaction.Commit();
            await ItemDropped.InvokeAsync(new MudItemDropInfo<T>(_transaction.Item, dropzoneIdentifier));
            TransactionEnded?.Invoke(this, EventArgs.Empty);
            _transaction = null;
        }

        public async Task CancelTransaction()
        {
            await _transaction.Cancel();
            TransactionEnded?.Invoke(this, EventArgs.Empty);
            _transaction = null;
        }

        /// <summary>
        /// Refreshes the dropzone and all items within. This is neded in case of adding items to the collection or changed values of items
        /// </summary>
        public void Refresh() => RefreshRequested?.Invoke(this, EventArgs.Empty);
    }
}
