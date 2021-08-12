using System;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart.Internal
{
    public class SvgDonutSegmentRepresentation : SvgSegementRepresentation
    {
        public MudEnhancedDonutChartDataPoint Point { get; set; }
        public Double Radius { get; set; }
        public Double StartAngle { get; set; }
        public Double FillAngle { get; set; }
        public MudColor Fill { get; set; }
        public Double InnerRadius { get; set; }

        public static Double CenterY(Double y) => -y + 50;
        public static Double MoveX(Double x) => x + 50;

        public override String GetPathValue()
        {
            Double startAngleInRad = StartAngle.ToRad();
            Double endAngleInRad = (StartAngle - FillAngle).ToRad();

            Point2D a = new Point2D(Math.Cos(startAngleInRad) * InnerRadius, Math.Sin(startAngleInRad) * InnerRadius);
            Point2D b = new Point2D(Math.Cos(startAngleInRad) * Radius, Math.Sin(startAngleInRad) * Radius);
            Point2D c = new Point2D(Math.Cos(endAngleInRad) * Radius, Math.Sin(endAngleInRad) * Radius);
            Point2D d = new Point2D(Math.Cos(endAngleInRad) * InnerRadius, Math.Sin(endAngleInRad) * InnerRadius);

            return $"{FormattableString.Invariant($"M {MoveX(a.X)} {CenterY(a.Y)} L { MoveX(b.X)} { CenterY(b.Y)} A {Radius} {Radius},0,{(FillAngle > 180.0 ? 1 : 0)},1, { MoveX(c.X)} {CenterY(c.Y)} L {MoveX(d.X)} {CenterY(d.Y)}  A {InnerRadius} {InnerRadius},0,{(FillAngle > 180.0 ? 1 : 0)},0, { MoveX(a.X)} {CenterY(a.Y)}")}";
        }

        //=> 

        public String GetCssClass() => new CssBuilder("mud-enhanced-chart-series donut")
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
