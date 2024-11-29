// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor;

/// <summary>
/// Indicates the type of chart to display.
/// </summary>
public enum ChartType
{
    /// <summary>
    /// Data is displayed as a hollow circle.
    /// </summary>
    Donut,
    /// <summary>
    /// Data is displayed as connecting lines.
    /// </summary>
    Line,
    /// <summary>
    /// Data is displayed as a portion of a circle.
    /// </summary>
    Pie,
    /// <summary>
    /// Data is displayed as rectangles.
    /// </summary>
    Bar,
    /// <summary>
    /// Data is displayed as connected rectangles.
    /// </summary>
    StackedBar,
    /// <summary>
    /// Data is displayed as connecting lines or as areas.
    /// </summary>
    Timeseries,
    /// <summary>
    /// Data is displayed as a heatmap. Similar to how github works.
    /// </summary>
    HeatMap,
}
