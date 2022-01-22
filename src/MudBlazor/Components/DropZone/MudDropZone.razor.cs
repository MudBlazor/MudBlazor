// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudDropZone : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-drop-zone")
            .AddClass(CanDropClass, _canDrop = true)
            .AddClass(NoDropClass, _canDrop = false)
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
    
    private bool _canDrop { get; set; }
}
