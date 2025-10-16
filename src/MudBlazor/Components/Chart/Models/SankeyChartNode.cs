using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor
{
    /// <summary>
    /// Options for a single Sankey Chart node.
    /// </summary>
    public class SankeyChartNode
    {
        /// <summary>
        /// The name of this node.
        /// </summary>
        /// <remarks>
        /// Note that every node <b>must have a unique name</b>.
        /// </remarks>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The column in which to display this node.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// The color of this node. Picks colors from <see cref="ChartOptions.ChartPalette"/> if set to <c>null</c>.
        /// </summary>
        public MudColor? Color { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="SankeyChartNode"/> with default values.
        /// </summary>
        public SankeyChartNode() { }

        /// <summary>
        ///  Initializes a new instance of <see cref="SankeyChartNode"/>.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="column">The column in which to display this node.</param>
        public SankeyChartNode(string name, int column)
        {
            Name = name;
            Column = column;
        }
    }
}
