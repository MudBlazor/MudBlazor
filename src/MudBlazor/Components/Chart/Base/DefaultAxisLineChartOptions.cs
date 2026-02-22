// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Charts;

/// <summary>
/// Represents the default options for a line chart.
/// </summary>
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
    public IDictionary<IChartSeries, SeriesDisplayOverride> SeriesDisplayOverrides
    {
        get => _seriesDisplayOverrides;
        set
        {
            _seriesDisplayOverrides = new Dictionary<IChartSeries, SeriesDisplayOverride>(value, ChartDataSetComparer.Instance);
        }
    }

    private Dictionary<IChartSeries, SeriesDisplayOverride> _seriesDisplayOverrides = new(ChartDataSetComparer.Instance);

    /// <summary>
    /// A comparer for chart data series.
    /// </summary>
    private sealed class ChartDataSetComparer : IEqualityComparer<IChartSeries>
    {
        /// <summary>
        /// The singleton instance of the comparer.
        /// </summary>
        public static readonly ChartDataSetComparer Instance = new();

        private ChartDataSetComparer() { }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>True if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(IChartSeries? x, IChartSeries? y)
            => x?.Name == y?.Name && x?.Visible == y?.Visible;

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The object for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(IChartSeries obj)
            => obj.Name?.GetHashCode() ?? 0;
    }
}
