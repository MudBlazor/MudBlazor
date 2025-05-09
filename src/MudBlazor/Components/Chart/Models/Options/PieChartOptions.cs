// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using MudBlazor.Charts;

namespace MudBlazor;

public class PieChartOptions : DefaultRadialChartOptions
{
    /// <summary>
    /// The aggregation option to use for charts with multiple data sets.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="AggregationOption.GroupByLabel"/>
    /// </remarks>
    public override AggregationOption AggregationOption { get; set; } = AggregationOption.GroupByLabel;

    public static implicit operator PieChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
