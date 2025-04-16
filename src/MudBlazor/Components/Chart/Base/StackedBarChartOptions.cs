// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Components.Chart;

public class StackedBarChartOptions : DefaultAxisChartOptions
{
    /// <summary>
    /// Specifies how bar groups are horizontally justified within the chart area.
    /// </summary>
    public Justify Justify { get; set; } = Justify.SpaceEvenly;

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
}
