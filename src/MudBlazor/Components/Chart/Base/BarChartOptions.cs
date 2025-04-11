// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Components.Chart;

public class BarChartOptions : DefaultAxisChartOptions
{
    /// <summary>
    /// Controls how bar groups are distributed across the available space
    /// </summary>
    public Justify Justify { get; set; } = Justify.SpaceEvenly;

    /// <summary>
    /// Specifies the proportion of horizontal space the chart should occupy, ranging from 0.01 to 1.0.
    /// </summary>
    /// <remarks>
    /// The default value is <c>1</c> (100%).  
    /// This setting is only applicable when using <see cref="Justify.FlexStart"/>, <see cref="Justify.Center"/>, or <see cref="Justify.FlexEnd"/>.
    /// </remarks>
    public double SpacingRatio { get; set; } = 1;

    /// <summary>
    /// Defines the relative width of the bars compared to the total available space, with a value between 0.01 and 1.0.
    /// </summary>
    /// <remarks>
    /// This setting is only applicable when using <see cref="Justify.FlexStart"/>, <see cref="Justify.Center"/>, or <see cref="Justify.FlexEnd"/>.
    /// </remarks>
    public double BarWidthRatio { get; set; } = .20;

    /// <summary>
    /// Defines the spacing between bars as a ratio of the group width, with a value between 0.0 and 1.0.
    /// </summary>
    public double BarInnerGapRatio { get; set; } = 0.40;
}
