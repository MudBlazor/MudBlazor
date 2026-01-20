// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Charts;

/// <summary>
/// Represents the default options for a chart.
/// </summary>
public abstract class DefaultChartOptions : IChartOptions
{
    /// <summary>
    /// Shows the chart series legend.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public bool ShowLegend { get; set; } = true;

    /// <summary>
    /// Enables tooltips for values
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    public virtual bool ShowToolTips { get; set; } = true;

    /// <summary>
    /// The format applied to the data marker tooltip title.
    /// </summary>
    /// <remarks>
    /// Defaults to "{{Y_VALUE}} - {{X_VALUE}}"
    /// </remarks>
    public virtual string TooltipTitleFormat { get; set; } = "{{Y_VALUE}} - {{X_VALUE}}";

    /// <summary>
    /// The format applied to the data marker tooltip subtitle.
    /// </summary>
    public virtual string? TooltipSubtitleFormat { get; set; }

    /// <summary>
    /// The list of colors applied to series values.
    /// </summary>
    /// <remarks>
    /// Defaults to an array of <c>20</c> colors.
    /// </remarks>
    public string[] ChartPalette { get; set; } =
    [
        Colors.Blue.Accent3, Colors.Teal.Accent3, Colors.Amber.Accent3, Colors.Orange.Accent3, Colors.Red.Accent3,
            Colors.DeepPurple.Accent3, Colors.Green.Accent3, Colors.LightBlue.Accent3, Colors.Teal.Lighten1, Colors.Amber.Lighten1,
            Colors.Orange.Lighten1, Colors.Red.Lighten1, Colors.DeepPurple.Lighten1, Colors.Green.Lighten1, Colors.LightBlue.Lighten1,
            Colors.Amber.Darken2, Colors.Orange.Darken2, Colors.Red.Darken2, Colors.DeepPurple.Darken2, Colors.Gray.Darken2
    ];
}
