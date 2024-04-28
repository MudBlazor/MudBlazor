using System.Diagnostics;

namespace MudBlazor
{
    /// <summary>
    /// Represents a set of data to display in a <see cref="MudChart"/>.
    /// </summary>
    /// <remarks>
    /// This class is typically used to display multiple sets of values in a line, bar, or stacked bar chart.
    /// </remarks>
    [DebuggerDisplay("{Index} = {Name} (Visible={IsVisible})")]
    public class ChartSeries
    {
        /// <summary>
        /// Gets or sets the legend label for this series.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the values to display.
        /// </summary>
        /// <remarks>
        /// The number of values in this array is typically equal to the number of values in the <see cref="MudChart"/> <c>XAxisLabels</c> property.
        /// </remarks>
        public double[] Data { get; set; }

        /// <summary>
        /// Gets or sets whether this series is displayed in the chart.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the position of this series within a list.
        /// </summary>
        public int Index { get; set; }
    }
}
