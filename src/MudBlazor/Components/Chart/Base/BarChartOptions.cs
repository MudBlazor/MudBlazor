// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Components.Chart;

public class BarChartOptions : DefaultChartOptions, IAxisChartOptions
{
    /// <summary>
    /// The labels applied to the horizontal axis.
    /// </summary>
    /// <remarks>
    /// The number of values in this array is typically equal to the number of values in the <see cref="ChartSeries.Data"/> property.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public string[] XAxisLabels { get; set; } = [];
}
