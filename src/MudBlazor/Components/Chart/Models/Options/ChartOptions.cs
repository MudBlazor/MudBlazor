// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

public sealed class ChartOptions : DefaultChartOptions, IChartOptions
{
    public override string TooltipTitleFormat { get; set; } = "{{Y_VALUE}}";
}
