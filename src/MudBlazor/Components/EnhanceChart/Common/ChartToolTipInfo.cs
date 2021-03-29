using System;

namespace MudBlazor.EnhanceChart
{
    public abstract record ChartToolTipInfo(String XLabel, String SeriesName, String Color, Double Value);
}
