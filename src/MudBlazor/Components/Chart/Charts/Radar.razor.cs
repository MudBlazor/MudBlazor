// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;

namespace MudBlazor.Charts;

public partial class Radar<T> : MudRadialChartBase<T, RadarChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    protected List<SvgPath> _gridLines = [];
    protected List<SvgPath> _axisLines = [];
    protected List<SvgPath> _axisValues = [];

    public int? SelectedPointIndex { get; set; }
    public int? HoveredPathIndex { get; set; }

    protected override void OnInitialized()
    {
        ChartType = ChartType.Radar;
        ChartOptions ??= new RadarChartOptions();
        base.OnInitialized();
    }

    public override void RebuildChart()
    {
        _paths.Clear();
        _legends.Clear();
        _gridLines.Clear();
        _axisLines.Clear();
        _axisValues.Clear();

        SetBounds();

        if (!HasValidData())
            return;

        var normalizedData = GetNormalizedData();
        var (seriesData, labelData) = GroupDataSet(ChartLabels ?? [], ChartSeries, ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet);
        var numAxes = labelData.Length;

        BuildLegends([.. seriesData.Select(x => x.Name)]);

        var angleStep = 2 * Math.PI / numAxes;
        var currentAngle = -Math.PI / 2 + (ChartOptions.AngleOffset * (Math.PI / 180)); // Convert offset to radians
        var radius = CalculateRadius();

        var axisMaxValue = CalculateAxisMaxValue(
            seriesData
                .Where((x, i) => x.Visible && !HiddenIndices.Contains(i))
                .SelectMany(s => s.Data ?? T.Zero)
                .DefaultIfEmpty(T.Zero)
                .Max()
        );

        if (ChartOptions.ShowGridLines)
            GenerateGridLines(numAxes, angleStep, currentAngle, radius);

        if (ChartOptions.ShowAxisValues)
            GenerateAxisValues(currentAngle, axisMaxValue, radius);

        GenerateAxisLines(numAxes, angleStep, currentAngle, radius, labelData);
        GenerateSvgPaths(seriesData, normalizedData, numAxes, angleStep, currentAngle, radius, axisMaxValue);
    }

    private bool HasValidData() =>
        ChartSeries != null &&
        ChartSeries.Count > 0 &&
        ChartSeries.Any(s => s.Data != null && s.Data.Count > 0);

    private double CalculateRadius()
    {
        if (!ChartOptions.ShowAxisLabels)
            return Radius;

        var padding = MatchBoundsToSize ? 60 : 40;
        var maxR = Math.Min((_boundWidth - padding) / 2.0, (_boundHeight - padding) / 2.0);

        return Math.Min(Radius, maxR);
    }

    private void GenerateSvgPaths(List<ChartSeries<T>> seriesData, T[] normalizedData, int numAxes,
                                  double angleStep, double currentAngle, double radius, T axisMaxValue)
    {
        for (var seriesIndex = 0; seriesIndex < seriesData.Count; seriesIndex++)
        {
            var series = seriesData[seriesIndex];

            if (series.Data == null || series.Data.Count == 0 || !series.Visible || HiddenIndices.Contains(seriesIndex))
                continue;

            var (pathString, points) = GeneratePolygonPath(series, seriesIndex, numAxes, angleStep, currentAngle, radius, axisMaxValue);

            var path = new SvgPolygon
            {
                Index = seriesIndex,
                Data = pathString,
                Points = points,
                LabelXValue = ChartOptions.ShowAsPercentage
                    ? ToS(Math.Round(double.CreateSaturating(normalizedData[seriesIndex]) * 100, 1)) + "%"
                    : series.Data.Values.SumGeneric().ToString(null, CultureInfo.InvariantCulture),
                LabelYValue = series.Name
            };

            _paths.Add(path);
        }
    }

    private static (string Path, List<SvgPathPoint> Points) GeneratePolygonPath(ChartSeries<T> series, int seriesIndex, int numAxes,
                                              double angleStep, double currentAngle, double radius, T axisMaxValue)
    {
        var path = new StringBuilder("M ");
        var points = new List<SvgPathPoint>();

        for (var i = 0; i < Math.Min(series.Data.Values.Count, numAxes); i++)
        {
            var value = series.Data[i].Y;
            var scale = radius * (axisMaxValue == T.Zero ? 0 : double.CreateSaturating(value / axisMaxValue));
            scale = Math.Max(0, scale);

            var angle = currentAngle + i * angleStep;
            var x = Math.Cos(angle) * scale;
            var y = Math.Sin(angle) * scale;

            path.Append($"{ToS(x)} {ToS(y)} L ");
            points.Add(new SvgPathPoint
            {
                Index = seriesIndex,
                PointIndex = i,
                LabelX = x,
                LabelY = y,
                LabelXValue = value.ToString(null, CultureInfo.InvariantCulture),
                LabelYValue = series.Name
            });
        }

        path.Length -= 2;
        path.Append('Z');

        return (path.ToString(), points);
    }

    private void GenerateAxisValues(double currentAngle, T axisMaxValue, double radius)
    {
        var axisAngle = currentAngle;
        var gridLevels = T.CreateSaturating(ChartOptions.GridLevels);
        var stepValue = axisMaxValue / gridLevels;

        for (var i = T.One; i <= gridLevels; i++)
        {
            var value = i * stepValue;
            var valueRadius = radius * double.CreateSaturating(i / gridLevels);
            var x = Math.Cos(axisAngle) * valueRadius;
            var y = Math.Sin(axisAngle) * valueRadius;

            _axisValues.Add(new SvgPath
            {
                LabelX = x + 5,
                LabelY = y - 1,
                LabelYValue = value.ToString()
            });
        }
    }

    private void GenerateAxisLines(int numAxes, double angleStep, double currentAngle, double radius, string[] labelData)
    {
        for (var i = 0; i < numAxes; i++)
        {
            var angle = currentAngle + i * angleStep;
            var xOuter = Math.Cos(angle) * radius;
            var yOuter = Math.Sin(angle) * radius;

            _axisLines.Add(new SvgPath
            {
                Data = $"M 0 0 L {ToS(xOuter)} {ToS(yOuter)}",
                LabelX = Math.Cos(angle) * (radius * 1.06),
                LabelY = Math.Sin(angle) * (radius * 1.08),
                LabelYValue = labelData.Length > i ? labelData[i] : $"Axis {i + 1}"
            });
        }
    }

    private void GenerateGridLines(int numAxes, double angleStep, double currentAngle, double radius)
    {
        var gridLevels = ChartOptions.GridLevels;
        for (var i = 1; i <= gridLevels; i++)
        {
            var gridRadius = radius * (i / (double)gridLevels);
            var pathStringBuilder = new StringBuilder("M ");

            for (var j = 0; j < numAxes; j++)
            {
                var angle = currentAngle + j * angleStep;
                var x = Math.Cos(angle) * gridRadius;
                var y = Math.Sin(angle) * gridRadius;
                pathStringBuilder.Append($"{ToS(x)} {ToS(y)} L ");
            }

            pathStringBuilder.Length -= 2;
            pathStringBuilder.Append('Z');

            _gridLines.Add(new SvgPath { Data = pathStringBuilder.ToString() });
        }
    }

    private T CalculateAxisMaxValue(T actualMaxValue)
    {
        var gridLevels = ChartOptions.GridLevels;
        var minStep = actualMaxValue / T.CreateSaturating(gridLevels);
        var step = FindNextNiceStep(minStep);

        return T.CreateSaturating(step * gridLevels);
    }

    private static double FindNextNiceStep(T minStep)
    {
        return Math.Ceiling(double.CreateSaturating(minStep) / 5) * 5;
    }

    private static (List<ChartSeries<T>> Series, string[] Labels) GroupDataSet(string[] labels, List<ChartSeries<T>> dataSet, bool groupByDataSet = false)
    {
        if (groupByDataSet)
            return (dataSet, labels);

        var groupedData = new List<ChartSeries<T>>();
        var dataLength = dataSet.Count != 0
                            ? dataSet.Max(series => series.Data.Values.Count)
                            : 0;

        for (var i = 0; i < dataLength; i++)
        {
            var data = dataSet.Select(series => i < series.Data.Values.Count ? series.Data.Values[i] : T.Zero).ToArray();
            var label = i < labels.Length ? labels[i] : $"Axis {i + 1}";

            groupedData.Add(new ChartSeries<T>
            {
                Name = label,
                Data = data
            });
        }

        var newLabels = dataSet.Select(ds => ds.Name).ToArray();

        return (groupedData, newLabels);
    }

    internal override void OnSegmentMouseOver(MouseEventArgs args, SvgPath segment)
    {
        base.OnSegmentMouseOver(args, segment);

        HoveredPathIndex = segment.Index;
    }

    internal override void OnSegmentMouseOut()
    {
        base.OnSegmentMouseOut();

        HoveredPathIndex = null;
    }

    internal async Task SetSelectedPointAsync(SvgPathPoint point)
    {
        SelectedPointIndex = point.PointIndex;

        await SetSelectedIndexAsync(point.Index);
    }
}
