// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Charts;

/// <summary>
/// Represents the default options for a chart with axes.
/// </summary>
public abstract class DefaultAxisChartOptions : DefaultChartOptions, IAxisChartOptions
{
    private int _yAxisTicks = 20;
    private int _maxNumYAxisTicks = 20;

    /// <summary>
    /// The spacing between vertical tick marks.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>20</c>.
    /// Value must be greater than or equal to <c>1</c>.
    /// </remarks>
    public int YAxisTicks
    {
        get => _yAxisTicks;
        set => _yAxisTicks = Math.Max(1, value);
    }

    /// <summary>
    /// The maximum allowed number of vertical tick marks.
    /// </summary>
    /// <remarks>
    /// If the number of ticks calculated exceeds this value, the tick marks will automatically be thinned out.
    /// Value must be greater than or equal to <c>1</c>.
    /// </remarks>
    public int MaxNumYAxisTicks
    {
        get => _maxNumYAxisTicks;
        set => _maxNumYAxisTicks = Math.Max(1, value);
    }

    /// <summary>
    /// The maximum value for the vertical axis.
    /// </summary>
    /// <remarks>
    /// This value is used only if all data points are less than or equal to it. 
    /// If any data point exceeds this value, the Y-axis maximum will automatically adjust to fit the data.
    /// If this value is <c>null</c>, the Y-axis maximum will be calculated automatically.
    /// </remarks>
    public double? YAxisSuggestedMax { get; set; }

    /// <summary>
    /// The format applied to numbers on the vertical axis.
    /// </summary>
    /// <remarks>
    /// Values in this property are standard .NET format strings, such as those passed into the <c>ToString()</c> method.  For a list of common formats, see: <see href="https://learn.microsoft.com/dotnet/standard/base-types/formatting-types" />
    /// </remarks>
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
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool YAxisLines { get; set; } = true;

    /// <summary>
    /// Shows horizontal axis lines.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool XAxisLines { get; set; }

    /// <summary>
    /// Rotation angle to rotate the labels in degrees.
    /// </summary>
    public int XAxisLabelRotation { get; set; }

    /// <summary>
    /// The title of the X-axis.
    /// </summary>
    public string? XAxisTitle { get; set; }

    /// <summary>
    /// The title of the Y-axis.
    /// </summary>
    public string? YAxisTitle { get; set; }
}
