// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Options specific to scatter plot charts, extending <see cref="DefaultAxisLineChartOptions"/>.
/// </summary>
public class ScatterPlotChartOptions : DefaultAxisLineChartOptions, IAxisLineChartOptions
{
    /// <summary>
    /// The radius of each data point marker, in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>5</c>.
    /// </remarks>
    public double PointRadius { get; set; } = 5;

    /// <summary>
    /// The spacing between tick marks on the X axis.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>20</c>.
    /// </remarks>
    public int XAxisTicks { get; set; } = 20;

    /// <summary>
    /// The maximum number of X-axis tick marks allowed.
    /// </summary>
    /// <remarks>
    /// If the number of ticks calculated exceeds this value, the tick marks will automatically be thinned out.
    /// Defaults to <c>20</c>.
    /// </remarks>
    public int MaxNumXAxisTicks { get; set; } = 20;

    /// <summary>
    /// The format applied to numbers on the horizontal axis.
    /// </summary>
    public string? XAxisFormat { get; set; }

    /// <summary>
    /// When <c>true</c>, renders a text label next to each scatter data point showing its X and Y values.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    public bool ShowDataLabels { get; set; }

    public override string TooltipTitleFormat { get; set; } = "{{X_VALUE}}, {{Y_VALUE}}";

    public static implicit operator ScatterPlotChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}

public record ScatterSeriesDisplayOverride : SeriesDisplayOverride
{
    /// <summary>
    /// Determines how this series is rendered on a scatter plot chart.
    /// Defaults to <see cref="ScatterSeriesType.Points"/>.
    /// </summary>
    /// <remarks>
    /// Set to <see cref="ScatterSeriesType.Line"/> to render this series as a line (e.g. a regression line)
    /// overlaid on scatter point series.
    /// </remarks>
    public ScatterSeriesType ScatterSeriesType { get; set; } = ScatterSeriesType.Points;
}

/// <summary>
/// Determines how a series is rendered on a <see cref="Charts.ScatterPlot{T}"/> chart.
/// </summary>
public enum ScatterSeriesType
{
    /// <summary>
    /// Renders data as individual scatter point markers (default).
    /// </summary>
    Points,

    /// <summary>
    /// Renders data as a continuous line, useful for regression lines or trend overlays.
    /// </summary>
    Line
}
