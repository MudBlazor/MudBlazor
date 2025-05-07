// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Charts;

public abstract class DefaultAxisChartOptions : DefaultChartOptions, IAxisChartOptions
{
    /// <summary>
    /// The spacing between vertical tick marks.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>20</c>.
    /// </remarks>
    public int YAxisTicks { get; set; } = 20;
    /// <summary>
    /// The maximum allowed number of vertical tick marks.
    /// </summary>
    /// <remarks>
    /// If the number of ticks calculated exceeds this value, the tick marks will automatically be thinned out.
    /// </remarks>
    public int MaxNumYAxisTicks { get; set; } = 20;

    /// <summary>
    /// The format applied to numbers on the vertical axis.
    /// </summary>
    /// <remarks>
    /// Values in this property are standard .NET format strings, such as those passed into the <c>ToString()</c> method.  For a list of common formats, see: <see href="https://learn.microsoft.com/dotnet/standard/base-types/formatting-types" />
    /// </remarks>
    public string? YAxisFormat { get; set; }

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
}
