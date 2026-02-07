using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;

namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays values as a percentage of a circle.
    /// </summary>
    /// <seealso cref="Bar{T}"/>
    /// <seealso cref="Donut{T}"/>
    /// <seealso cref="Pie{T}"/>
    /// <seealso cref="Line{T}"/>
    /// <seealso cref="StackedBar{T}"/>
    /// <seealso cref="TimeSeries{T}"/>
    public partial class Donut<T> : MudRadialChartBase<T, DonutChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        protected override void OnInitialized()
        {
            ChartType = ChartType.Donut;
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
            var cumulativeRadians = -Math.PI / 2;
            var donutRatio = ChartOptions.DonutRingRatio.EnsureRange(0.1, 1);
            var chartLabels = GetChartLabels();

            for (var i = 0; i < normalizedData.Length; i++)
            {
                if (normalizedData[i] == T.Zero)
                    continue;

                var data = normalizedData[i];
                var actualValue = T.Max(T.Zero, chartData[i]);
                var radians = 2 * Math.PI * double.CreateSaturating(data);
                var coords = Donut<T>.GetSegmentCoordinates(cumulativeRadians, radians);
                cumulativeRadians += radians;

                var geometry = new PathGeometry
                {
                    Coords = coords,
                    OuterRadius = Radius,
                    InnerRadius = Radius * (1 - donutRatio),
                    Data = data
                };

                var pathData = Donut<T>.BuildSvgPath(geometry);
                var midAngle = cumulativeRadians - radians / 2;
                var (x, y) = Donut<T>.GetLabelPosition(midAngle, Radius, donutRatio, data);

                _paths.Add(new SvgPath
                {
                    Index = i,
                    Data = pathData,
                    LabelX = x,
                    LabelY = y,
                    LabelXValue = ChartOptions.ShowAsPercentage
                        ? $"{Math.Round(double.CreateSaturating(data) * 100, 1).ToInvariantString()}%"
                        : actualValue.ToString(null, CultureInfo.InvariantCulture),
                    LabelYValue = chartLabels.Length > i ? chartLabels[i] : string.Empty
                });
            }

            BuildLegends(chartLabels);
        }

        private static SegmentCoordinates GetSegmentCoordinates(double startRadians, double segmentRadians)
        {
            var half = segmentRadians / 2;
            return new SegmentCoordinates
            {
                StartX = Math.Cos(startRadians),
                StartY = Math.Sin(startRadians),
                MidX = Math.Cos(startRadians + half),
                MidY = Math.Sin(startRadians + half),
                EndX = Math.Cos(startRadians + segmentRadians),
                EndY = Math.Sin(startRadians + segmentRadians)
            };
        }

        private static string BuildSvgPath(PathGeometry g)
        {
            var sb = new StringBuilder();
            var arcFlag = double.CreateSaturating(g.Data) > 0.5 ? 1 : 0;

            static double ToR(double value, double radius) => value * radius;

            sb.Append($"M {ToS(ToR(g.Coords.StartX, g.OuterRadius))} {ToS(ToR(g.Coords.StartY, g.OuterRadius))} ");

            if (g.Data >= T.One)
                sb.Append($"A {ToS(g.OuterRadius)} {ToS(g.OuterRadius)} 0 {arcFlag} 1 {ToS(ToR(g.Coords.MidX, g.OuterRadius))} {ToS(ToR(g.Coords.MidY, g.OuterRadius))} ");

            sb.Append($"A {ToS(g.OuterRadius)} {ToS(g.OuterRadius)} 0 {arcFlag} 1 {ToS(ToR(g.Coords.EndX, g.OuterRadius))} {ToS(ToR(g.Coords.EndY, g.OuterRadius))} ");
            sb.Append($"L {ToS(ToR(g.Coords.EndX, g.InnerRadius))} {ToS(ToR(g.Coords.EndY, g.InnerRadius))} ");

            if (g.Data >= T.One)
                sb.Append($"A {ToS(g.InnerRadius)} {ToS(g.InnerRadius)} 0 {arcFlag} 0 {ToS(ToR(g.Coords.MidX, g.InnerRadius))} {ToS(ToR(g.Coords.MidY, g.InnerRadius))} ");

            sb.Append($"A {ToS(g.InnerRadius)} {ToS(g.InnerRadius)} 0 {arcFlag} 0 {ToS(ToR(g.Coords.StartX, g.InnerRadius))} {ToS(ToR(g.Coords.StartY, g.InnerRadius))} Z");

            return sb.ToString();
        }

        private static (double X, double Y) GetLabelPosition(double angle, double outerRadius, double donutRatio, T data)
        {
            if (donutRatio >= 1 && data >= T.One)
                return (0, 0);

            var radius = outerRadius * (1 - donutRatio / 2);
            return (Math.Cos(angle) * radius, Math.Sin(angle) * radius);
        }

        private readonly struct PathGeometry
        {
            public SegmentCoordinates Coords { get; init; }
            public double OuterRadius { get; init; }
            public double InnerRadius { get; init; }
            public T Data { get; init; }
        }

    }
}
