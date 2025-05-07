// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

public abstract class DefaultBarChartOptions : DefaultAxisChartOptions
{
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
    public double SeriesSpacingRatio { get; set; } = 1;

    /// <summary>
    /// Determines the proportion of horizontal space allocated to each bar group, relative to the available tick width.
    /// Value should be between 0.01 and 1.0.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0.20</c> (20%).
    /// </remarks>
    public double BarWidthRatio { get; set; } = 0.20;

    /// <summary>
    /// Specifies the width of the bar stoke.
    /// </summary>
    /// <remarks>
    /// Overrides the <see cref="BarWidthRatio"/> setting.
    /// </remarks>
    public int? FixedBarWidth { get; set; }

    public override string TooltipTitleFormat { get; set; } = "{{Y_VALUE}}";
}
