// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MudBlazor.Components.EnchancedChart;

namespace MudBlazor
{
    public record ChartLegendInfoSeries(String Name, String Color,Boolean IsEnabled, IDataSeries Series);
    
}
