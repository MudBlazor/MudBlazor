// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Options specific to stacked bar charts, extending <see cref="DefaultBarChartOptions"/>.
/// </summary>
public class StackedBarChartOptions : DefaultBarChartOptions
{
    /// <inheritdoc />
    public override double SeriesSpacingRatio { get; set; } = 0.8;

    /// <summary>
    /// Determines whether to show zero values
    /// </summary>
    /// <remarks>Defaults to <c>true</c></remarks>
    public bool ShowZeroValues { get; set; } = true;

    /// <summary>
    /// Converts a <see cref="ChartOptions"/> instance to a <see cref="StackedBarChartOptions"/> instance.
    /// </summary>
    /// <param name="options">The <see cref="ChartOptions"/> instance to convert. Cannot be <see langword="null"/>.</param>
    public static implicit operator StackedBarChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
