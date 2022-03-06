// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDropZone<T> : MudComponentBase, IDisposable
    {
        private bool _containerIsInitilized = false;
        private bool _canDrop = false;
        private bool _itemOnDropZone = false;
        private bool _dragInProgress = false;
        private bool _disposedValue = false;
        private Guid _id = Guid.NewGuid();

        [Inject] private IJSRuntime JsRuntime { get; set; }

        [CascadingParameter]
        protected MudDropContainer<T> Container { get; set; }

        /// <summary>
        /// Child content of component
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// The unique identifier of this drop zone. It is used within transaction to 
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Appearance)]
        public string Identifier { get; set; }

        /// <summary>
        /// The render fragment (template) that should be used to render the items within a drop zone. Overrides value provided by drop container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public RenderFragment<T> ItemRenderer { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone. Overrides value provided by drop container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Items)]
        public Func<T, bool> ItemsSelector { get; set; }

        /// <summary>
        /// The method is used to determinate if an item can be dropped within a drop zone. Overrides value provided by drop container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public Func<T, bool> CanDrop { get; set; }

        /// <summary>
        /// The CSS class(es), that is applied to drop zones that are a valid target for drag and drop transaction. Overrides value provided by drop container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string CanDropClass { get; set; }

        /// <summary>
        /// The CSS class(es), that is applied to drop zones that are NOT valid target for drag and drop transaction. Overrides value provided by drop container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public string NoDropClass { get; set; }

        /// <summary>
        /// If true, drop classes CanDropClass <see cref="CanDropClass"/>  or NoDropClass <see cref="NoDropClass"/> or applied as soon, as a transaction has started. Overrides value provided by drop container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.DropRules)]
        public bool? ApplyDropClassesOnDragStarted { get; set; }

        /// <summary>
        /// The method is used to determinate if an item should be disabled for dragging. Defaults to allow all items. Overrides value provided by drop container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Disabled)]
        public Func<T, bool> ItemIsDisabled { get; set; }

        /// <summary>
        /// If a drop item is disabled (determinate by <see cref="ItemIsDisabled"/>). This class is applied to the element. Overrides value provided by drop container
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.DropZone.Disabled)]
        public string DisabledClass { get; set; }

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

        #region view helper

        private IEnumerable<T> GetItems()
        {
            Func<T, bool> predicate = (item) => Container.ItemsSelector(item, Identifier ?? string.Empty);
            if (ItemsSelector != null)
            {
                predicate = ItemsSelector;
            }

            return (Container?.Items ?? Array.Empty<T>()).Where(predicate).ToArray();
        }

        private RenderFragment<T> GetItemTemplate() => ItemRenderer ?? Container?.ItemRenderer;

        private string GetDragginClass()
        {
            if (string.IsNullOrEmpty(DraggingClass) == true)
            {
                return Container?.DraggingClass ?? string.Empty;
            }

            return DraggingClass;
        }

        private string GetItemDraggingClass()
        {
            if (string.IsNullOrEmpty(ItemDraggingClass) == false)
            {
                return ItemDraggingClass;
            }

            return Container?.ItemDraggingClass ?? string.Empty;
        }

        private bool GetApplyDropClassesOnDragStarted() => (ApplyDropClassesOnDragStarted ?? Container?.ApplyDropClassesOnDragStarted) ?? false;

        private bool GetItemDisabledStatus(T item)
        {
            var result = false;
            var predicate = ItemIsDisabled ?? Container?.ItemIsDisabled;
            if (predicate != null)
            {
                result = predicate(item);
            }

            return result;
        }

        protected string Classname =>
            new CssBuilder("mud-drop-zone")
                .AddClass("mud-drop-zone-drag-block", Container?.TransactionInProgress() == true && Container.GetTransactionOrignZoneIdentiifer() != Identifier)
                .AddClass(CanDropClass ?? Container?.CanDropClass, Container?.TransactionInProgress() == true && Container.GetTransactionOrignZoneIdentiifer() != Identifier && _canDrop == true && (_itemOnDropZone == true || GetApplyDropClassesOnDragStarted() == true))
                .AddClass(NoDropClass ?? Container?.NoDropClass, Container?.TransactionInProgress() == true && Container.GetTransactionOrignZoneIdentiifer() != Identifier && _canDrop == false && (_itemOnDropZone == true || GetApplyDropClassesOnDragStarted() == true))
                .AddClass(GetDragginClass(), _dragInProgress == true)
                .AddClass(Class)
                .Build();

        #endregion

        #region helper

        private (T, bool) ItemCanBeDropped()
        {
            if (Container == null || Container.TransactionInProgress() == false)
            {
                return (default(T), false);
            }

            var item = Container.GetTransactionItem();

            var result = true;
            if (CanDrop != null)
            {
                result = CanDrop(item);
            }
            else if (Container.CanDrop != null)
            {
                result = Container.CanDrop(item, Identifier);
            }

            return (item, result);
        }

        #endregion

        #region container event handling

        private void Container_TransactionEnded(object sender, EventArgs e)
        {
            _itemOnDropZone = false;

            if (GetApplyDropClassesOnDragStarted() == true)
            {
                _canDrop = false;
            }

            StateHasChanged();
        }

        private void Container_TransactionStarted(object sender, MudDragAndDropItemTransaction<T> e)
        {
            if (GetApplyDropClassesOnDragStarted() == true)
            {
                var dropResult = ItemCanBeDropped();
                _canDrop = dropResult.Item2;
            }

            StateHasChanged();
        }

        private void Container_RefreshRequested(object sender, EventArgs e) => InvokeAsync(StateHasChanged);

        #endregion

        #region handling event callbacks

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
                await Container.CancelTransaction();
                return;
            }

            await Container.CommitTransaction(Identifier);
        }

        private void FinishedDragOperation() => _dragInProgress = false;
        private void DragOperationStarted() => _dragInProgress = true;

        #endregion

        #region life cycle

        protected override void OnParametersSet()
        {
            if (Container != null && _containerIsInitilized == false)
            {
                _containerIsInitilized = true;
                Container.TransactionStarted += Container_TransactionStarted;
                Container.TransactionEnded += Container_TransactionEnded;
                Container.RefreshRequested += Container_RefreshRequested;
            }

            base.OnParametersSet();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender == true)
            {
                await JsRuntime.InvokeVoidAsync("mudDragAndDrop.initDropZone", _id.ToString());
            }

            await base.OnAfterRenderAsync(firstRender);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    if (Container != null)
                    {
                        Container.TransactionStarted -= Container_TransactionStarted;
                        Container.TransactionEnded -= Container_TransactionEnded;
                        Container.RefreshRequested -= Container_RefreshRequested;
                    }
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
