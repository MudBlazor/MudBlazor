// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Options specific to line charts, extending <see cref="DefaultAxisLineChartOptions"/>.
/// </summary>
public class LineChartOptions : DefaultAxisLineChartOptions, IAxisLineChartOptions
{
    public override string TooltipTitleFormat { get; set; } = "{{Y_VALUE}}";

    public static implicit operator LineChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
