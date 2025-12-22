// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;
#nullable enable

/// <summary>
/// Represents configuration options for a chart, including tooltip formatting.
/// </summary>
/// <remarks>
/// This class provides customization options for chart behavior and appearance.
/// </remarks>
public sealed class ChartOptions : DefaultChartOptions, IChartOptions
{
    /// <summary>
    /// The format applied to the data marker tooltip title.
    /// </summary>
    public override string TooltipTitleFormat { get; set; } = "{{Y_VALUE}}";
}
