// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Options specific to heatmap charts, extending <see cref="DefaultChartOptions"/>.
/// </summary>
public class HeatMapChartOptions : DefaultChartOptions
{
    /// <summary>
    /// Enables smooth color transitions for heatmap cells and removes all padding between cells. />
    /// Defaults to false
    /// </summary>
    public bool EnableSmoothGradient { get; set; } = false;

    /// <summary>
    /// The position of the X axis labels (top or bottom).
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="XAxisLabelPosition.Bottom"/>
    /// </remarks>
    public XAxisLabelPosition XAxisLabelPosition { get; set; } = XAxisLabelPosition.Bottom;

    /// <summary>
    /// The position of the Y axis labels (left or right).
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="YAxisLabelPosition.Left"/>
    /// </remarks>
    public YAxisLabelPosition YAxisLabelPosition { get; set; } = YAxisLabelPosition.Left;

    /// <summary>
    /// show tooltips for every box in the <see cref="ChartType.HeatMap"/>
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public override bool ShowToolTips { get; set; } = false;

    /// <summary>
    /// Show labels for every box in the <see cref="ChartType.HeatMap"/>
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>
    /// </remarks>
    public bool ShowLabels { get; set; } = true;

    /// <summary>
    /// Show label values for the legend boxes in the <see cref="ChartType.HeatMap"/>
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>
    /// </remarks>
    public bool ShowLegendLabels { get; set; } = false;

    /// <inheritdoc/>
    public override string TooltipTitleFormat { get; set; } = "{{Y_VALUE}}";

    /// <summary>
    /// The format applied to labels for every box in the <see cref="ChartType.HeatMap"/>
    /// Defaults to "F2"
    /// </summary>
    public string ValueFormatString { get; set; } = "F2";

    public static implicit operator HeatMapChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
