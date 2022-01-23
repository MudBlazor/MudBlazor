// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudDropZone<T> : MudComponentBase
{
    private bool _canDrop = false;
    private bool _itemOnDropZone = false;

    [CascadingParameter]
    protected MudDropContainer<T> Container { get; set; }

    protected string Classname =>
        new CssBuilder("mud-drop-zone")
            .AddClass(CanDropClass, _canDrop == true && _itemOnDropZone == true)
            .AddClass(NoDropClass, _canDrop == false && _itemOnDropZone == true)
            .AddClass(Class)
            .Build();

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

    /// <summary>
    /// The CSS class to use if an item from this zone is being dragged.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.DropZone.Appearance)]
    public string DraggingClass { get; set; }

    /// <summary>
    /// Child content of component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public Func<T, Task<bool>> CanDrop { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public EventCallback<T> ItemDropped { get; set; }

    [Parameter]
    [Category(CategoryTypes.Button.Behavior)]
    public IEnumerable<string> CanDropGroups { get; set; } = Array.Empty<string>();

    private async Task<(DragAndDropTransaction<T>, bool)> ItemCanBeDropped()
    {
        if (Container == null || Container.TransactionInProgress() == false)
        {
            return (null, false);
        }

        var context = Container.GetContext();
        if (string.IsNullOrEmpty(context.DropGroup) == false)
        {
            var groups = CanDropGroups ?? Array.Empty<string>();
            if(groups.Any() == true)
            {
                if(groups.Any(x => x == context.DropGroup) == false)
                {
                    return (context, false);
                }
            }
        }

        var result = true;
        if (CanDrop != null)
        {
            result = await CanDrop(context.Item);
        }

        return (context, result);
    }

    private async Task HandleDragEnter()
    {
        var (context, isValidZone) = await ItemCanBeDropped();
        if (context == null)
        {
            return;
        }

        _itemOnDropZone = true;
        _canDrop = isValidZone;
    }

    private async Task HandleDragLeave()
    {
        var (context, _) = await ItemCanBeDropped();
        if (context == null)
        {
            return;
        }

        _itemOnDropZone = false;
    }

    private async Task HandleDrop()
    {
        var (context, isValidZone) = await ItemCanBeDropped();
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

        await context.Commit();
        await ItemDropped.InvokeAsync(context.Item);
    }
}
