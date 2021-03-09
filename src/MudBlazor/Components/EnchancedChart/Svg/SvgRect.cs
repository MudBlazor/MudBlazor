// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Utilities;

namespace MudBlazor.Components.EnchancedChart.Svg
{
    public class SvgPolygonBasedRectangle
    {
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }
        public Point2D P3 { get; set; }
        public Point2D P4 { get; set; }
        public CssColor Fill { get; set; }

        public String GetPathValue() => $"{P1.X},{P1.Y} {P2.X},{P2.Y} {P3.X},{P3.Y} {P4.X},{P4.Y}";
    }
}
