// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Components.Chart;

public class LineChartOptions : DefaultAxisChartOptions
{
    /// <summary>
    /// The width of lines, in pixels.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>3</c> for three pixels.
    /// </remarks>
    public double LineStrokeWidth { get; set; } = 3;

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
    public InterpolationOption InterpolationOption { get; set; } = InterpolationOption.Straight;

    /// <summary>
    /// Optional per-series display overrides.
    /// </summary>
    public Dictionary<ChartDataSet, LineChartSeriesDisplayOverride> SeriesDisplayOverrides
    {
        get => _seriesDisplayOverrides;
        set
        {
            _seriesDisplayOverrides = new Dictionary<ChartDataSet, LineChartSeriesDisplayOverride>(value, new ChartDataSetComparer());
        }
    }

    private Dictionary<ChartDataSet, LineChartSeriesDisplayOverride> _seriesDisplayOverrides = new(new ChartDataSetComparer());
}

public class LineChartSeriesDisplayOverride
{
    public LineDisplayType? LineDisplayType { get; set; }
    public InterpolationOption? InterpolationOption { get; set; }
}
