// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Charts;

public class PieChartOptions : DefaultRadialChartOptions
{
    /// <summary>
    /// The aggregation option to use for charts with multiple data sets.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="AggregationOption.GroupByLabel"/>
    /// </remarks>
    public override AggregationOption AggregationOption { get; set; } = AggregationOption.GroupByLabel;
}
