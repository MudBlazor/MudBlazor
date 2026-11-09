// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Options specific to radar charts, extending <see cref="DefaultRadialChartOptions"/>.
/// </summary>
public class RadarChartOptions : DefaultRadialChartOptions, IRadialChartOptions, IHasDataPointOptions
{
    /// <summary>
    /// Gets or sets the starting angle offset for the first axis, in degrees.
    /// Default is 0  (starts at the 12 o'clock position).
    /// </summary>
    public double AngleOffset { get; set; } = 0;

    /// <summary>
    /// If true, grid lines will be shown.
    /// Default is true.
    /// </summary>
    public bool ShowGridLines { get; set; } = true;

    /// <summary>
    /// Sets the color of the grid lines.
    /// Default is "var(--mud-palette-divider)".
    /// </summary>
    public string GridLineColor { get; set; } = "var(--mud-palette-divider)";

    /// <summary>
    /// Sets the stroke width of the grid lines.
    /// Default is 1.0.
    /// </summary>
    public double GridLineWidth { get; set; } = 1.0;

    /// <summary>
    /// Number of concentric grid levels (rings) to display.
    /// Default is 5.
    /// </summary>
    public int GridLevels { get; set; } = 5;

    /// <summary>
    /// If true, labels for each axis will be shown.
    /// Default is true.
    /// </summary>
    public bool ShowAxisLabels { get; set; } = true;

    /// <summary>
    /// Whether axis values should be displayed.
    /// </summary>
    public bool ShowAxisValues { get; set; } = true;

    /// <summary>
    /// Sets the color of the axis lines.
    /// Default is "var(--mud-palette-lines-inputs)".
    /// </summary>
    public string AxisLineColor { get; set; } = "var(--mud-palette-lines-inputs)";

    /// <summary>
    /// Sets the stroke width of the axis lines.
    /// Default is 1.0.
    /// </summary>
    public double AxisLineWidth { get; set; } = 1.0;

    /// <summary>
    /// Opacity of the filled area for each series. Value between 0 (transparent) and 1 (opaque).
    /// </summary>
    /// <remarks>
    /// Default is 0.4.
    /// </remarks>
    public override double FillOpacity { get; set; } = 0.4;

    /// <summary>
    /// Stroke width of the series lines.
    /// Default is 2.0.
    /// </summary>
    public double StrokeWidth { get; set; } = 2.0;

    /// <summary>
    /// If true, circles will be drawn at each data point on the series lines.
    /// Default is true.
    /// </summary>
    public bool ShowDataMarkers { get; set; } = true;

    /// <summary>
    /// Radius of the circles drawn at each data point, if ShowDataPoints is true.
    /// Default is 3.0.
    /// </summary>
    public double DataPointRadius { get; set; } = 3.0;

    /// <summary>
    /// The suggested maximum value for the axis.
    /// </summary>
    public double? AxisSuggestedMax { get; set; }

    /// <summary>
    /// The format applied to numbers on the axis.
    /// </summary>
    public string? AxisFormat { get; set; }

    /// <summary>
    /// Custom formatting function for radar axis values.
    /// If set, this function will be used to convert radial axis values to strings for display purposes.
    /// If not provided, <see cref="AxisFormat"/> will be used instead.
    /// </summary>
    public Func<double, string>? AxisToStringFunc { get; set; }

    /// <inheritdoc/>
    public override AggregationOption AggregationOption { get; set; } = AggregationOption.GroupByDataSet;

    public static implicit operator RadarChartOptions(ChartOptions options) => new()
    {
        ShowLegend = options.ShowLegend,
        ShowToolTips = options.ShowToolTips,
        TooltipTitleFormat = options.TooltipTitleFormat,
        TooltipSubtitleFormat = options.TooltipSubtitleFormat,
        ChartPalette = options.ChartPalette,
    };
}
