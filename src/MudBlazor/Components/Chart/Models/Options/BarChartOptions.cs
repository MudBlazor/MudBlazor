// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using MudBlazor.Charts;

namespace MudBlazor;

public class BarChartOptions : DefaultBarChartOptions
{
    /// <summary>
    /// Defines the spacing between bars as a ratio of the group width, with a value between 0.0 and 1.0.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0.20</c> (20%).
    /// </remarks>
    public double BarSpacingRatio { get; set; } = 0.20;

    public static implicit operator BarChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
