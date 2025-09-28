namespace MudBlazor
{
    public class SankeyChartEdge
    {
        public string Source { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public double Value { get; set; }
        public double Opacity { get; set; } = 1;

        public SankeyChartEdge() { }
        public SankeyChartEdge(string source, string target, double value)
        {
            Source = source;
            Target = target;
            Value = value;
        }
    }
}
