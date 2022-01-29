// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudDynamicDropItem<T> : MudComponentBase
{
    [CascadingParameter]
    protected MudItemDropContainer<T> Container { get; set; }

    protected string Classname =>
        new CssBuilder("mud-drop-item")
            .AddClass(DraggingClass, string.IsNullOrWhiteSpace(DraggingClass) == false && _dragOperationIsInProgress == true)
            .AddClass("hidden", HideOnDrag == true && _dragOperationIsInProgress == true)
            .AddClass(Class)
            .Build();

    private bool _dragOperationIsInProgress = false;

    /// <summary>
    /// The CSS class to use if the item is being dragged, note that is only applies to the item inside the starting zone.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Appearance)]
    public string DraggingClass { get; set; }

    /// <summary>
    /// If true, the item will be hidden ones a drag event starts from its original drop zone.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public bool HideOnDrag { get; set; }

    /// <summary>
    /// Child content of component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public RenderFragment ChildContent { get; set; }

    //[Parameter]
    //[Category(CategoryTypes.Button.Behavior)]
    //public bool Disable { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public T Item { get; set; }

    //[Parameter]
    //[Category(CategoryTypes.Button.Behavior)]
    //public EventCallback<T> OnDragFinished { get; set; }

    //[Parameter]
    //[Category(CategoryTypes.Button.Behavior)]
    //public EventCallback<T> OnDragCancelled { get; set; }

    //[Parameter]
    //[Category(CategoryTypes.Button.Behavior)]
    //public EventCallback<T> OnDropFailed { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public string DropGroup { get; set; }

    private void DragStarted()
    {
        if (Container == null) { return; }

        _dragOperationIsInProgress = true;
        Container.StartTransaction(Item, DropGroup ?? string.Empty, OnDroppedSucceeded, OnDroppedCanceled);
    }

    private void OnDroppedSucceeded()
    {
        _dragOperationIsInProgress = false;

        //await OnDragFinished.InvokeAsync(this.Item);
        StateHasChanged();
    }

    private void OnDroppedCanceled()
    {
        _dragOperationIsInProgress = false;

        //await OnDropFailed.InvokeAsync(this.Item);
        StateHasChanged();
    }

    private void DragEnded(DragEventArgs e)
    {
        if (_dragOperationIsInProgress == false) { return; }

        _dragOperationIsInProgress = false;

        //await OnDragCancelled.InvokeAsync(Item);
        StateHasChanged();
    }
}
