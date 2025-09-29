namespace MudBlazor
{
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

        public SankeyChartEdge() { }
        public SankeyChartEdge(string source, string target, double value)
        {
            Source = source;
            Target = target;
            Value = value;
        }
    }
}
