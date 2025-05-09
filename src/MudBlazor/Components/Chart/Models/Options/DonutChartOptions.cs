// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public class DonutChartOptions : PieChartOptions
{
    /// <summary>
    /// The width of the donut hole as a ratio of the chart size.
    /// </summary>
    public double DonutHoleRatio { get; set; } = 0.5;

    public static implicit operator DonutChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
