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
    /// A value between 0.1 and 1.0 indicating how much of the chart width should occupied by the bars.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>1</c> (100%).
    /// Only applies when using <see cref="Justify.FlexStart"/>, <see cref="Justify.Center"/> or <see cref="Justify.FlexEnd"/>
    /// </remarks>
    public double SpacingRatio { get; set; } = 1;
}
