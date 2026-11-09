using System.Globalization;
using System.Numerics;
using System.Text;
using MudBlazor.Extensions;

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
        protected override void OnInitialized()
        {
            ChartType = ChartType.Pie;
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
            var cumulativeRadians = -Math.PI / 2;
            var chartLabels = GetChartLabels();

            for (var i = 0; i < normalizedData.Length; i++)
            {
                if (normalizedData[i] == 0.0)
                    continue;

                var data = normalizedData[i];
                var value = T.Max(T.Zero, chartData[i]);
                var radians = 2 * Math.PI * data;
                var half = radians / 2;

                var coords = GetSegmentCoordinates(cumulativeRadians, half, radians);
                cumulativeRadians += radians;

                var pathData = BuildSvgPath(coords, Radius, data);

                var midAngle = cumulativeRadians - (Math.PI * data);
                var (x, y) = GetLabelPosition(midAngle, Radius, data);

                _paths.Add(new SvgPetal
                {
                    Index = i,
                    Data = pathData,
                    LabelX = x,
                    LabelY = y,
                    LabelXValue = ChartOptions.ShowAsPercentage
                        ? $"{Math.Round(data * 100, 1).ToInvariantString()}%"
                        : value.ToString(null, CultureInfo.InvariantCulture),
                    LabelYValue = chartLabels.Length > i ? chartLabels[i] : string.Empty,
                    SegmentRadius = Radius,
                    AngleRadians = radians,
                    LabelOffset = 0.5,
                });
            }

            BuildLegends(chartLabels);
        }

        private static SegmentCoordinates GetSegmentCoordinates(double startAngle, double halfAngle, double fullAngle)
        {
            return new SegmentCoordinates
            {
                StartX = Math.Cos(startAngle),
                StartY = Math.Sin(startAngle),
                MidX = Math.Cos(startAngle + halfAngle),
                MidY = Math.Sin(startAngle + halfAngle),
                EndX = Math.Cos(startAngle + fullAngle),
                EndY = Math.Sin(startAngle + fullAngle),
                LargeArcFlag = fullAngle > Math.PI ? 1 : 0
            };
        }

        private static string BuildSvgPath(SegmentCoordinates c, double radius, double data)
        {
            var sb = new StringBuilder();

            sb.Append($"M {ToS(c.StartX * radius)} {ToS(c.StartY * radius)} ");
            if (data >= 1.0)
                sb.Append($"A {ToS(radius)} {ToS(radius)} 0 {c.LargeArcFlag} 1 {ToS(c.MidX * radius)} {ToS(c.MidY * radius)} ");
            sb.Append($"A {ToS(radius)} {ToS(radius)} 0 {c.LargeArcFlag} 1 {ToS(c.EndX * radius)} {ToS(c.EndY * radius)} ");
            sb.Append("L 0 0 Z");

            return sb.ToString();
        }

        private static (double X, double Y) GetLabelPosition(double angle, double radius, double data)
        {
            if (data >= 1.0)
                return (0, 0);

            var r = radius * 0.5;
            return (Math.Cos(angle) * r, Math.Sin(angle) * r);
        }
    }
}
