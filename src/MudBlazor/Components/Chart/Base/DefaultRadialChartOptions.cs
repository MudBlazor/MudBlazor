// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Components.Chart;

public abstract class DefaultRadialChartOptions : DefaultChartOptions, IRadialChartOptions
{
    /// <summary>
    /// The aggregation option to use for charts with multiple data sets.
    /// </summary>
    public virtual AggregationOption AggregationOption { get; set; }

    /// <summary>
    /// Show the series value as a percentage of the total.
    /// </summary>
    public bool ShowAsPercentage { get; set; }
}
