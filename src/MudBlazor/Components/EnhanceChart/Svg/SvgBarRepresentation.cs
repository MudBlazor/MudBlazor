using System;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart.Internal
{
    public class SvgBarRepresentation
    {
        public MudEnhancedBarChartSeries Series { get; set; }
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }
        public Point2D P3 { get; set; }
        public Point2D P4 { get; set; }
        public MudColor Fill { get; set; }
        public String XLabel { get; set; }
        public Double YValue { get; set; }
        public String OldPath { get;  set; }
        public String Id { get;  set; }

        public String GetPathValue() => $"{FormattableString.Invariant($"{P1.X},{P1.Y} {P2.X},{P2.Y} {P3.X},{P3.Y} {P4.X},{P4.Y}")}";
        public String GetActiveClass() => Series?.IsActive == true ? "active" : "inactive";

        public void Actived()
        {
            Series.SentRequestToBecomeActiveAlone();
            Series.SentRequestForTooltip(this);
        }

        public void Deactived()
        {
            Series.RevokeExclusiveActiveState();
            Series.SentRevokationForTooltip(this);
        }
    }
}
