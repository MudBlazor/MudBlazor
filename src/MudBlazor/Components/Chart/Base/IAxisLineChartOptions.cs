// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

#nullable enable
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
