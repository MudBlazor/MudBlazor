// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Charts;

/// <summary>
/// Represents the default options for a radial chart.
/// </summary>
public abstract class DefaultRadialChartOptions : DefaultChartOptions, IRadialChartOptions
{
    /// <summary>
    /// The aggregation option to use for charts with multiple data sets.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="AggregationOption.GroupByLabel"/>
    /// </remarks>
    public virtual AggregationOption AggregationOption { get; set; } = AggregationOption.GroupByLabel;

    /// <summary>
    /// Gets or sets the opacity level of the fill, where 0 represents fully transparent and 1 represents fully opaque.
    /// </summary>
    /// <remarks>
    /// Defaults to 1 (fully opaque).
    /// </remarks>
    public virtual double FillOpacity { get; set; } = 1;

    /// <summary>
    /// Show the series value as a percentage of the total.
    /// </summary>
    public bool ShowAsPercentage { get; set; }
}
