using MudBlazor.Utilities;

namespace MudBlazor
{
    #nullable enable
    public class SankeyChartNode
    {
        public string Name { get; set; } = string.Empty;
        public int Column { get; set; }
        public MudColor? Color { get; set; }

        public SankeyChartNode() { }
        public SankeyChartNode(string name, int column)
        {
            Name = name;
            Column = column;
        }
    }
}
