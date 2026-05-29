// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Options specific to rose charts, extending <see cref="DefaultRadialChartOptions"/>.
/// </summary>
public class RoseChartOptions : DefaultRadialChartOptions, IHasValueLabelOptions
{
    /// <summary>
    /// Gets or sets the starting angle offset for the first sector, in degrees.
    /// </summary>
    /// <remarks>
    /// Default is 0 (starts at the 3 o'clock position).
    /// </remarks>
    public double AngleOffset { get; set; } = 0;

    /// <summary>
    /// Gets or sets the scaling factor for the radii of the sectors.
    /// </summary>
    /// <remarks>
    /// A value of 1 means the largest sector will touch the chart's radius.
    /// Default is 0.9.
    /// </remarks>
    public double ScaleFactor { get; set; } = 0.9;

    /// <summary>
    /// Whether values should be displayed within the chart.
    /// </summary>
    public bool ShowValues { get; set; }

    public static implicit operator RoseChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
