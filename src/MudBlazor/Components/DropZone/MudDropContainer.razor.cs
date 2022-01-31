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
    public class MudDragAndDropItemTransaction<T>
    {
        private Func<Task> _commitCallback;
        private Func<Task> _cancelCallback;

        public T Item { get; init; }
        public string ZoneIdentifier { get; init; }

        public MudDragAndDropItemTransaction(T item, string identifier, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            Item = item;
            ZoneIdentifier = identifier;

            _commitCallback = commitCallback;
            _cancelCallback = cancelCallback;
        }

        public async Task Cancel() => await _cancelCallback.Invoke();
        public async Task Commit() => await _commitCallback.Invoke();
    }

    public record MudItemDropInfo<T>(T Item, string DropzoneIdentifier);

    public partial class MudDropContainer<T> : MudComponentBase
    {
        private MudDragAndDropItemTransaction<T> _transaction;

        protected string Classname =>
        new CssBuilder("mud-drop-container")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The items that can be drag and dropped within the container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// The items that can be drag and dropped within the container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public RenderFragment<T> ItemRenderer { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public Func<T, string, bool> ItemsSelector { get; set; }

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public EventCallback<MudItemDropInfo<T>> ItemDropped { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public Func<T, string, bool> CanDrop { get; set; }

        /// <summary>
        /// The CSS class to use if valid drop.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public string CanDropClass { get; set; }

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ApplyDropClassesOnDragStarted { get; set; } = false;

        /// <summary>
        /// The CSS class to use if not valid drop.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public string NoDropClass { get; set; }

        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public Func<T, bool> ItemIsDisbaled { get; set; }

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string DisabledClass { get; set; } = "disabled";

        /// <summary>
        /// A additional class that is applied, when an item from this dropzone is dragged
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string ItemDraggingClass { get; set; }

        /// <summary>
        /// A additional class that is applied, when an item from this dropzone is dragged
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string DraggingClass { get; set; }

        public event EventHandler<MudDragAndDropItemTransaction<T>> TransactionStarted;
        public event EventHandler TransactionEnded;

        public void StartTransaction(T item, string identifier, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            _transaction = new MudDragAndDropItemTransaction<T>(item, identifier, commitCallback, cancelCallback);
            TransactionStarted?.Invoke(this, _transaction);
        }

        public MudDragAndDropItemTransaction<T> GetContext() => _transaction;

        public bool TransactionInProgress() => _transaction != null;
        public string GetTransactionOrignZoneIdentiifer() => _transaction?.ZoneIdentifier ?? string.Empty;

        internal async Task CommitTransaction(string dropzoneIdentifier)
        {
            await _transaction.Commit();
            await ItemDropped.InvokeAsync(new MudItemDropInfo<T>(_transaction.Item, dropzoneIdentifier));
            TransactionEnded?.Invoke(this, EventArgs.Empty);
            _transaction = null;
        }

        internal async Task CancelTransaction()
        {
            await _transaction.Cancel();
            TransactionEnded?.Invoke(this, EventArgs.Empty);
            _transaction = null;
        }


    }
}
