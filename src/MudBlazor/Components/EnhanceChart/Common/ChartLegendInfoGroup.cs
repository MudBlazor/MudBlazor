using System;
using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{
    public record ChartLegendInfoGroup(String Name, IEnumerable<ChartLegendInfoSeries> Series, Boolean DisplayGrouped);
}
