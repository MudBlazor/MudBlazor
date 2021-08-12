using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;

namespace MudBlazor.EnhanceChart
{
    public partial class MudEnhancedDonutChartToolTip
    {
        /// <summary>
        /// Tooltip data that is displayed 
        /// </summary>
        [Parameter] public IEnumerable<DonutChartToolTipInfo> ToolTips { get; set; }

        private String GetStyle()
        {
            if (ToolTips == null || ToolTips.Any() == false)
            {
                return String.Empty;
            }

            var first = ToolTips.First();

            var centerAngleInRad = ((first.StartAngle + first.EndAngle) / 2.0).ToRad();
            var middleRadius = first.Radius / 2.0;

            Double left = 50 + Math.Cos(centerAngleInRad) * middleRadius;
            Double top = 50 - (Math.Sin(centerAngleInRad) * middleRadius);

            return $"position: absolute; left: {left.ToString(System.Globalization.CultureInfo.InvariantCulture)}%; top: {top.ToString(System.Globalization.CultureInfo.InvariantCulture)}%";
        }

    }
}
