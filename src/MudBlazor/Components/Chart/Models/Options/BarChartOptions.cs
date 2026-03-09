// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Options specific to bar charts, extending <see cref="DefaultBarChartOptions"/>.
/// </summary>
public class BarChartOptions : DefaultBarChartOptions
{
    private double _barSpacingRatio = 0.20;
    /// <summary>
    /// Defines the spacing between bars as a ratio of the group width, with a value between 0.0 and 1.0.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0.20</c> (20%).
    /// </remarks>
    public double BarSpacingRatio
    {
        get => _barSpacingRatio;
        set => _barSpacingRatio = Math.Clamp(value, 0.0, 1.0);
    }

    /// <summary>
    /// Converts a <see cref="ChartOptions"/> instance to a <see cref="BarChartOptions"/> instance.
    /// </summary>
    /// <param name="options">The <see cref="ChartOptions"/> instance to convert. Cannot be <see langword="null"/>.</param>
    public static implicit operator BarChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
