using System;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public abstract record SegementChartToolTipInfo(String Label, Double Value, String Color, Double StartAngle, Double EndAngle, Double Radius) : ChartToolTipInfo(Label, Label, Color, Value);
    public record PieChartToolTipInfo(String Label, Double Value, String Color, Double StartAngle, Double EndAngle, Double Radius) : SegementChartToolTipInfo(Label, Value, Color, StartAngle, EndAngle, Radius);
    public record DonutChartToolTipInfo(String Label, Double Value, String Color, Double StartAngle, Double EndAngle, Double Radius) : SegementChartToolTipInfo(Label, Value, Color, StartAngle, EndAngle, Radius);

}
