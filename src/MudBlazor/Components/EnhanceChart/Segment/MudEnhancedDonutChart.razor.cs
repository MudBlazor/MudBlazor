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
    record MudEnhancedDonutChartSnapShot(Double StartAngle, Double Padding, Double Thickness);

    public partial class MudEnhancedDonutChart : MudEnhancedSegmentChart<MudEnhancedDonutChart, MudEnhancedDonutChartDataPoint, SvgDonutSegmentRepresentation>, ISnapshot<MudEnhancedDonutChartSnapShot>
    {
        [Parameter] public Double Thickness { get; set; } = 20.0;

        #region Methods

        protected override String GetPathForNewElement(MudEnhancedDonutChartDataPoint item, Double radius, Double startAngle, Double minAngle) =>
            new SvgDonutSegmentRepresentation
            {
                Point = item,
                Radius = radius,
                InnerRadius = radius - Thickness,
                Fill = item.FillColor,
                FillAngle = minAngle,
                StartAngle = startAngle,
            }.GetPathValue();

        protected override SvgDonutSegmentRepresentation GetSegementRepresentation(MudEnhancedDonutChartDataPoint point, Double radius, Double segementLength, Double startAngle) => new()
        {
            Point = point,
            Radius = radius,
            Fill = point.FillColor,
            FillAngle = segementLength,
            InnerRadius = radius - Thickness,
            StartAngle = startAngle,
        };

        protected override void OnParametersSet()
        {
            ISnapshot<MudEnhancedDonutChartSnapShot> _this = this;

            if (_this.SnapshotHasChanged(true) == true)
            {
                TriggerAnimation = true;
                CreateDrawingInstruction();
            }
        }

        MudEnhancedDonutChartSnapShot ISnapshot<MudEnhancedDonutChartSnapShot>.OldSnapshotValue { get; set; }
        MudEnhancedDonutChartSnapShot ISnapshot<MudEnhancedDonutChartSnapShot>.CreateSnapShot() => new (StartAngle, Padding, Thickness);

        #endregion
    }
}
