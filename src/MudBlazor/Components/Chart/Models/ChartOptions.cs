using System;
using System.Collections.Generic;
using System.Text;

namespace MudBlazor
{
    public class ChartOptions
    {
        public int YAxisTicks { get; set; } = 20;
        public bool YAxisLines { get; set; } = true;
        public bool XAxisLines { get; set; }
        public bool DisableLegend { get; set; }
    }
}
