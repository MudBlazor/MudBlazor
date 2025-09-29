using MudBlazor.Utilities;

#nullable enable

namespace MudBlazor
{
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

        public SankeyChartNode() { }
        public SankeyChartNode(string name, int column)
        {
            Name = name;
            Column = column;
        }
    }
}
