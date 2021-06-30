using System;
using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{
    public record DataSeriesBasedChartLegendInfoGroup(String Name, IEnumerable<ChartLegendInfoSeries> Series, Boolean DisplayGrouped) : IChartLegendInfoGroup;
    public record DataPointBasedChartLegendInfoGroup(IEnumerable<ChartLegendInfoPoint> Points) : IChartLegendInfoGroup;
}
