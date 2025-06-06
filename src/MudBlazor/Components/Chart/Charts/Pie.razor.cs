using System.Data;
using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays values as a percentage of a circle.
    /// </summary>
    /// <seealso cref="Bar{T}"/>
    /// <seealso cref="Donut{T}"/>
    /// <seealso cref="Line{T}"/>
    /// <seealso cref="StackedBar{T}"/>
    /// <seealso cref="TimeSeries{T}"/>
    partial class Pie<T> : MudRadialChartBase<T, PieChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        public static new ChartType ChartType => ChartType.Pie;

        protected override void OnInitialized()
        {
            ChartOptions ??= new PieChartOptions();
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
            var chartLabels = ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet
                    ? ChartSeries.Select(ds => ds.Name).ToArray()
                    : ChartLabels ?? [];

            for (var i = 0; i < normalizedData.Length; i++)
            {
                if (normalizedData[i] == T.Zero)
                    continue;

                var seriesdata = T.Max(T.Zero, chartData[i]); //Ensure non-negative values
                var data = normalizedData[i];

                var startx = Math.Cos(cumulativeRadians);
                var starty = Math.Sin(cumulativeRadians);

                var sliceAngleRadians = 2 * Math.PI * double.CreateSaturating(data);
                var halfAngle = sliceAngleRadians / 2;

                cumulativeRadians += halfAngle;

                var midx = Math.Cos(cumulativeRadians);
                var midy = Math.Sin(cumulativeRadians);

                cumulativeRadians += halfAngle;

                var endx = Math.Cos(cumulativeRadians);
                var endy = Math.Sin(cumulativeRadians);
                var largeArcFlag = double.CreateSaturating(data) > 0.5 ? 1 : 0;

                var pathStringBuilder = new StringBuilder();

                pathStringBuilder.Append($"M {ToS(startx * Radius)} {ToS(starty * Radius)} "); // Move to the start point
                if (data >= T.One)
                {
                    pathStringBuilder.Append($"A {ToS(Radius)} {ToS(Radius)} 0 {ToS(largeArcFlag)} 1 {ToS(midx * Radius)} {ToS(midy * Radius)} "); // Add an arc to a mid point half way through the slice to support 100% pies
                }
                pathStringBuilder.Append($"A {ToS(Radius)} {ToS(Radius)} 0 {ToS(largeArcFlag)} 1 {ToS(endx * Radius)} {ToS(endy * Radius)} "); // Add an arc to the end point
                pathStringBuilder.Append("L 0 0 Z"); // Line to the center

                // Calculate the midpoint angle
                var midAngle = cumulativeRadians - Math.PI * double.CreateSaturating(data);
                var midRadius = Radius * 0.5d;

                var midX = 0d;
                var midY = 0d;

                if (data < T.One) // don't find mid point when data is 100%, just use the 0,0 point.
                {
                    // Calculate the midpoint coordinates at half the radius
                    midX = Math.Cos(midAngle) * midRadius;
                    midY = Math.Sin(midAngle) * midRadius;
                }

                // Standard pie slice path going to the center.
                var path = new SvgPetal
                {
                    Index = i,
                    Data = pathStringBuilder.ToString(),
                    LabelX = midX,
                    LabelY = midY,
                    LabelXValue = ChartOptions.ShowAsPercentage
                        ? Math.Round(double.CreateSaturating(data) * 100, 1).ToInvariantString() + "%"
                        : seriesdata.ToString(null, CultureInfo.InvariantCulture),
                    LabelYValue = chartLabels.Length > i ? chartLabels[i] : string.Empty,
                    SegmentRadius = Radius,
                    AngleRadians = sliceAngleRadians,
                    LabelOffset = 0.5,
                };

                _paths.Add(path);
            }

            for (var i = 0; i < chartLabels.Length; i++)
            {
                var labels = chartLabels[i];

                if (labels.Length == 0)
                    continue;

                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = labels,
                    Visible = ChartOptions.AggregationOption == AggregationOption.GroupByLabel ? !HiddenIndices.Contains(i) : ChartSeries[i].Visible,
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                };
                _legends.Add(legend);
            }
        }
    }
}
