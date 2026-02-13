// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

/// <summary>
/// Represents the options for a chart.
/// </summary>
public interface IChartOptions
{
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

    /// <summary>
    /// The format string for tooltip titles.
    /// </summary>
    public string TooltipTitleFormat { get; set; }

    /// <summary>
    /// The format string for tooltip subtitles.
    /// </summary>
    public string? TooltipSubtitleFormat { get; set; }
}
