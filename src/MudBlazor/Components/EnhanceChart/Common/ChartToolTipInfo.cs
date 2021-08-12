using System;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public abstract record ChartToolTipInfo(String XLabel, String SeriesName, MudColor Color, Double Value);
}
