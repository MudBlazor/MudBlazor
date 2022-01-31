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
    public partial class MudItemDropZone<T> : MudComponentBase, IDisposable
    {
        [CascadingParameter]
        protected MudItemDropContainer<T> Container { get; set; }

        [Parameter]
        public string Identifier { get; set; }

        /// <summary>
        /// The items that can be drag and dropped within the container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public RenderFragment<T> ItemRenderer { get; set; }

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public Func<T, bool> ItemsSelector { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool HideItemOnDrag { get; set; }

        /// <summary>
        /// A additional class that is applied, when an item from this dropzone is dragged
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string DraggingClass2 { get; set; }

        /// <summary>
        /// A additional class that is applied, when an item from this dropzone is dragged
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string ItemDraggingClass { get; set; }

        private IEnumerable<T> GetItems()
        {
            Func<T, bool> predicate = null;
            if (ItemsSelector != null)
            {
                predicate = ItemsSelector;
            }

            predicate = (item) => Container.ItemsSelector(item, Identifier ?? String.Empty);

            return Container.Items.Where(predicate).ToArray();
        }


        private RenderFragment<T> GetItemTemplate() => ItemRenderer ?? Container?.ItemRenderer;


        private bool _canDrop = false;
        private bool _itemOnDropZone = false;

        private String GetDragginClass()
        {
            if (String.IsNullOrEmpty(DraggingClass2) == true)
            {
                return Container?.DraggingClass ?? String.Empty;
            }

            return DraggingClass2;
        }

        private String GetItemDraggingClass()
        {
            if (String.IsNullOrEmpty(ItemDraggingClass) == true)
            {
                return Container?.ItemDraggingClass ?? String.Empty;
            }

            return DraggingClass2;
        }

        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool? ApplyDropClassesOnDragStarted { get; set; }

        private bool GetApplyDropClassesOnDragStarted() => (ApplyDropClassesOnDragStarted ?? Container?.ApplyDropClassesOnDragStarted) ?? false;

        protected string Classname =>
            new CssBuilder("mud-drop-zone")
                .AddClass(CanDropClass ?? Container.CanDropClass, Container.TransactionInProgress() == true && _canDrop == true && (_itemOnDropZone == true || GetApplyDropClassesOnDragStarted() == true))
                .AddClass(NoDropClass ?? Container.NoDropClass, Container.TransactionInProgress() == true && _canDrop == false && (_itemOnDropZone == true || GetApplyDropClassesOnDragStarted() == true))
                .AddClass(GetDragginClass(), _dragInProgress == true)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public Func<T, bool> CanDrop { get; set; }

        /// <summary>
        /// The CSS class to use if valid drop.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public string CanDropClass { get; set; }

        /// <summary>
        /// The CSS class to use if not valid drop.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public string NoDropClass { get; set; }

        private bool _containerIsInitilized;

        protected override void OnParametersSet()
        {
            if (Container != null && _containerIsInitilized == false)
            {
                _containerIsInitilized = true;
                Container.TransactionStarted += Container_TransactionStarted;
                Container.TransactionEnded += Container_TransactionEnded;
            }

            base.OnParametersSet();
        }

        private void Container_TransactionEnded(object sender, EventArgs e)
        {
            _itemOnDropZone = false;

            if (GetApplyDropClassesOnDragStarted() == false) { return; }

            _canDrop = false;
            StateHasChanged();
        }

        private void Container_TransactionStarted(object sender, DragAndDropItemTransaction<T> e)
        {
            if (GetApplyDropClassesOnDragStarted() == false) { return; }

            var dropResult = ItemCanBeDropped();
            _canDrop = dropResult.Item2;
            StateHasChanged();
        }

        private (DragAndDropItemTransaction<T>, bool) ItemCanBeDropped()
        {
            if (Container == null || Container.TransactionInProgress() == false)
            {
                return (null, false);
            }

            var context = Container.GetContext();

            var result = true;
            if (CanDrop != null)
            {
                result = CanDrop(context.Item);
            }
            else if (Container.CanDrop != null)
            {
                result = Container.CanDrop(context.Item, Identifier);
            }

            return (context, result);
        }

        private void HandleDragEnter()
        {
            var (context, isValidZone) = ItemCanBeDropped();
            if (context == null)
            {
                return;
            }

            _itemOnDropZone = true;
            _canDrop = isValidZone;
        }

        private void HandleDragLeave()
        {
            var (context, _) = ItemCanBeDropped();
            if (context == null)
            {
                return;
            }

            Console.WriteLine();

            _itemOnDropZone = false;
        }

        private async Task HandleDrop()
        {
            var (context, isValidZone) = ItemCanBeDropped();
            if (context == null)
            {
                return;
            }

            _itemOnDropZone = false;

            if (isValidZone == false)
            {
                await Container.CancelTransaction();
                return;
            }

            await Container.CommitTransaction(Identifier);
        }

        private bool _dragInProgress = false;
        private bool _disposedValue;

        private void FinishedDragOperation() => _dragInProgress = false;
        private void DragOperationStarted() => _dragInProgress = true;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Container.TransactionStarted -= Container_TransactionStarted;
                    Container.TransactionEnded -= Container_TransactionEnded;
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
