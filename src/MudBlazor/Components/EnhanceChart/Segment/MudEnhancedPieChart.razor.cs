using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.EnhanceChart.Internal;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart
{
    public partial class MudEnhancedPieChart : MudEnhancedSegmentChart<MudEnhancedPieChart,MudEnhancedPieChartDataPoint,SvgCircleSegmentRepresentation>
    {
        #region Methods

        protected override String GetPathForNewElement(MudEnhancedPieChartDataPoint item, Double radius, Double startAngle, Double minAngle) =>
            new SvgCircleSegmentRepresentation
            {
                Point = item,
                Radius = radius,
                Fill = item.FillColor,
                FillAngle = minAngle,
                StartAngle = startAngle,
            }.GetPathValue();

        protected override SvgCircleSegmentRepresentation GetSegementRepresentation(MudEnhancedPieChartDataPoint point, Double radius, Double segementLength, Double startAngle) => new()
        {
            Point = point,
            Radius = radius,
            Fill = point.FillColor,
            FillAngle = segementLength,
            StartAngle = startAngle,
        };

        #endregion
    }
}
