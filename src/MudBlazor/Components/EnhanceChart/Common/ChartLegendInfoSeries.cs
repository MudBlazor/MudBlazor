using System;

namespace MudBlazor.EnhanceChart
{
    public record ChartLegendInfoSeries(String Name, String Color,Boolean IsEnabled, IDataSeries Series);
    
}
