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
        public string[] ChartPalette { get; set; } = { Colors.Blue.Accent3, Colors.Teal.Accent3, Colors.Amber.Accent3, Colors.Orange.Accent3, Colors.Red.Accent3, Colors.DeepPurple.Accent3, Colors.Green.Accent3, Colors.LightBlue.Accent3 };
    }
}
