// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

public partial class MudPaper : MudComponentBase
{
    protected string Classname =>
    new CssBuilder("mud-paper")
      .AddClass($"mud-paper-outlined", Outlined)
      .AddClass($"mud-paper-square", Square)
      .AddClass($"mud-elevation-{Elevation.ToString()}", !Outlined)
      .AddClass(Class)
    .Build();

    protected string Stylename =>
    new StyleBuilder()
        .AddStyle("height", $"{Height}", !String.IsNullOrEmpty(Height))
        .AddStyle("width", $"{Width}", !String.IsNullOrEmpty(Width))
        .AddStyle("max-height", $"{MaxHeight}", !String.IsNullOrEmpty(MaxHeight))
        .AddStyle("max-width", $"{MaxWidth}", !String.IsNullOrEmpty(MaxWidth))
        .AddStyle("min-height", $"{MinHeight}", !String.IsNullOrEmpty(MinHeight))
        .AddStyle("min-width", $"{MinWidth}", !String.IsNullOrEmpty(MinWidth))
        .AddStyle(Style)
    .Build();

    /// <summary>
    /// The higher the number, the heavier the drop-shadow.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public int Elevation { set; get; } = 1;

    /// <summary>
    /// If true, border-radius is set to 0.
    /// </summary>
    [Parameter] 
    [Category(CategoryTypes.Paper.Appearance)]
    public bool Square { get; set; }

    /// <summary>
    /// If true, card will be outlined.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public bool Outlined { get; set; }

    /// <summary>
    /// Height of the component.
    /// </summary>
    [Parameter] 
    [Category(CategoryTypes.Paper.Appearance)]
    public string Height { get; set; }

    /// <summary>
    /// Width of the component.
    /// </summary>
    [Parameter] 
    [Category(CategoryTypes.Paper.Appearance)]
    public string Width { get; set; }

    /// <summary>
    /// Max-Height of the component.
    /// </summary>
    [Parameter] 
    [Category(CategoryTypes.Paper.Appearance)]
    public string MaxHeight { get; set; }

    /// <summary>
    /// Max-Width of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public string MaxWidth { get; set; }

    /// <summary>
    /// Min-Height of the component.
    /// </summary>
    [Parameter] 
    [Category(CategoryTypes.Paper.Appearance)]
    public string MinHeight { get; set; }

    /// <summary>
    /// Min-Width of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public string MinWidth { get; set; }

    /// <summary>
    /// Child content of the component.
    /// </summary>
    [Parameter] 
    [Category(CategoryTypes.Paper.Behavior)]
    public RenderFragment ChildContent { get; set; }
}
