using System;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public record ChartLegendInfoSeries(String Name, MudColor Color,Boolean IsEnabled, IDataSeries Series);
    public record ChartLegendInfoPoint(String Name, MudColor Color,Boolean IsEnabled, IDataPoint Point);

}
