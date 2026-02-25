// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Numerics;
using System.Text;

namespace MudBlazor.Charts;

public partial class Rose<T> : MudRadialChartBase<T, RoseChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    protected override void OnInitialized()
    {
        ChartType = ChartType.Rose;
        ChartOptions ??= new RoseChartOptions();
        base.OnInitialized();
    }

    public override void RebuildChart()
    {
        _paths.Clear();
        _legends.Clear();

        SetBounds();

        var chartData = AggregateSeriesData(ChartOptions!.AggregationOption);
        var normalizedData = GetNormalizedData();
        var nonZeroCount = normalizedData.Count(d => d > 0.0);
        if (nonZeroCount == 0) return;

        var angleStep = 2 * Math.PI / nonZeroCount;
        var currentAngle = ChartOptions.AngleOffset * (Math.PI / 180);
        var chartLabels = GetChartLabels();
        var maxValue = normalizedData.Length > 0 ? normalizedData.Max() : 0.0;
        var sum = normalizedData.Sum();

        for (var i = 0; i < normalizedData.Length; i++)
        {
            if (normalizedData[i] == 0.0)
                continue;

            var value = T.Max(T.Zero, chartData[i]);
            var data = normalizedData[i];

            var radius = CalculateScaledRadius(data, maxValue);
            var coords = GetSegmentCoordinates(currentAngle, angleStep, nonZeroCount);
            var arc = BuildRadialPath(coords, radius, nonZeroCount);

            var midAngle = currentAngle + angleStep / 2;
            var (x, y) = GetRadialLabelPosition(midAngle, radius, nonZeroCount);

            _paths.Add(new SvgPetal
            {
                Index = i,
                Data = arc,
                SegmentRadius = radius,
                AngleRadians = angleStep,
                LabelX = x,
                LabelY = y,
                LabelXValue = ChartOptions.ShowAsPercentage
                    ? $"{ToS(Math.Round(data / sum * 100, 1))}%"
                    : value.ToString(null, CultureInfo.InvariantCulture),
                LabelYValue = chartLabels.Length > i ? chartLabels[i] : string.Empty
            });

            currentAngle += angleStep;
        }

        BuildLegends(chartLabels);
    }

    private double CalculateScaledRadius(double value, double max)
    {
        return Math.Max(0, Radius * (max == 0.0 ? 0 : value / max) * ChartOptions!.ScaleFactor);
    }

    private static SegmentCoordinates GetSegmentCoordinates(double angle, double step, int count) => new()
    {
        StartX = Math.Cos(angle),
        StartY = Math.Sin(angle),
        EndX = Math.Cos(angle + step),
        EndY = Math.Sin(angle + step),
        LargeArcFlag = count == 1 || step > Math.PI ? 1 : 0
    };

    private static string BuildRadialPath(SegmentCoordinates coords, double radius, int count)
    {
        var sb = new StringBuilder();

        sb.Append("M 0 0 ");
        sb.Append($"L {ToS(coords.StartX * radius)} {ToS(coords.StartY * radius)} ");

        if (count == 1)
        {
            sb.Append($"A {ToS(radius)} {ToS(radius)} 0 {coords.LargeArcFlag} 1 {ToS(coords.EndX * radius * -1)} {ToS(coords.EndY * radius)} ");
        }

        sb.Append($"A {ToS(radius)} {ToS(radius)} 0 {coords.LargeArcFlag} 1 {ToS(coords.EndX * radius)} {ToS(coords.EndY * radius)} ");
        sb.Append('Z');

        return sb.ToString();
    }

    private static (double X, double Y) GetRadialLabelPosition(double midAngle, double radius, int count)
    {
        if (count <= 1)
            return (0, 0);

        var r = radius * 0.85;

        return (Math.Cos(midAngle) * r, Math.Sin(midAngle) * r);
    }
}
