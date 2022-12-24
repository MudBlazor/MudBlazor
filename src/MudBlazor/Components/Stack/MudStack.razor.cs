// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor;

public partial class MudStack : MudComponentBase
{
    protected string Classname =>
    new CssBuilder("d-flex")
        .AddClass($"flex-{(Row ? "row" : "column")}{(Reverse ? "-reverse" : string.Empty)}")
        .AddClass($"justify-{Justify?.ToDescriptionString()}", Justify != null)
        .AddClass($"align-{AlignItems?.ToDescriptionString()}", AlignItems != null)
        .AddClass($"gap-{Spacing}")
        .AddClass($"flex-grow-{StretchChildren?.ToDescriptionString()}", StretchChildren is not null and not MudBlazor.StretchChildren.None)
        .AddClass(Class)
    .Build();

    /// <summary>
    /// If true, items will be placed horizontally in a row instead of vertically.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public bool Row { get; set; }

    /// <summary>
    /// Reverses the order of its items.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public bool Reverse { get; set; }

    /// <summary>
    /// Defines the spacing between its items.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public int Spacing { get; set; } = 3;

    /// <summary>
    /// Defines the spacing between its items.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public Justify? Justify { get; set; }

    /// <summary>
    /// Defines the spacing between its items.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public AlignItems? AlignItems { get; set; }

    /// <summary>
    /// Defines stretching the Stack's children on the main axis.
    /// If there is only one child, StretchChildren.FirstChild and
    /// StretchChildren.LastChild will have the same effect and the child will be stretched.
    /// StretchChildren.MiddleChildren will stretch all children except the first and last child.
    /// If there are two or fewer elements StretchChildren.MiddleChildren will have no effect.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public StretchChildren? StretchChildren { get; set; }

    /// <summary>
    /// Child content of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public RenderFragment ChildContent { get; set; }
}

