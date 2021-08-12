// Not Used

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.EnhanceChart.Internal;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public partial class MudEnhancedPieChartDataPoint : MudEnhancedSegementChartDataPoint<MudEnhancedPieChart, MudEnhancedPieChartDataPoint,SvgCircleSegmentRepresentation>
    {
        protected override SegementChartToolTipInfo GenerateToolTip(SvgCircleSegmentRepresentation reprensentation) =>
         new PieChartToolTipInfo(
                reprensentation.Point.Label, reprensentation.Point.Value, reprensentation.Fill,
                reprensentation.StartAngle, reprensentation.StartAngle - reprensentation.FillAngle, reprensentation.Radius);

       
    }
}
