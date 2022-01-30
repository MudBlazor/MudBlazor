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
    public partial class MudItemDropZone<T> : MudComponentBase
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
        public string DraggingClass { get; set; }

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

        protected string Classname =>
            new CssBuilder("mud-drop-zone")
                .AddClass(CanDropClass, _canDrop == true && _itemOnDropZone == true)
                .AddClass(NoDropClass, _canDrop == false && _itemOnDropZone == true)
            .AddClass(DraggingClass ?? Container?.DraggingClass, _dragInProgress == true)
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
                await context.Cancel();
                return;
            }

            await Container.CommitTransaction(context, Identifier);
        }

        private bool _dragInProgress = false;

        private void FinishedDragOperation() => _dragInProgress = false;
        private void DragOperationStarted() => _dragInProgress = true;
    }
}
