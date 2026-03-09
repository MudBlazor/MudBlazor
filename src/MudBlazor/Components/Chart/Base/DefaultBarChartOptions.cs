// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

/// <summary>
/// Represents the default options for a bar chart.
/// </summary>
public abstract class DefaultBarChartOptions : DefaultAxisChartOptions
{
    private double _seriesSpacingRatio = 1;
    private double _barWidthRatio = 0.40;

    /// <summary>
    /// Specifies how bar groups are horizontally justified within the chart area.
    /// </summary>
    public Justify Justify { get; set; } = Justify.SpaceBetween;

    /// <summary>
    /// Controls the amount of space between data sets.
    /// This value, between 0.1 and 1.0 is multiplied against the available space to calculate spacing.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c> (100%). 
    /// This setting is only applicable when using <see cref="Justify.FlexStart"/>, <see cref="Justify.Center"/>, or <see cref="Justify.FlexEnd"/>.
    /// </remarks>
    public virtual double SeriesSpacingRatio
    {
        get => _seriesSpacingRatio;
        set => _seriesSpacingRatio = Math.Clamp(value, 0.1, 1.0);
    }

    /// <summary>
    /// Determines the proportion of horizontal space allocated to each bar group, relative to the available tick width.
    /// Value should be between 0.01 and 1.0.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0.40</c> (40%).
    /// </remarks>
    public double BarWidthRatio
    {
        get => _barWidthRatio;
        set => _barWidthRatio = Math.Clamp(value, 0.01, 1.0);
    }

    /// <summary>
    /// Specifies the width of the bar stoke.
    /// </summary>
    /// <remarks>
    /// Overrides the <see cref="BarWidthRatio"/> setting.
    /// </remarks>
    public int? FixedBarWidth { get; set; }

    /// <summary>
    /// The format applied to the data marker tooltip title.
    /// </summary>
    /// <remarks>
    /// Defaults to "{{Y_VALUE}}"
    /// </remarks>
    public override string TooltipTitleFormat { get; set; } = "{{Y_VALUE}}";
}
