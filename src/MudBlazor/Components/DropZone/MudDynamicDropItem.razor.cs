// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudDynamicDropItem<T> : MudComponentBase where T : notnull
{
    private bool _dragOperationIsInProgress = false;
    private readonly string _id = Identifier.Create();
    private double _onTouchStartX;
    private double _onTouchStartY;
    private double _onTouchLastX;
    private double _onTouchLastY;
    private int _dragHandleCount = 0;

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;

    [CascadingParameter]
    protected MudDropContainer<T>? Container { get; set; }

    /// <summary>
    /// The zone identifier of the corresponding drop zone
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Behavior)]
    public string? ZoneIdentifier { get; set; }

    /// <summary>
    /// the data item that is represented by this item
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Behavior)]
    public T? Item { get; set; }

    /// <summary>
    /// Child content of component
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// An additional class that is applied to this element when a drag operation is in progress
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.DraggingClass)]
    public string? DraggingClass { get; set; }

    /// <summary>
    /// An event callback set fires, when a drag operation has been started
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Behavior)]
    public EventCallback<T> OnDragStarted { get; set; }

    /// <summary>
    /// An event callback set fires, when a drag operation has been ended. This included also a canceled transaction
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Behavior)]
    public EventCallback<T> OnDragEnded { get; set; }

    /// <summary>
    /// When true, the item can't be dragged. defaults to false
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Disabled)]
    public bool Disabled { get; set; } = false;

    /// <summary>
    /// The class that is applied when disabled <see cref="Disabled"/> is set to true
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Disabled)]
    public string? DisabledClass { get; set; }

    /// <summary>
    /// The index of this item within its drop zone.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Sorting)]
    public int Index { get; set; } = -1;

    /// <summary>
    /// Hides this item's content.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Sorting)]
    public bool HideContent { get; set; }

    /// <summary>
    /// At least one <see cref="MudDragHandle{T}"/> is registered for this item.
    /// When <c>true</c>, the item is not directly draggable and drag starts from a handle.
    /// </summary>
    public bool HasDragHandle => _dragHandleCount > 0;

    /// <summary>
    /// Returns <c>true</c> when the outer div should carry <c>draggable="true"</c>.
    /// This is suppressed when a <see cref="MudDragHandle{T}"/> is present so that only
    /// the handle element initiates the browser drag gesture.
    /// </summary>
    private bool IsEffectivelyDraggable => !Disabled && !HasDragHandle;

    #region Drag-handle registration

    /// <summary>
    /// Increments the handle reference count and, once at least one handle exists,
    /// suppresses the default full-item draggable behavior.
    /// </summary>
    internal void RegisterDragHandle()
    {
        _dragHandleCount++;
        // Re-render so the draggable attribute reflects the new state.
        StateHasChanged();
    }

    /// <summary>
    /// Decrements the handle reference count and restores full-item draggable behavior
    /// when no handles remain.
    /// </summary>
    internal void UnregisterDragHandle()
    {
        if (_dragHandleCount > 0)
        {
            _dragHandleCount--;
            StateHasChanged();
        }
    }

    #endregion

    #region Event handling and callbacks

    private Task OnTouchStart(TouchEventArgs e) => IsEffectivelyDraggable ? TouchStartedAsync(e) : Task.CompletedTask;
    private Task OnTouchMove(TouchEventArgs e) => IsEffectivelyDraggable ? TouchMovedAsync(e) : Task.CompletedTask;
    private Task OnTouchEnd(TouchEventArgs e) => IsEffectivelyDraggable ? TouchEndedAsync(e) : Task.CompletedTask;

    internal async Task DragStartedAsync()
    {
        if (Container is null)
        {
            return;
        }

        _dragOperationIsInProgress = true;
        Container.StartTransaction(Item, ZoneIdentifier ?? string.Empty, Index, OnDroppedSucceeded, OnDroppedCanceled);
        await OnDragStarted.InvokeAsync();
    }

    internal async Task TouchStartedAsync(TouchEventArgs e)
    {
        if (Index == -1) return; //the -1 item shouldn't be ever moved.
        if (Disabled) return; //disabled items shouldn't be moved.

        _onTouchStartX = e.ChangedTouches[0].ClientX;
        _onTouchStartY = e.ChangedTouches[0].ClientY;
        _onTouchLastX = _onTouchStartX;
        _onTouchLastY = _onTouchStartY;

        if (Container is null)
        {
            return;
        }

        _dragOperationIsInProgress = true;
        await JsRuntime.InvokeVoidAsync("mudDragAndDrop.makeDropZonesNotRelative");
        Container.StartTransaction(Item, ZoneIdentifier ?? string.Empty, Index, OnDroppedSucceeded,
            OnDroppedCanceled);
        await OnDragStarted.InvokeAsync();
    }

    private async Task OnDroppedSucceeded()
    {
        _dragOperationIsInProgress = false;
        await JsRuntime.InvokeVoidAsync("mudDragAndDrop.resetItem", _id);
        await OnDragEnded.InvokeAsync(Item);
        StateHasChanged();
    }

    private async Task OnDroppedCanceled()
    {
        _dragOperationIsInProgress = false;
        await JsRuntime.InvokeVoidAsync("mudDragAndDrop.resetItem", _id);
        await OnDragEnded.InvokeAsync(Item);
        StateHasChanged();
    }

    internal async Task DragEndedAsync()
    {
        if (_dragOperationIsInProgress)
        {
            _dragOperationIsInProgress = false;
            if (Container is not null)
            {
                await Container.CancelTransaction();
            }
        }
        else
        {
            await OnDragEnded.InvokeAsync(Item);
        }
    }

    internal async Task TouchMovedAsync(TouchEventArgs e)
    {
        if (Index == -1 || Disabled) return;

        //Calculate change from last Move event
        var x = e.ChangedTouches[0].ClientX - _onTouchLastX;
        var y = e.ChangedTouches[0].ClientY - _onTouchLastY;

        _onTouchLastX = e.ChangedTouches[0].ClientX;
        _onTouchLastY = e.ChangedTouches[0].ClientY;

        //Send to JS to move DOM element
        await JsRuntime.InvokeVoidAsync("mudDragAndDrop.moveItemByDifference", _id, x, y);

        if (Container is not null && Container.TransactionInProgress())
        {
            var dropIndexOnPositionString = await JsRuntime.InvokeAsync<string>("mudDragAndDrop.getDropIndexOnPosition", _onTouchLastX, _onTouchLastY, _id);
            if (int.TryParse(dropIndexOnPositionString, out var dropIndex))
            {
                Container.UpdateTransactionIndex(dropIndex);
            }
        }

    }

    internal async Task TouchEndedAsync(TouchEventArgs e)
    {
        if (Index == -1 || Disabled)
        {
            return;
        }

        if (_dragOperationIsInProgress)
        {
            _onTouchLastX = e.ChangedTouches[0].ClientX;
            _onTouchLastY = e.ChangedTouches[0].ClientY;
            var dropZoneIdentifier =
                await JsRuntime.InvokeAsync<string>("mudDragAndDrop.getDropZoneIdentifierOnPosition", _onTouchLastX,
                    _onTouchLastY);

            var (_, isValidZone) = ItemCanBeDropped(dropZoneIdentifier);
            if (isValidZone)
            {
                Container?.UpdateTransactionZone(dropZoneIdentifier);
                var dropZone = Container?.GetDropZone(dropZoneIdentifier);
                if (dropZone is not null)
                {
                    await dropZone.HandleDrop();
                }
            }
            else
            {
                _dragOperationIsInProgress = false;
                if (Container is not null)
                {
                    await Container.CancelTransaction();
                }

                StateHasChanged();
            }
        }
    }

    /// <summary>
    /// This allows us to know if an item can be dropped on a given drop zone.
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    private (T?, bool) ItemCanBeDropped(string identifier)
    {
        var dropZone = Container?.GetDropZone(identifier);
        if (dropZone is null || Container is null || !Container.TransactionInProgress())
        {
            return (default, false);
        }

        var item = Container.GetTransactionItem();

        var result = true;
        if (dropZone.CanDrop is not null)
        {
            if (item is not null)
            {
                result = dropZone.CanDrop(item);
            }
        }
        else if (Container.CanDrop is not null)
        {
            if (item is not null)
            {
                result = Container.CanDrop(item, identifier);
            }
        }

        return (item, result);
    }

    private void HandleDragEnter()
    {
        if (Container is not null && Container.TransactionInProgress())
        {
            Container.UpdateTransactionIndex(Index);
        }
    }

    private void HandleDragLeave()
    {
    }

    #endregion

    protected string Classname =>
        new CssBuilder("mud-drop-item")
            .AddClass("mud-drop-item-draggable", IsEffectivelyDraggable)
            .AddClass(DraggingClass, _dragOperationIsInProgress)
            .AddClass(DisabledClass, Disabled)
            .AddClass(Class)
            .Build();

    protected string Stylename =>
        new StyleBuilder()
            .AddStyle("transform", "translate3d(0px, 0px, 0px)")
            .AddStyle(Style)
            .Build();
}
