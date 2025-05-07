// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

public class DonutChartOptions : PieChartOptions
{
    /// <summary>
    /// The width of the donut hole as a ratio of the chart size.
    /// </summary>
    public double DonutHoleRatio { get; set; } = 0.5;

}
