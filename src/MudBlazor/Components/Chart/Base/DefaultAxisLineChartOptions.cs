// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor.Charts;

public abstract class DefaultAxisLineChartOptions : DefaultAxisChartOptions, IAxisLineChartOptions
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
    public IDictionary<ChartSeries, SeriesDisplayOverride> SeriesDisplayOverrides
    {
        get => _seriesDisplayOverrides;
        set
        {
            _seriesDisplayOverrides = new Dictionary<ChartSeries, SeriesDisplayOverride>(value, ChartDataSetComparer.Instance);
        }
    }

    private Dictionary<ChartSeries, SeriesDisplayOverride> _seriesDisplayOverrides = new(ChartDataSetComparer.Instance);

    private sealed class ChartDataSetComparer : IEqualityComparer<ChartSeries>
    {
        public static readonly ChartDataSetComparer Instance = new();

        private ChartDataSetComparer() { }

        public bool Equals(ChartSeries? x, ChartSeries? y)
            => x?.Name == y?.Name;

        public int GetHashCode(ChartSeries obj)
            => obj.Name?.GetHashCode() ?? 0;
    }
}
