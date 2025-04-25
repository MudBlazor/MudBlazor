// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Components.Chart;

public class ChartDataSetComparer : IEqualityComparer<ChartDataSet>
{
    public bool Equals(ChartDataSet? x, ChartDataSet? y)
        => x?.Label == y?.Label;

    public int GetHashCode(ChartDataSet obj)
        => obj.Label?.GetHashCode() ?? 0;
}
