using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays values as a percentage of a circle.
    /// </summary>
    /// <seealso cref="Bar"/>
    /// <seealso cref="Donut"/>
    /// <seealso cref="Pie"/>
    /// <seealso cref="Line"/>
    /// <seealso cref="StackedBar"/>
    /// <seealso cref="TimeSeries"/>
    public partial class Donut : MudRadialChartBase<DonutChartOptions>
    {
        public static new ChartType ChartType => ChartType.Donut;

        protected override void OnInitialized()
        {
            ChartOptions ??= new DonutChartOptions();
            base.OnInitialized();
        }

        public override void RebuildChart()
        {
            _paths.Clear();
            _legends.Clear();

            SetBounds();

            var chartData = AggregateSeriesData(ChartOptions!.AggregationOption);
            var normalizedData = GetNormalizedData();
            var cumulativeRadians = -Math.PI / 2; // Start at -90 degrees
            var donutRadiusRatio = ChartOptions!.DonutRingRatio.EnsureRange(0.1, 1);
            var chartLabels = ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet
                    ? ChartSeries.Select(ds => ds.Name).ToArray()
                    : ChartLabels ?? [];

            for (var i = 0; i < normalizedData.Length; i++)
            {
                if (normalizedData[i] == 0)
                    continue;

                var seriesdata = Math.Max(0, chartData[i]); //Ensure non-negative values
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

                // Calculate inner radius with a hole.
                var innerRadius = CalculatedRadius * (1 - donutRadiusRatio);

                // Outer coordinates
                var outerStartX = startx * CalculatedRadius;
                var outerStartY = starty * CalculatedRadius;
                var outerMidX = midx * CalculatedRadius;
                var outerMidY = midy * CalculatedRadius;
                var outerEndX = endx * CalculatedRadius;
                var outerEndY = endy * CalculatedRadius;

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
                    pathStringBuilder.Append($"A {ToS(CalculatedRadius)} {ToS(CalculatedRadius)} 0 {ToS(largeArcFlag)} 1 {ToS(outerMidX)} {ToS(outerMidY)} "); // Add an arc to a mid point half way through the slice (outer) to support 100% donuts
                }
                pathStringBuilder.Append($"A {ToS(CalculatedRadius)} {ToS(CalculatedRadius)} 0 {ToS(largeArcFlag)} 1 {ToS(outerEndX)} {ToS(outerEndY)} "); // Add an arc to the end point (outer)
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

                // Calculate the midpoint angle
                var midAngle = cumulativeRadians - Math.PI * data;
                var midRadius = CalculatedRadius * (1 - donutRadiusRatio / 2);

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
                path.LabelXValue = ChartOptions.ShowAsPercentage ? Math.Round(data * 100, 1).ToInvariantString() + "%" : seriesdata.ToInvariantString();
                path.LabelYValue = chartLabels.Length > i ? chartLabels[i] : string.Empty;

                _paths.Add(path);
            }

            for (var i = 0; i < chartLabels.Length; i++)
            {
                var labels = i < chartLabels.Length ? chartLabels[i] : "";

                if (labels.Length == 0)
                    continue;

                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = labels,
                    Visible = !HiddenIndices.Contains(i),
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                };
                _legends.Add(legend);
            }
        }
    }
}
