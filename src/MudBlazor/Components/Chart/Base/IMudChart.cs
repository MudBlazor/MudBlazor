// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;

namespace MudBlazor.Charts;

public interface IMudChart<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    List<ChartSeries<T>> ChartSeries { get; set; }
    string[] LegendPalette { get; }
    ChartType ChartType { get; }

    void RebuildChart();
}
