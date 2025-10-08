// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Charts;

/// <summary>
/// Represents the options for a chart that has axes.
/// </summary>
public interface IAxisChartOptions : IChartOptions
{
    /// <summary>
    /// The spacing between vertical tick marks.
    /// </summary>
    public int YAxisTicks { get; set; }

    /// <summary>
    /// The maximum allowed number of vertical tick marks.
    /// </summary>
    public int MaxNumYAxisTicks { get; set; }

    /// <summary>
    /// The format applied to numbers on the vertical axis.
    /// </summary>
    public string? YAxisFormat { get; set; }

    /// <summary>
    /// Custom formatting function for vertical axis values.
    /// If set, this function will be used to convert Y-axis values to strings for display purposes.
    /// If not provided, <see cref="YAxisFormat"/> will be used instead.
    /// </summary>
    public Func<double, string>? YAxisToStringFunc { get; set; }

    /// <summary>
    /// Shows vertical axis lines.
    /// </summary>
    public bool YAxisLines { get; set; }

    /// <summary>
    /// Shows horizontal axis lines.
    /// </summary>
    public bool XAxisLines { get; set; }

    /// <summary>
    /// Rotation angle to rotate the labels in degrees.
    /// </summary>
    public int XAxisLabelRotation { get; set; }
}
