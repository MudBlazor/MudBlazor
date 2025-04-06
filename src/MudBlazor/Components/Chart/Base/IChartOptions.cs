// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Components.Chart;

public interface IChartOptions
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
    /// Shows vertical axis lines.
    /// </summary>
    public bool YAxisLines { get; set; }

    /// <summary>
    /// Shows horizontal axis lines.
    /// </summary>
    public bool XAxisLines { get; set; }

    /// <summary>
    /// Shows the chart series legend.
    /// </summary>
    public bool ShowLegend { get; set; }

    /// <summary>
    /// The list of colors applied to series values.
    /// </summary>
    public string[] ChartPalette { get; set; }

    /// <summary>
    /// Enables tooltips for values
    /// </summary>
    public bool ShowToolTips { get; set; }

    public string DefaultDataMarkerTooltipTitleFormat { get; set; }
}
