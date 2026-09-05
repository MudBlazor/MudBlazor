// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A CSS grid based layout component for organizing content into defined rows and columns.
/// </summary>
/// <seealso cref="MudMatrixItem"/>
/// <seealso cref="ExplicitMatrix"/>
/// <seealso cref="ImplicitMatrix"/>
public partial class MudMatrix : MudComponentBase
{
    protected string Classname =>
        new CssBuilder("mud-matrix")
            .AddClass($"gap-y-{RowGap.ToString()}")
            .AddClass($"gap-x-{ColumnGap.ToString()}")
            .AddClass($"mud-matrix-justify-rows-{JustifyRows.ToStringFast(true)}")
            .AddClass($"mud-matrix-justify-columns-{JustifyColumns.ToStringFast(true)}")
            .AddClass(Class)
            .Build();

    protected string Stylename =>
        new StyleBuilder()
            .AddStyle("grid-template-columns", ExplicitColumns.ToString())
            .AddStyle("grid-template-rows", ExplicitRows.ToString())
            .AddStyle($"grid-auto-columns", ImplicitColumns.ToString())
            .AddStyle($"grid-auto-rows", ImplicitRows.ToString())
            .AddStyle($"grid-auto-flow", HorizontalFlow ? "column" : "row")
            .AddStyle(Style)
            .Build();

    /// <summary>
    /// The gap between columns, measured in increments of <c>4px</c>.
    /// </summary>
    /// <remarks>
    /// <para>Default is 6.</para>
    /// <para>Minimum is 0.</para>
    /// <para>Maximum is 20.</para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public int ColumnGap { set; get; } = 6;

    /// <summary>
    /// The gap between rows, measured in increments of <c>4px</c>.
    /// </summary>
    /// <remarks>
    /// <para>Default is 6.</para>
    /// <para>Minimum is 0.</para>
    /// <para>Maximum is 20.</para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public int? RowGap { set; get; } = 6;

    /// <summary>
    /// Controls the direction in which items are placed into the grid.
    /// When true, items fill from top to bottom and wrap into new columns.
    /// When false, items fill from left to right and wrap into new rows.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Maps to CSS <c>grid-auto-flow</c>.
    /// </para>
    /// <para>
    /// Default is false (row)
    /// </para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public bool HorizontalFlow { get; set; }

    /// <summary>
    /// Defines how columns are aligned when there is leftover space within a row.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public MatrixJustify JustifyColumns { get; set; } = MatrixJustify.Start;

    /// <summary>
    /// Defines how rows are aligned when there is leftover space within the Matrix.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public MatrixJustify JustifyRows { get; set; } = MatrixJustify.Start;

    /// <summary>
    /// Child content of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Defines the structure of the columns.
    /// This controls how many columns exist and how wide each column is.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Maps to CSS <c>grid-template-columns</c>.
    /// </para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public ExplicitMatrix ExplicitColumns { get; set; } = new();

    /// <summary>
    /// Defines the structure of the rows.
    /// This controls how many rows exist and how tall each row is.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Maps to CSS <c>grid-template-rows</c>.
    /// </para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public ExplicitMatrix ExplicitRows { get; set; } = new();

    /// <summary>
    /// Defines the structure for columns created when the number of items
    /// exceeds the defined ColumnTemplate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This maps to CSS <c>grid-auto-columns</c>.
    /// </para>
    /// <para>
    /// This only applies to columns that are created automatically when content overflows
    /// the explicit column definition.
    /// </para>
    /// <para>
    /// This does not affect explicitly defined columns (see <see cref="ExplicitColumns"/>).
    /// </para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public ImplicitMatrix ImplicitColumns { get; set; } = new();

    /// <summary>
    /// Defines the structure for rows created when the number of items
    /// exceeds the defined RowTemplate.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This maps to CSS <c>grid-auto-rows</c>.
    /// </para>
    /// <para>
    /// This only applies to rows that are created automatically when content overflows
    /// the explicit row definition.
    /// </para>
    /// <para>
    /// This does not affect explicitly defined rows (see <see cref="ExplicitRows"/>).
    /// </para>
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Matrix.Behavior)]
    public ImplicitMatrix ImplicitRows { get; set; } = new();
}

