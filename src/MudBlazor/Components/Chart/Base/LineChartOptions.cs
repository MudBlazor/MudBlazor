// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Components.Chart;

public class LineChartOptions : DefaultAxisLineChartOptions, IAxisLineChartOptions
{
    public override string TooltipTitleFormat { get; set; } = "{{Y_VALUE}}";
}
