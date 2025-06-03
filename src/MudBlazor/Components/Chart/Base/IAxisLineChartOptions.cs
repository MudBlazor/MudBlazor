// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

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
    public IDictionary<ChartSeries, SeriesDisplayOverride> SeriesDisplayOverrides { get; set; }
}

public record SeriesDisplayOverride
{
    public LineDisplayType? LineDisplayType { get; set; }
    public InterpolationOption? InterpolationOption { get; set; }
    public double StrokeOpacity { get; set; } = 1;
    public double FillOpacity { get; set; } = 0.4;
}
