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
            .AddClass($"gap-{Spacing}")
            .AddClass($"flex-grow-{StretchItems?.ToDescriptionString()}", StretchItems is not null and not Stretch.None)
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
    /// If there is only one child, <see cref="Stretch.Start"/> and <see cref="Stretch.End"/>
    /// will have the same effect, and the child will be stretched.
    /// <see cref="Stretch.Middle"/> stretches all children except the first and last child.
    /// If there are two or fewer elements, <see cref="Stretch.Middle"/> will have no effect.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Stack.Behavior)]
    public Stretch? StretchItems { get; set; }

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
