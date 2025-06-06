// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;

#nullable enable
namespace MudBlazor.Charts;

public partial class Rose<T> : MudRadialChartBase<T, RoseChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    public static new ChartType ChartType => ChartType.Rose;

    protected override void OnInitialized()
    {
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
        var nonZeroCount = normalizedData.Count(d => d > T.Zero);

        if (nonZeroCount == 0)
            return;

        var angleStep = 2 * Math.PI / nonZeroCount;
        var currentAngle = ChartOptions.AngleOffset * (Math.PI / 180);

        var chartLabels = ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet
                ? ChartSeries.Select(ds => ds.Name).ToArray()
                : ChartLabels ?? [];

        var maxValue = normalizedData.Length > 0 ? normalizedData.Max() : T.Zero;

        for (var i = 0; i < normalizedData.Length; i++)
        {
            if (normalizedData[i] == T.Zero)
                continue;

            var seriesdata = T.Max(T.Zero, chartData[i]); //Ensure non-negative values
            var dataValue = normalizedData[i];

            // Scale radius based on data value
            var sectorRadius = Radius * double.CreateSaturating((dataValue / maxValue)) * ChartOptions.ScaleFactor;

            sectorRadius = Math.Max(0, sectorRadius);

            var startx = Math.Cos(currentAngle);
            var starty = Math.Sin(currentAngle);
            var endx = Math.Cos(currentAngle + angleStep);
            var endy = Math.Sin(currentAngle + angleStep);
            var largeArcFlag = nonZeroCount == 1 ? 1 : (angleStep > Math.PI ? 1 : 0);

            var pathStringBuilder = new StringBuilder();
            pathStringBuilder.Append($"M 0 0 "); // Move to center
            pathStringBuilder.Append($"L {ToS(startx * sectorRadius)} {ToS(starty * sectorRadius)} ");

            if (nonZeroCount == 1)
            {
                pathStringBuilder.Append($"A {ToS(sectorRadius)} {ToS(sectorRadius)} 0 {largeArcFlag} 1 {ToS(endx * sectorRadius * -1)} {ToS(endy * sectorRadius)} ");
            }

            pathStringBuilder.Append($"A {ToS(sectorRadius)} {ToS(sectorRadius)} 0 {largeArcFlag} 1 {ToS(endx * sectorRadius)} {ToS(endy * sectorRadius)} ");
            pathStringBuilder.Append('Z');

            var midAngle = currentAngle + angleStep / 2;
            var labelRadius = sectorRadius * 0.85; // Position label inside the sector a bit
            var midX = 0d;
            var midY = 0d;

            if (nonZeroCount > 1)
            {
                midX = Math.Cos(midAngle) * labelRadius;
                midY = Math.Sin(midAngle) * labelRadius;
            }

            var path = new SvgPetal
            {
                Index = i,
                Data = pathStringBuilder.ToString(),
                SegmentRadius = sectorRadius,
                AngleRadians = angleStep,
                LabelX = midX,
                LabelY = midY,
                LabelXValue = ChartOptions.ShowAsPercentage
                                ? ToS(Math.Round(double.CreateSaturating(dataValue / normalizedData.SumGeneric()) * 100, 1)) + "%"
                                : seriesdata.ToString(null, CultureInfo.InvariantCulture),
                LabelYValue = chartLabels.Length > i ? chartLabels[i] : string.Empty
            };

            _paths.Add(path);
            currentAngle += angleStep;
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
