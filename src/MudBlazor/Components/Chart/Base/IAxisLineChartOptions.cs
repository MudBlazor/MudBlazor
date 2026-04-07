// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

/// <summary>
/// Represents the options for a line chart that has axes.
/// </summary>
public interface IAxisLineChartOptions : IAxisChartOptions
{
    /// <summary>
    /// The width of lines, in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>3</c> for three pixels.
    /// </remarks>
    public double LineStrokeWidth { get; set; }

    /// <summary>
    /// Shows points at data points on the chart
    /// </summary>
    public bool ShowDataMarkers { get; set; }

    /// <summary>
    /// Shows zero point on vertical axis.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>
    /// </remarks>
    public bool YAxisRequireZeroPoint { get; set; }

    /// <summary>
    /// Prevent negative overshoots if all Y values are non-negative
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>
    /// </remarks>
    public bool ClampToZero { get; set; }

    /// <summary>
    /// The style of line to use for the chart <see cref="LineDisplayType.Line"/> or <see cref="LineDisplayType.Area"/>
    /// </summary>
    public LineDisplayType LineDisplayType { get; set; }

    /// <summary>
    /// The technique used to smooth lines.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="InterpolationOption.Straight"/>.
    /// </remarks>
    public InterpolationOption InterpolationOption { get; set; }

    /// <summary>
    /// Optional per-series display overrides.
    /// </summary>
    public IDictionary<IChartSeries, SeriesDisplayOverride> SeriesDisplayOverrides { get; set; }
}

/// <summary>
/// Represents display overrides for a specific series (line) in a line chart.
/// </summary>
public record SeriesDisplayOverride
{
    /// <summary>
    /// The display type for the series, overriding the chart's default.
    /// </summary>
    public LineDisplayType? LineDisplayType { get; set; }

    /// <summary>
    /// The interpolation option for the series, overriding the chart's default.
    /// </summary>
    public InterpolationOption? InterpolationOption { get; set; }

    /// <summary>
    /// The stroke opacity for the series.
    /// </summary>
    public double StrokeOpacity { get; set; } = 1;

    /// <summary>
    /// The fill opacity for the series, used when the display type is 'Area'.
    /// </summary>
    public double FillOpacity { get; set; } = 0.4;
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
