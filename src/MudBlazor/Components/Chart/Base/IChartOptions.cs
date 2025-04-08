// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Components.Chart;

public interface IChartOptions
{
    /// <summary>
    /// Shows the chart series legend.
    /// </summary>
    public bool ShowLegend { get; set; }

    /// <summary>
    /// The list of colors applied to series values.
    /// </summary>
    public string[] ChartPalette { get; set; }

    /// <summary>
    /// Enables tooltips for values
    /// </summary>
    public bool ShowToolTips { get; set; }

    public string TooltipTitleFormat { get; set; }
}
