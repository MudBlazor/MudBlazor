using System.Diagnostics;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// Represents a set of data to display in a <see cref="MudChart"/>.
    /// </summary>
    /// <remarks>
    /// This class is typically used to display multiple sets of values in a line, bar, or stacked bar chart.
    /// </remarks>
    [DebuggerDisplay("{Index} = {Name} (Visible={Visible})")]
    public class ChartSeries
    {
        /// <summary>
        /// The legend label for this series.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The values to display.
        /// </summary>
        /// <remarks>
        /// The number of values in this array is typically equal to the number of values in the <see cref="MudChart"/> <c>XAxisLabels</c> property.
        /// </remarks>
        public double[] Data { get; set; } = [];

        /// <summary>
        /// Displays this series in the chart.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// The position of this series within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Shows points at datapoints on line and area charts.
        /// </summary>
        public bool ShowDataMarkers { get; set; }

        /// <summary>
        /// Tooltip title format for the series. Supported tags are {{SERIES_NAME}}, {{X_VALUE}} and {{Y_VALUE}}.
        /// </summary>
        public string DataMarkerTooltipTitleFormat { get; set; } = "{{Y_VALUE}}";

        /// <summary>
        /// Tooltip subtitle format for the series. Supported tags are {{SERIES_NAME}}, {{X_VALUE}} and {{Y_VALUE}}.
        /// </summary>
        public string? DataMarkerTooltipSubtitleFormat { get; set; }

        public LineDisplayType LineDisplayType { get; set; }

        public double FillOpacity { get; set; } = 0.4;
    }
}
