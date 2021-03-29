using System.Collections.Generic;

namespace MudBlazor.EnhanceChart
{
    public record ChartLegendInfo(IEnumerable<ChartLegendInfoGroup> Groups);
}
