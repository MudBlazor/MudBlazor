// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudDropItem : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-drop-item")
            .AddClass(DraggingClass, !String.IsNullOrWhiteSpace(DraggingClass))
            .AddClass("hidden", HideOnDrag)
            .AddClass(Class)
            .Build();
    
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
}
