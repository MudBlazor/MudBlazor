using System;
using System.Collections.Generic;

namespace MudBlazor.Components.Chart.Models
{
    public class TimeSeriesChartSeries
    {
        public record TimeValue(DateTime DateTime, double Value);

        public string Name { get; set; } = string.Empty;

        public List<TimeValue> Data { get; set; } = new();

        public bool IsVisible { get; set; } = true;

        public int Index { get; set; }

        public TimeSeriesDiplayType Type { get; set; } = TimeSeriesDiplayType.Line;

        public double FillOpacity { get; set; } = 0.4;
        public double StrokeOpacity { get; set; } = 1;
    }

    public enum TimeSeriesDiplayType
    {
        Line,
        Area,
    }
}
