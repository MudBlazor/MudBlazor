// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

public interface IMudChart
{
    List<ChartSeries> ChartSeries { get; set; }
    ChartType ChartType { get; }

    void RebuildChart();
}
