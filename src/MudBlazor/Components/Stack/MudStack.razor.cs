// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
public partial class MudStack : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("d-flex")
            .AddClass($"flex-{(Row ? "row" : "column")}{(Reverse ? "-reverse" : string.Empty)}")
            .AddClass($"justify-{Justify?.ToDescriptionString()}", Justify is not null)
            .AddClass($"align-{AlignItems?.ToDescriptionString()}", AlignItems is not null)
            .AddClass($"flex-{Wrap?.ToDescriptionString()}", Wrap is not null)
            .AddClass($"gap-{Spacing}", Spacing >= 0)
            .AddClass($"flex-grow-{StretchItems?.ToDescriptionString()}", StretchItems is not null and not MudBlazor.StretchItems.None)
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
    /// The gap between items, measured in increments of <c>4px</c>.
    /// </summary>
    /// <remarks>
    /// Default is <c>3</c>.
    /// Maximum is <c>20</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public int Spacing { get; set; } = 3;

    /// <summary>
    /// Defines the distribution of children along the main axis within a <see cref="MudStack"/> component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public Justify? Justify { get; set; }

    /// <summary>
    /// Defines the alignment of children along the cross axis within a <see cref="MudStack"/> component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public AlignItems? AlignItems { get; set; }

    /// <summary>
    /// Defines the stretching behaviour of children along the main axis within a <see cref="MudStack"/> component.
    /// </summary>
    /// <remarks>
    /// Note: This property affects children of the <see cref="MudStack"/> component.
    /// If there is only one child, <see cref="StretchItems.Start"/> and <see cref="StretchItems.End"/>
    /// will have the same effect, and the child will be stretched.
    /// <see cref="StretchItems.Middle"/> stretches all children except the first and last child.
    /// If there are two or fewer elements, <see cref="StretchItems.Middle"/> will have no effect.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public StretchItems? StretchItems { get; set; }

    /// <summary>
    /// Defines the flexbox wrapping behavior of its items.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public Wrap? Wrap { get; set; }

    /// <summary>
    /// Child content of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
