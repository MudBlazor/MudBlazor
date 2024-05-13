﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
public partial class MudGrid : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-grid")
            .AddClass($"gap-{Spacing}")
            .AddClass($"justify-{Justify.ToDescriptionString()}")
            .AddClass(Class)
            .Build();

    /// <summary>
    /// The gap between items, measured in increments of <c>4px</c>.
    /// <br/>
    /// Maximum is <c>16</c>.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public int Spacing { set; get; } = 3;

    /// <summary>
    /// Defines the distribution of children along the main axis within a <see cref="MudStack"/> component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public Justify Justify { get; set; } = Justify.FlexStart;

    /// <summary>
    /// Child content of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
