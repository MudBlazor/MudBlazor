// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// A surface for grouping other components.
/// </summary>
public partial class MudPaper : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-paper")
            .AddClass($"mud-paper-outlined", Outlined)
            .AddClass($"mud-paper-square", Square)
            .AddClass($"mud-elevation-{Elevation}", !Outlined)
            .AddClass(Class)
            .Build();

    protected string Stylename =>
        new StyleBuilder()
            .AddStyle("height", $"{Height}", !string.IsNullOrEmpty(Height))
            .AddStyle("width", $"{Width}", !string.IsNullOrEmpty(Width))
            .AddStyle("max-height", $"{MaxHeight}", !string.IsNullOrEmpty(MaxHeight))
            .AddStyle("max-width", $"{MaxWidth}", !string.IsNullOrEmpty(MaxWidth))
            .AddStyle("min-height", $"{MinHeight}", !string.IsNullOrEmpty(MinHeight))
            .AddStyle("min-width", $"{MinWidth}", !string.IsNullOrEmpty(MinWidth))
            .AddStyle(Style)
            .Build();

    /// <summary>
    /// The size of the drop shadow.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c>.  A higher number creates a heavier drop shadow.  Use a value of <c>0</c> for no shadow.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public int Elevation { set; get; } = MudGlobal.PaperDefaults.Elevation;

    /// <summary>
    /// Displays a square shape.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.  When <c>true</c>, the <c>border-radius</c> is set to <c>0</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public bool Square { get; set; } = MudGlobal.PaperDefaults.Square;

    /// <summary>
    /// Displays an outline around this component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public bool Outlined { get; set; } = MudGlobal.PaperDefaults.Outlined;

    /// <summary>
    /// The height of this component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Can be a pixel height (<c>150px</c>), percentage (<c>30%</c>), or other CSS height value.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public string? Height { get; set; }

    /// <summary>
    /// The width of this component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Can be a pixel width (<c>150px</c>), percentage (<c>30%</c>), or other CSS width value.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public string? Width { get; set; }

    /// <summary>
    /// The maximum height of this component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Can be a pixel height (<c>150px</c>), percentage (<c>30%</c>), or other CSS height value.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public string? MaxHeight { get; set; }

    /// <summary>
    /// The maximum width of this component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Can be a pixel width (<c>150px</c>), percentage (<c>30%</c>), or other CSS width value.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public string? MaxWidth { get; set; }

    /// <summary>
    /// The minimum height of this component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Can be a pixel height (<c>150px</c>), percentage (<c>30%</c>), or other CSS height value.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public string? MinHeight { get; set; }

    /// <summary>
    /// The minimum width of this component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.  Can be a pixel width (<c>150px</c>), percentage (<c>30%</c>), or other CSS width value.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Paper.Appearance)]
    public string? MinWidth { get; set; }

    /// <summary>
    /// The content within this component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Paper.Behavior)]
    public RenderFragment? ChildContent { get; set; }
}
