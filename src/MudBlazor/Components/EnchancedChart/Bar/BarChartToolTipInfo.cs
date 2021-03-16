// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Utilities;

namespace MudBlazor.Components.EnchancedChart.Bar
{
    public record BarChartToolTipInfo(String XLabel, Double Value, String SeriesName, String Color, String DataSetSeriesName, Point2D P1, Point2D P2, Point2D P3, Point2D P4) : ChartToolTipInfo(XLabel, SeriesName, Color, Value);
}
