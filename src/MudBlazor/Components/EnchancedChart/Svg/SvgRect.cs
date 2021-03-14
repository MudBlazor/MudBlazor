// Not Used

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor.Components.EnchancedChart.Svg
{
    public class SvgPolygonBasedRectangle
    {
        public IDataSeries Series { get; set; }
        public Point2D P1 { get; set; }
        public Point2D P2 { get; set; }
        public Point2D P3 { get; set; }
        public Point2D P4 { get; set; }
        public CssColor Fill { get; set; }

        public String GetPathValue() => $"{FormattableString.Invariant($"{P1.X},{P1.Y} {P2.X},{P2.Y} {P3.X},{P3.Y} {P4.X},{P4.Y}")}";
        public String GetActiveClass() => Series?.IsActive == true ? "active" : "inactive";

        public void Actived() => Series.SentRequestToBecomeActiveAlone();
        public void Deactived() => Series.RevokeExclusiveActiveState();
    }
}
