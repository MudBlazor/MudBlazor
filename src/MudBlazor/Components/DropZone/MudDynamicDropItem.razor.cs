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
            .AddClass("blub23", HideOnDrag == true && _dragOperationIsInProgress == true)
            .AddClass(DisabledClass, Disabled == true)
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

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public EventCallback<T> OnDragStarted { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public EventCallback<T> OnDragSuccess { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public EventCallback<T> OnDropFailed { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public EventCallback<T> OnDragEnded { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public string ZoneIdentifier { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public bool Disabled { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public string DisabledClass { get; set; }

    private async Task DragStarted()
    {
        if (Container == null) { return; }

        _dragOperationIsInProgress = true;
        Container.StartTransaction(Item, ZoneIdentifier ?? string.Empty, OnDroppedSucceeded, OnDroppedCanceled);
        await OnDragStarted.InvokeAsync();
    }

    private async Task OnDroppedSucceeded()
    {
        _dragOperationIsInProgress = false;

        await OnDragSuccess.InvokeAsync(Item);
        await OnDragEnded.InvokeAsync(Item);
        StateHasChanged();
    }

    private async Task OnDroppedCanceled()
    {
        _dragOperationIsInProgress = false;

        await OnDropFailed.InvokeAsync(this.Item);
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

    private Guid _id = Guid.NewGuid();
}
