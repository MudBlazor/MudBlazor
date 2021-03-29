using System;
using MudBlazor.Utilities;

namespace MudBlazor.EnhanceChart.Internal
{
    public record SvgLine(Point2D P1, Point2D P2, Double thickness, String color, String cssClass);
}
