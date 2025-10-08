// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

/// <summary>
/// Represents the options for a radial chart.
/// </summary>
public interface IRadialChartOptions : IChartOptions
{
    /// <summary>
    /// The aggregation function to use for the data.
    /// </summary>
    /// <remarks>
    /// Refer to <see cref="AggregationOption"/>
    /// </remarks>
    public AggregationOption AggregationOption { get; set; }

    /// <summary>
    /// The opacity of the fill color.
    /// </summary>
    public double FillOpacity { get; set; }
}

/// <summary>
/// Represents options for data points.
/// </summary>
public interface IHasDataPointOptions
{
    /// <summary>
    /// The width of the stroke for data points.
    /// </summary>
    public double StrokeWidth { get; set; }

    /// <summary>
    /// Whether to show data markers.
    /// </summary>
    public bool ShowDataMarkers { get; set; }

    /// <summary>
    /// The radius of data points.
    /// </summary>
    public double DataPointRadius { get; set; }
}

/// <summary>
/// Represents options for value labels.
/// </summary>
public interface IHasValueLabelOptions
{
    /// <summary>
    /// Whether to show value labels.
    /// </summary>
    public bool ShowValues { get; set; }
}
