using System.Data;
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
    /// <seealso cref="Line"/>
    /// <seealso cref="StackedBar"/>
    /// <seealso cref="TimeSeries"/>
    partial class Pie : MudRadialChartBase<PieChartOptions>
    {
        public override void RebuildChart()
        {
            _paths.Clear();
            _legends.Clear();

            SetBounds();

            var chartData = AggregateSeriesData(ChartOptions!.AggregationOption);
            var normalizedData = GetNormalizedData();
            var cumulativeRadians = -Math.PI / 2; // Start at -90 degrees
            var chartLabels = ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet
                    ? ChartSeries.Select(ds => ds.Label).ToArray()
                    : ChartLabels;

            for (var i = 0; i < normalizedData.Length; i++)
            {
                if (normalizedData[i] == 0)
                    continue;

                var seriesdata = chartData[i];
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

                pathStringBuilder.Append($"M {ToS(startx * CalculatedRadius)} {ToS(starty * CalculatedRadius)} "); // Move to the start point
                if (data >= 1)
                {
                    pathStringBuilder.Append($"A {ToS(CalculatedRadius)} {ToS(CalculatedRadius)} 0 {ToS(largeArcFlag)} 1 {ToS(midx * CalculatedRadius)} {ToS(midy * CalculatedRadius)} "); // Add an arc to a mid point half way through the slice to support 100% pies
                }
                pathStringBuilder.Append($"A {ToS(CalculatedRadius)} {ToS(CalculatedRadius)} 0 {ToS(largeArcFlag)} 1 {ToS(endx * CalculatedRadius)} {ToS(endy * CalculatedRadius)} "); // Add an arc to the end point
                pathStringBuilder.Append("L 0 0 Z"); // Line to the center

                // Standard pie slice path going to the center.
                path = new SvgPath()
                {
                    Index = i,
                    Data = pathStringBuilder.ToString()
                };

                // Calculate the midpoint angle
                var midAngle = cumulativeRadians - Math.PI * data;
                var midRadius = CalculatedRadius * 0.5d;

                var midX = 0d;
                var midY = 0d;

                if (data < 1) // don't find mid point when data is 100%, just use the 0,0 point.
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
                    Visible = !_hiddenIndicies.Contains(i),
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                };
                _legends.Add(legend);
            }
        }
    }
}
