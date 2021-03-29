using System;

namespace MudBlazor.EnhanceChart.Internal
{
    public class SvgText
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Value { get; set; }
        public Double Length { get; set; }
        public String Class { get; set; }
        public Double Height { get; set; }
        public String TextPlacement { get; set; } = "middle";
        public String Baseline { get; set; } = "middle";
        
    }
}
