﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudDynamicDropItem<T> : MudComponentBase
{
    private bool _dragOperationIsInProgress = false;
    private Guid _id = Guid.NewGuid();
    private double _onTouchStartX;
    private double _onTouchStartY;
    private double _onTouchLastX;
    private double _onTouchLastY;

    [Inject] private IJSRuntime JsRuntime { get; set; }

    [CascadingParameter]
    protected MudDropContainer<T> Container { get; set; }

    /// <summary>
    /// The zone identifier of the corresponding drop zone
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Behavior)]
    public string ZoneIdentifier { get; set; }

    /// <summary>
    /// the data item that is represneted by this item
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Behavior)]
    public T Item { get; set; }

    /// <summary>
    /// Child content of component
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Appearance)]
    public RenderFragment ChildContent { get; set; }

    /// <summary>
    /// An additional class that is applied to this element when a drag operation is in progress
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.DraggingClass)]
    public string DraggingClass { get; set; }

    /// <summary>
    /// An event callback set fires, when a drag operation has been started
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Behavior)]
    public EventCallback<T> OnDragStarted { get; set; }

    /// <summary>
    /// An event callback set fires, when a drag operation has been eneded. This included also a cancelled transaction
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
    public string DisabledClass { get; set; }

    [Parameter]
    [Category(CategoryTypes.DropZone.Sorting)]
    public int Index { get; set; } = -1;

    [Parameter]
    [Category(CategoryTypes.DropZone.Sorting)]
    public bool HideContent { get; set; }

    #region Event handling and callbacks

    private async Task DragStarted()
    {
        if (Container == null) { return; }

        _dragOperationIsInProgress = true;
        Container.StartTransaction(Item, ZoneIdentifier ?? string.Empty, Index, OnDroppedSucceeded, OnDroppedCanceled);
        await OnDragStarted.InvokeAsync();
    }

    private async Task TouchStarted(TouchEventArgs e)
    {
        _onTouchStartX = e.ChangedTouches[0].ClientX;
        _onTouchStartY = e.ChangedTouches[0].ClientY;
        _onTouchLastX = _onTouchStartX;
        _onTouchLastY = _onTouchStartY;

        if (Container == null) { return; }

        _dragOperationIsInProgress = true;
        await JsRuntime.InvokeVoidAsync("mudDragAndDrop.makeDropZonesNotRelative");
        Container.StartTransaction(Item, ZoneIdentifier ?? string.Empty, Index, OnDroppedSucceeded, OnDroppedCanceled);
        await OnDragStarted.InvokeAsync();
    }

    private async Task OnDroppedSucceeded()
    {
        _dragOperationIsInProgress = false;

        await OnDragEnded.InvokeAsync(Item);
        StateHasChanged();
    }

    private async Task OnDroppedCanceled()
    {
        _dragOperationIsInProgress = false;

        await OnDragEnded.InvokeAsync(Item);
        StateHasChanged();
    }

    private async Task DragEnded(DragEventArgs e)
    {
        if (_dragOperationIsInProgress == true)
        {
            _dragOperationIsInProgress = false;
            await Container?.CancelTransaction();
        }
        else
        {
            await OnDragEnded.InvokeAsync(Item);
        }
    }
    private async void TouchMoved(TouchEventArgs e)
    {
        //Calculate change from last Move event
        var x = e.ChangedTouches[0].ClientX - _onTouchLastX;
        var y = e.ChangedTouches[0].ClientY - _onTouchLastY;

        _onTouchLastX = e.ChangedTouches[0].ClientX;
        _onTouchLastY = e.ChangedTouches[0].ClientY;

        //Send to JS to move DOM element
        await JsRuntime.InvokeVoidAsync("mudDragAndDrop.moveItemByDifference", _id.ToString(), x, y);


        //JS.InvokeVoidAsync("draggableTouch");
    }

    private async Task TouchEnded(TouchEventArgs e)
    {
        if (_dragOperationIsInProgress == true)
        {
            _dragOperationIsInProgress = false;
            await JsRuntime.InvokeVoidAsync("mudDragAndDrop.resetItem", _id.ToString());
            await JsRuntime.InvokeVoidAsync("mudDragAndDrop.makeDropZonesRelative");
            await Container?.CancelTransaction();
        }
        else
        {
            await OnDragEnded.InvokeAsync(Item);
        }
    }

    private void HandleDragEnter()
    {
        if (Container == null || Container.TransactionInProgress() == false) { return; }
        
        Container.UpdateTransactionIndex(Index);
    }

    private void HandleDragLeave()
    {
    }

    #endregion

    protected string Classname =>
    new CssBuilder("mud-drop-item")
        .AddClass(DraggingClass, _dragOperationIsInProgress == true)
        .AddClass(DisabledClass, Disabled == true)
        .AddClass(Class)
        .Build();

}
