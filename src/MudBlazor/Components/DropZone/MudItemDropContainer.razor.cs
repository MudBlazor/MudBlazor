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
    public class DragAndDropItemTransaction<T>
    {
        private Func<Task> _commitCallback;
        private Func<Task> _cancelCallback;

        public T Item { get; set; }
        public string DropGroup { get; set; }

        public DragAndDropItemTransaction(T item, string dropGroup, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            Item = item;
            DropGroup = dropGroup;

            _commitCallback = commitCallback;
            _cancelCallback = cancelCallback;
        }

        public async Task Cancel() => await _cancelCallback.Invoke();
        public async Task Commit() => await _commitCallback.Invoke();
    }

    public class MudItemDropInfo<T>
    {
        public T Item { get; private set; }
        public string DropzoneIdentifier { get; private set; }

        public MudItemDropInfo(T item, String identifier)
        {
            Item = item;
            DropzoneIdentifier = identifier;
        }
    }

    public partial class MudItemDropContainer<T> : MudComponentBase
    {
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
        public Func<T, string, bool> CanDrop { get; set; }

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public EventCallback<MudItemDropInfo<T>> ItemDropped { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public Func<T, string, bool> ItemsSelector { get; set; }

        /// <summary>
        /// A additional class that is applied, when an item from this dropzone is dragged
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string DraggingClass { get; set; }

        private DragAndDropItemTransaction<T> _transaction;

        public void StartTransaction(T item, string dropGroup, Func<Task> commitCallback, Func<Task> cancelCallback)
        {
            _transaction = new DragAndDropItemTransaction<T>(item, dropGroup, commitCallback, cancelCallback);
        }

        public DragAndDropItemTransaction<T> GetContext() => _transaction;

        public bool TransactionInProgress() => _transaction != null;

        internal async Task CommitTransaction(DragAndDropItemTransaction<T> context, string dropzoneIdentifier)
        {
            await context.Commit();
            await ItemDropped.InvokeAsync(new MudItemDropInfo<T>(context.Item, dropzoneIdentifier));
        }
    }
}
