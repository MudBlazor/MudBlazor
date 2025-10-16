namespace MudBlazor
{
    /// <summary>
    /// Options for a single Sankey Chart edge.
    /// </summary>
    public class SankeyChartEdge
    {
        /// <summary>
        /// The name of the source <see cref="SankeyChartNode"/>.
        /// </summary>
        public string Source { get; set; } = string.Empty;

        /// <summary>
        /// The name of the target <see cref="SankeyChartNode"/>.
        /// </summary>
        public string Target { get; set; } = string.Empty;

        /// <summary>
        /// The weight i.e. the size of this edge.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="SankeyChartEdge"/> with default values.
        /// </summary>
        public SankeyChartEdge() { }

        /// <summary>
        /// Initialize a new instance of <see cref="SankeyChartEdge"/>.
        /// </summary>
        /// <param name="source">The name of the source <see cref="SankeyChartNode"/>.</param>
        /// <param name="target">The name of the target <see cref="SankeyChartNode"/>.</param>
        /// <param name="value">The weight i.e. the size of this edge.</param>
        public SankeyChartEdge(string source, string target, double value)
        {
            Source = source;
            Target = target;
            Value = value;
        }
    }
}
