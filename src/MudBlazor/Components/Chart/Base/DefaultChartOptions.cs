// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Components.Chart;

public class DefaultChartOptions : IChartOptions
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
    /// Shows the chart series legend.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool ShowLegend { get; set; } = true;

    /// <summary>
    /// Enables tooltips for values
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool ShowToolTips { get; set; } = true;

    /// <summary>
    /// The format applied to the data marker tooltip title.
    /// </summary>
    public string DefaultDataMarkerTooltipTitleFormat { get; set; } = "{{Y_VALUE}} - {{X_VALUE}}";

    /// <summary>
    /// The list of colors applied to series values.
    /// </summary>
    /// <remarks>
    /// Defaults to an array of <c>20</c> colors.
    /// </remarks>
    public string[] ChartPalette { get; set; } =
    [
        Colors.Blue.Accent3, Colors.Teal.Accent3, Colors.Amber.Accent3, Colors.Orange.Accent3, Colors.Red.Accent3,
            Colors.DeepPurple.Accent3, Colors.Green.Accent3, Colors.LightBlue.Accent3, Colors.Teal.Lighten1, Colors.Amber.Lighten1,
            Colors.Orange.Lighten1, Colors.Red.Lighten1, Colors.DeepPurple.Lighten1, Colors.Green.Lighten1, Colors.LightBlue.Lighten1,
            Colors.Amber.Darken2, Colors.Orange.Darken2, Colors.Red.Darken2, Colors.DeepPurple.Darken2, Colors.Gray.Darken2
    ];
}
