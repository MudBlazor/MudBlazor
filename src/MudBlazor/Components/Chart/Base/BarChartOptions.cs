// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Components.Chart;

public class BarChartOptions : DefaultAxisChartOptions
{
    /// <summary>
    /// Specifies how bar groups are horizontally justified within the chart area.
    /// </summary>
    public Justify Justify { get; set; } = Justify.SpaceEvenly;

    /// <summary>
    /// Controls the amount of space between groups of bars (data sets).
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
    /// Defines the spacing between bars as a ratio of the group width, with a value between 0.0 and 1.0.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>0.10</c> (10%).
    /// </remarks>
    public double BarSpacingRatio { get; set; } = 0.10;

    /// <summary>
    /// Specifies the width of the bar stoke.
    /// </summary>
    /// <remarks>
    /// Overrides the <see cref="BarWidthRatio"/> and <see cref="BarSpacingRatio"/> setting.
    /// </remarks>
    public int? FixedBarWidth { get; set; }
}
