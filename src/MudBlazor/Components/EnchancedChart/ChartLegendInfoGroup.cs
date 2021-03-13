// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public record ChartLegendInfoGroup(String name, IEnumerable<ChartLegendInfoSeries> series, Boolean displayGrouped);
}
