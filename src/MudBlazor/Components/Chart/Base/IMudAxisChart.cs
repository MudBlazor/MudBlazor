// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Charts;

/// <summary>
/// Represents data for the grid of an axis chart.
/// </summary>
/// <param name="LowestHorizontalLine">The lowest horizontal line value.</param>
/// <param name="HorizontalLineCount">The number of horizontal lines.</param>
/// <param name="YAxisTicks">The interval between Y-axis ticks.</param>
/// <param name="BoundWidth">The width of the chart bounds.</param>
/// <param name="BoundHeight">The height of the chart bounds.</param>
/// <typeparam name="T">The data type of the chart.</typeparam>
public record struct AxisGridData<T>(int LowestHorizontalLine, int HorizontalLineCount, T YAxisTicks, double BoundWidth, double BoundHeight)
    where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable;

/// <summary>
/// Represents a chart that has axes.
/// </summary>
/// <typeparam name="T">The data type of the chart.</typeparam>
public interface IMudAxisChart<T> : IMudChart<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    /// <summary>
    /// The data for the grid of the chart.
    /// </summary>
    public AxisGridData<T>? SharedData { get; set; }

    /// <summary>
    /// The chart to be overlaid on top of this chart.
    /// </summary>
    public IMudChart<T>? OverlayChart { get; set; }

    /// <summary>
    /// The content to be rendered as an overlay.
    /// </summary>
    public RenderFragment? OverlayContent { get; set; }
}
