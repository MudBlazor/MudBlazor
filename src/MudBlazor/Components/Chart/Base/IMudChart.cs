// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace MudBlazor.Charts;

/// <summary>
/// Represents a chart component.
/// </summary>
/// <typeparam name="T">The data type of the chart.</typeparam>
public interface IMudChart<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    /// <summary>
    /// The series of data to be displayed in the chart.
    /// </summary>
    List<ChartSeries<T>> ChartSeries { get; set; }

    /// <summary>
    /// The palette of colors to be used for the legend.
    /// </summary>
    string[] LegendPalette { get; }

    /// <summary>
    /// The type of chart.
    /// </summary>
    ChartType ChartType { get; }

    /// <summary>
    /// Rebuilds the chart with the current data.
    /// </summary>
    void RebuildChart();
}
