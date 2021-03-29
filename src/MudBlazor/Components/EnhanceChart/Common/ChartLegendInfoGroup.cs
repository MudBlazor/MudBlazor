using System;
using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{
    public record ChartLegendInfoGroup(String name, IEnumerable<ChartLegendInfoSeries> series, Boolean displayGrouped);
}
