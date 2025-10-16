// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

/// <summary>
/// Options specific to donut charts, extending <see cref="PieChartOptions"/>.
/// </summary>
public class DonutChartOptions : PieChartOptions
{
    /// <summary>
    /// The width of the donut ring as a ratio of the chart size.
    /// </summary>
    public double DonutRingRatio { get; set; } = 0.25;


    public static implicit operator DonutChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
