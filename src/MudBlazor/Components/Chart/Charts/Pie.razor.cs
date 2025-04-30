using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays values as a percentage of a circle.
    /// </summary>
    /// <seealso cref="Bar"/>
    /// <seealso cref="Donut"/>
    /// <seealso cref="Line"/>
    /// <seealso cref="StackedBar"/>
    /// <seealso cref="TimeSeries"/>
    partial class Pie : MudCategoryChartBase
    {
        private const int Radius = 140;

        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart? MudChartParent { get; set; }

        /// <summary>
        /// Defines the ratio of the circle to the donut hole.
        /// </summary>
        /// <remarks>
        /// 1.0 = full circle, 0.25 = donut with 25% radius thickness 75% hole.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Appearance)]
        public double CircleDonutRatio { get; set; } = 1;

        private readonly List<SvgPath> _paths = [];
        private readonly List<SvgLegend> _legends = [];
        private SvgPath? _hoveredSegment;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _paths.Clear();
            _legends.Clear();

            if (InputData == null)
                return;

            var normalizedData = GetNormalizedData();
            double cumulativeRadians = -Math.PI / 2; // Start at -90 degrees

            double donutRadiusRatio = CircleDonutRatio.EnsureRange(0.1, 1);

            for (var i = 0; i < normalizedData.Length; i++)
            {
                var originalData = InputData[i];
                var data = normalizedData[i];
                var startx = Math.Cos(cumulativeRadians);
                var starty = Math.Sin(cumulativeRadians);
                cumulativeRadians += 2 * Math.PI * data / 2;
                var midx = Math.Cos(cumulativeRadians);
                var midy = Math.Sin(cumulativeRadians);
                cumulativeRadians += 2 * Math.PI * data / 2;
                var endx = Math.Cos(cumulativeRadians);
                var endy = Math.Sin(cumulativeRadians);
                var largeArcFlag = data > 0.5 ? 1 : 0;

                SvgPath path;
                var pathStringBuilder = new StringBuilder();
                if (donutRadiusRatio < 1)
                {
                    // Calculate inner radius with a hole.
                    var innerRadius = Radius * (1 - donutRadiusRatio);

                    // Outer coordinates
                    var outerStartX = startx * Radius;
                    var outerStartY = starty * Radius;
                    var outerMidX = midx * Radius;
                    var outerMidY = midy * Radius;
                    var outerEndX = endx * Radius;
                    var outerEndY = endy * Radius;

                    // Inner coordinates (for the hole)
                    var innerStartX = startx * innerRadius;
                    var innerStartY = starty * innerRadius;
                    var innerMidX = midx * innerRadius;
                    var innerMidY = midy * innerRadius;
                    var innerEndX = endx * innerRadius;
                    var innerEndY = endy * innerRadius;


                    pathStringBuilder.Append($"M {ToS(outerStartX)} {ToS(outerStartY)} "); // Move to the start point
                    if (data >= 1)
                    {
                        pathStringBuilder.Append($"A {ToS(Radius)} {ToS(Radius)} 0 {ToS(largeArcFlag)} 1 {ToS(outerMidX)} {ToS(outerMidY)} "); // Add an arc to a mid point half way through the slice (outer) to support 100% donuts
                    }
                    pathStringBuilder.Append($"A {ToS(Radius)} {ToS(Radius)} 0 {ToS(largeArcFlag)} 1 {ToS(outerEndX)} {ToS(outerEndY)} "); // Add an arc to the end point (outer)
                    pathStringBuilder.Append($"L {ToS(innerEndX)} {ToS(innerEndY)} "); // Line to the end point of the inner arc
                    if (data >= 1)
                    {
                        pathStringBuilder.Append($"A {ToS(innerRadius)} {ToS(innerRadius)} 0 {ToS(largeArcFlag)} 0 {ToS(innerMidX)} {ToS(innerMidY)} ");  // Add an arc to a mid point half way through the slice to support 100% donuts
                    }
                    pathStringBuilder.Append($"A {ToS(innerRadius)} {ToS(innerRadius)} 0 {ToS(largeArcFlag)} 0 {ToS(innerStartX)} {ToS(innerStartY)} Z"); // Add an arc to the start point (inner)

                    // Build a compound path: outer arc -> line to inner arc -> inner arc -> close
                    path = new SvgPath
                    {
                        Index = i,
                        Data = pathStringBuilder.ToString()
                    };
                }
                else
                {
                    pathStringBuilder.Append($"M {ToS(startx * Radius)} {ToS(starty * Radius)} "); // Move to the start point
                    if (data >= 1)
                    {
                        pathStringBuilder.Append($"A {ToS(Radius)} {ToS(Radius)} 0 {ToS(largeArcFlag)} 1 {ToS(midx * Radius)} {ToS(midy * Radius)} "); // Add an arc to a mid point half way through the slice to support 100% pies
                    }
                    pathStringBuilder.Append($"A {ToS(Radius)} {ToS(Radius)} 0 {ToS(largeArcFlag)} 1 {ToS(endx * Radius)} {ToS(endy * Radius)} "); // Add an arc to the end point
                    pathStringBuilder.Append("L 0 0 Z"); // Line to the center

                    // Standard pie slice path going to the center.
                    path = new SvgPath()
                    {
                        Index = i,
                        Data = pathStringBuilder.ToString()
                    };
                }

                // Calculate the midpoint angle
                var midAngle = cumulativeRadians - Math.PI * data;
                var midRadius = Radius * (1 - donutRadiusRatio / 2);

                var midX = 0d;
                var midY = 0d;

                if (donutRadiusRatio < 1 || data < 1) // don't find mid point when donut is 100% and data is 100%, just use the 0,0 point.
                {
                    // Calculate the midpoint coordinates at half the radius
                    midX = Math.Cos(midAngle) * midRadius;
                    midY = Math.Sin(midAngle) * midRadius;
                }

                path.LabelX = midX;
                path.LabelY = midY;
                path.LabelXValue = originalData.ToString(CultureInfo.InvariantCulture);
                path.LabelYValue = InputLabels.Length > i ? InputLabels[i] : string.Empty;

                _paths.Add(path);
            }

            for (var i = 0; i < normalizedData.Length; i++)
            {
                var percent = normalizedData[i] * 100;
                var labels = i < InputLabels.Length ? InputLabels[i] : "";

                if (labels.Length == 0)
                    continue;

                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = labels,
                    Data = ToS(Math.Round(percent, 1))
                };
                _legends.Add(legend);
            }
        }

        private void OnSegmentMouseOver(MouseEventArgs _, SvgPath segment)
        {
            _hoveredSegment = segment;
        }

        private void OnSegmentMouseOut(MouseEventArgs _)
        {
            _hoveredSegment = null;
        }
    }
}
