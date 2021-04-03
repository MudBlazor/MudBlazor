using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.EnhanceChart
{
    public partial class MudEnhancedBarChartToolTip
    {
        /// <summary>
        /// Tooltip data that is displayed 
        /// </summary>
        [Parameter] public IEnumerable<BarChartToolTipInfo> ToolTips { get; set; }

        private String GetLeft()
        {
            Double left = 0.0;
            if (ToolTips != null && ToolTips.Any() == true)
            {
                BarChartToolTipInfo first = ToolTips.ElementAt(0);
                left = first.P2.X + ((first.P3.X - first.P2.X) * 0.9);
            }
            return left.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private String GetTop()
        {
            Double top = 0.0;
            if (ToolTips != null && ToolTips.Any() == true)
            {
                BarChartToolTipInfo first = ToolTips.ElementAt(0);
                top = first.P3.Y - ((first.P4.Y - first.P3.Y) * 0.1);
            }

            return top.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        private String GetStyle() => $"position: absolute; left: {GetLeft()}%; top: {GetTop()}%";
    }
}
