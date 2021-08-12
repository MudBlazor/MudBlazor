// Not Used

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.EnhanceChart.Internal;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public partial class MudEnhancedDonutChartDataPoint : MudEnhancedSegementChartDataPoint<MudEnhancedDonutChart, MudEnhancedDonutChartDataPoint, SvgDonutSegmentRepresentation>
    {
        protected override SegementChartToolTipInfo GenerateToolTip(SvgDonutSegmentRepresentation reprensentation) =>
         new DonutChartToolTipInfo(
                reprensentation.Point.Label, reprensentation.Point.Value, reprensentation.Fill,
                reprensentation.StartAngle, reprensentation.StartAngle - reprensentation.FillAngle, reprensentation.Radius);

    }
}
