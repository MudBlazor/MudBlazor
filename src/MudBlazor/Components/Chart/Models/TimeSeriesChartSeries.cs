#nullable enable
namespace MudBlazor
{
    public class TimeSeriesChartSeries
    {
        public record TimeValue(DateTime DateTime, double Value);

        public string Name { get; set; } = string.Empty;

        public List<TimeValue> Data { get; set; } = [];

        public bool IsVisible { get; set; } = true;

        public int Index { get; set; }

        [Obsolete("Use LineDisplayType instead. This will be removed in a future major version.", false)]
        public TimeSeriesDisplayType Type { get => (TimeSeriesDisplayType)LineDisplayType; set => LineDisplayType = (LineDisplayType)value; }
        public LineDisplayType LineDisplayType { get; set; } = LineDisplayType.Line;

        public double FillOpacity { get; set; } = 0.4;

        public double StrokeOpacity { get; set; } = 1;

        /// <summary>
        /// Shows points at datapoints on line and area charts.
        /// </summary>
        public bool ShowDataMarkers { get; set; }

        /// <summary>
        /// Tooltip title format for the series. Supported tags are {{SERIES_NAME}}, {{X_VALUE}} and {{Y_VALUE}}.
        /// </summary>
        public string DataMarkerTooltipTitleFormat { get; set; } = "{{X_VALUE}} - {{Y_VALUE}}";

        /// <summary>
        /// Tooltip subtitle format for the series. Supported tags are {{SERIES_NAME}}, {{X_VALUE}} and {{Y_VALUE}}.
        /// </summary>
        public string? DataMarkerTooltipSubtitleFormat { get; set; }
    }
}
