using System;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart.Internal
{
    public class SvgCircleSegmentRepresentation : SvgSegementRepresentation
    {
        public MudEnhancedPieChartDataPoint Point { get; set; }
        public Double Radius { get; set; }
        public Double StartAngle { get; set; }
        public Double FillAngle { get; set; }
        public MudColor Fill { get; set; }


        public static Double CenterY(Double y) => -y + 50;
        public static Double MoveX(Double x) => x + 50;

        public override String GetPathValue() => $"{FormattableString.Invariant($"M 50 50 L { MoveX(Radius * Math.Cos(StartAngle.ToRad()))} { CenterY(Radius * Math.Sin(StartAngle.ToRad()))} A {Radius} {Radius},0,{(FillAngle > 180.0 ? 1 : 0)},1, { MoveX(Radius * Math.Cos((StartAngle - FillAngle).ToRad()))} {CenterY(Radius * Math.Sin((StartAngle - FillAngle).ToRad()))} L 50 50")}";

        public String GetCssClass() => new CssBuilder("mud-enhanced-chart-series pie")
            .AddClass("active", Point?.IsActive).AddClass("inactive", !Point?.IsActive)
            .AddClass(Point.AddtionalClass).ToString();

        public void Actived()
        {
            Point.SentRequestToBecomeActiveAlone();
            Point.SentRequestForTooltip(this);
        }

        public void Deactived()
        {
            Point.RevokeExclusiveActiveState();
            Point.SentRevokationForTooltip(this);
        }
    }
}
