// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Components.Grid;
using MudBlazor.Utilities;

namespace MudBlazor;


/// <summary>
/// A 12-point grid system for organizing content with responsive breakpoints for different screen sizes.
/// </summary>
/// <seealso cref="MudItem"/>
public partial class MudGrid : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-grid")
            .AddClass("mud-grid-template", Template)
            .AddClass("mud-grid-flex", !Template)
            .AddClass($"mud-grid-spacing-xs-{Spacing.ToString()}", !Template)
            .AddClass($"mud-grid-gap-{Spacing.ToString()}", Template)
            .AddClass($"justify-{Justify.ToStringFast(true)}", !Template)
            .AddClass(Class)
            .Build();

    protected string Stylename =>
        new StyleBuilder()
            .AddStyle("grid-template-columns", ColumnTemplate?.ToString() ?? (Columns > 0 ? $"repeat({Columns}, 1fr)" : "auto"), Template)
            .AddStyle("grid-template-rows", RowTemplate?.ToString() ?? (Rows > 0 ? $"repeat({Rows}, 1fr)" : "auto"), Template)
            .AddStyle(Style)
            .Build();

    /// <summary>
    /// The gap between items, measured in increments of <c>4px</c>.
    /// </summary>
    /// <remarks>
    /// <para>Defaults to 6.</para>
    /// <para>Maximum is 20.</para>
    /// <para>The increment was halved in v7, so the default is now 6 instead of 3.</para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public int Spacing { set; get; } = 6;

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

    /// <summary>
    /// Allows grid to behave with defined rows and columns
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public bool Template { get; set; }

    /// <summary>
    /// Number of columns per row. 
    /// </summary>
    /// <remarks>
    /// <para>Each column takes equal width (1fr).</para>
    /// <para>If set to 0, it will default to auto.</para>
    /// <para>Ignored if <see cref="ColumnTemplate"/> is set.</para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public int Columns { get; set; } = 1;

    /// <summary>
    /// Number of rows in the grid.
    /// </summary>
    /// <remarks>
    /// <para>Each row takes equal height (1fr). </para>
    /// <para>If set to 0, it will default to auto.</para>
    /// <para>Requires the grid to have a defined height to take effect.</para>
    /// <para>Ignored if <see cref="RowTemplate"/> is set.</para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public int Rows { get; set; } = 0;

    /// <summary>
    /// Template for colums. 
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public GridTemplate? ColumnTemplate { get; set; }

    /// <summary>
    /// Template for rows.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Grid.Behavior)]
    public GridTemplate? RowTemplate { get; set; }
}
