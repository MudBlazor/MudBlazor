using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{
    public interface IChartLegendInfoGroup
    {

    }

    public record ChartLegendInfo(IEnumerable<IChartLegendInfoGroup> Groups);
}
