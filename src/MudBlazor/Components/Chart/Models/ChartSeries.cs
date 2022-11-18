using System.Collections.Generic;

namespace MudBlazor
{
    public class ChartSeries
    {
        public string Name { get; set; }

        public ICollection<decimal> Data { get; set; }
    }
}
