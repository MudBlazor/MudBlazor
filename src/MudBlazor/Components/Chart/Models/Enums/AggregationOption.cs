// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

#nullable enable
/// <summary>
/// Specifies the options for aggregating data in a dataset.
/// </summary>
public enum AggregationOption
{
    /// <summary>
    /// No aggregation is applied;
    /// </summary>
    None,
    /// <summary>
    /// Aggregate data based on the dataset
    /// </summary>
    GroupByDataSet,
    /// <summary>
    /// Aggregate data based on labels
    /// </summary>
    GroupByLabel,
}
