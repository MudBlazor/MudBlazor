using System;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public record BarChartToolTipInfo(String XLabel, Double Value, String SeriesName, MudColor Color, String DataSetSeriesName, Point2D P1, Point2D P2, Point2D P3, Point2D P4) : ChartToolTipInfo(XLabel, SeriesName, Color, Value);
}
