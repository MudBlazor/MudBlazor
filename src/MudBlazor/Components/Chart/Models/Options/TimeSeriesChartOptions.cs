// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Options specific to time series charts, extending <see cref="DefaultAxisLineChartOptions"/>.
/// </summary>
public class TimeSeriesChartOptions : DefaultAxisLineChartOptions
{
    /// <summary>
    /// Specifies the datetime format for timestamp labels. 
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"HH:mm"</c>.
    /// </remarks>
    public string TimeLabelFormat { get; set; } = "HH:mm";

    /// <summary>
    /// A way to have minimum spacing between timsetamp labels, default of 5 minutes.
    /// </summary>
    public TimeSpan TimeLabelSpacing { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Determines whether timestamp labels should be rounded to the nearest spacing value. 
    /// </summary>
    /// <remarks>
    /// Default is <c>false</c>.
    /// </remarks>
    public bool TimeLabelSpacingRounding { get; set; }

    /// <summary>
    /// Determines how timestamp labels are adjusted when <see cref="TimeLabelSpacingRounding"/> is enabled.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, the series is padded to allow rounding with labels before and after the series start and end.
    /// When <c>false</c>, labels are moved inward to align with the label spacing without altering the axis start and end times.
    /// </remarks>
    public bool TimeLabelSpacingRoundingPadSeries { get; set; }

    /// <summary>
    /// Specifies the DateTime format for timestamp labels in DataPoint marker tooltips. 
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"HH:mm"</c>.
    /// </remarks>
    public string TooltipTimeLabelFormat { get; set; } = "HH:mm";

    ///<inheritdoc/>
    public override string TooltipTitleFormat { get; set; } = "{{X_VALUE}}";

    ///<inheritdoc/>
    public override string? TooltipSubtitleFormat { get; set; } = "{{Y_VALUE}}";

    public static implicit operator TimeSeriesChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
