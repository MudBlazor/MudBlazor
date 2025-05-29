// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

public interface IRadialChartOptions : IChartOptions
{
    public AggregationOption AggregationOption { get; set; }

    public double FillOpacity { get; set; }
}

public interface IDataPointOptions
{
    public double StrokeWidth { get; set; }
    public bool ShowDataMarkers { get; set; }
    public double DataPointRadius { get; set; }
}
