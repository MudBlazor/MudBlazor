#nullable enable
namespace MudBlazor.Components.Chart.Models
{
    public class TimeSeriesChartSeries
    {
        public record TimeValue(DateTime DateTime, double Value);

        public string Name { get; set; } = string.Empty;

        public List<TimeValue> Data { get; set; } = [];

        public bool IsVisible { get; set; } = true;

        public int Index { get; set; }

        public TimeSeriesDiplayType Type { get; set; } = TimeSeriesDiplayType.Line;

        public double FillOpacity { get; set; } = 0.4;
        public double StrokeOpacity { get; set; } = 1;
    }

    // TODO: v8 rename to TimeSeriesDisplayType (typo)
    public enum TimeSeriesDiplayType
    {
        Line,
        Area,
    }
}
