// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace MudBlazor;

/// <summary>
/// Provides data for the <see cref="MudChartBase{T,TOptions}.OnDataPointMouseOver"/> event.
/// </summary>
/// <typeparam name="T">The numeric data type of the chart.</typeparam>
public sealed class ChartHoverEventArgs<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    /// <summary>
    /// <c>true</c> when the pointer entered the data point; <c>false</c> when it left.
    /// </summary>
    public bool MouseIsOver { get; init; }

    /// <summary>
    /// The zero-based series or segment index of the hovered element.
    /// </summary>
    /// <remarks>
    /// Returns <c>-1</c> for chart types without a natural integer identifier (e.g. Sankey).
    /// </remarks>
    public int Index { get; init; } = -1;

    /// <summary>
    /// The X-axis label of the hovered element, such as a category name.
    /// </summary>
    public string? XLabel { get; init; }

    /// <summary>
    /// The formatted Y-axis value of the hovered element.
    /// </summary>
    public string? YLabel { get; init; }

    /// <summary>
    /// The actual data value of the hovered element.
    /// </summary>
    /// <remarks>
    /// Populated for <b>Donut</b>, <b>Pie</b>, <b>Rose</b>, and <b>HeatMap</b> charts.
    /// <c>null</c> for Bar, StackedBar, Line, TimeSeries, Radar, and Sankey charts, where the value is only available as the formatted <see cref="YLabel"/> string.
    /// </remarks>
    public T? Value { get; init; }

    /// <summary>
    /// The row index of the hovered cell. Only populated for HeatMap charts.
    /// </summary>
    public int? Row { get; init; }

    /// <summary>
    /// The column index of the hovered cell. Only populated for HeatMap charts.
    /// </summary>
    public int? Column { get; init; }
}
