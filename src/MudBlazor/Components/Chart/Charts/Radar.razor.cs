// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;

namespace MudBlazor.Charts;

public partial class Radar : MudRadialChartBase<RadarChartOptions>
{
    public static new ChartType ChartType => ChartType.Radar;

    protected List<SvgPath> _gridLines = [];
    protected List<SvgPath> _axisLines = [];
    protected List<SvgPath> _axisValues = [];

    public int? SelectedPointIndex { get; set; }
    public int? HoveredPathIndex { get; set; }

    protected override void OnInitialized()
    {
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

        if (ChartSeries == null || ChartSeries.Count == 0 || ChartSeries.All(s => s.Data == null || !s.Data.Any()))
            return;

        var normalizedData = GetNormalizedData();
        var (seriesData, labelData) = Radar.GroupDataSet(ChartLabels ?? [], ChartSeries, ChartOptions!.AggregationOption == AggregationOption.GroupByDataSet);
        var numAxes = labelData.Length;

        // Setup Legends
        for (var i = 0; i < seriesData.Count; i++)
        {
            var label = seriesData[i].Name;

            if (label.Length == 0)
                continue;

            var legend = new SvgLegend()
            {
                Index = i,
                Labels = label,
                Visible = ChartOptions.AggregationOption == AggregationOption.GroupByLabel ? !HiddenIndices.Contains(i) : ChartSeries[i].Visible,
                OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
            };
            _legends.Add(legend);
        }

        var angleStep = 2 * Math.PI / numAxes;
        var currentAngle = -Math.PI / 2 + (ChartOptions.AngleOffset * (Math.PI / 180)); // Convert offset to radians

        // Determine overall max value for scaling, considering all series
        var globalMaxValue = 1.0;

        globalMaxValue = Math.Max(1, seriesData.Where((x, i) => x.Visible && !HiddenIndices.Contains(i)).SelectMany(s => s.Data).DefaultIfEmpty(0).Max());

        var radius = Radius;
        if (ChartOptions.ShowAxisLabels)
        {
            var labelReservedSpace = ChartOptions.ShowAxisLabels ? MatchBoundsToSize ? 60 : 40 : 0;
            var maxAllowableRadius = Math.Min((_boundWidth - labelReservedSpace) / 2.0, (_boundHeight - labelReservedSpace) / 2.0);

            radius = Math.Min(Radius, maxAllowableRadius);
        }

        // Draw grid lines
        if (ChartOptions.ShowGridLines)
        {
            var gridLevels = ChartOptions.GridLevels; // e.g., 5 levels
            for (var i = 1; i <= gridLevels; i++)
            {
                var gridRadius = radius * (i / (double)gridLevels);
                var pathStringBuilder = new StringBuilder();
                pathStringBuilder.Append("M ");
                for (var j = 0; j < numAxes; j++)
                {
                    var angle = currentAngle + j * angleStep;
                    var x = Math.Cos(angle) * gridRadius;
                    var y = Math.Sin(angle) * gridRadius;
                    pathStringBuilder.Append($"{ToS(x)} {ToS(y)} L ");
                }
                pathStringBuilder.Length -= 2; // Remove last "L "
                pathStringBuilder.Append('Z'); // Close path
                _gridLines.Add(new SvgPath { Data = pathStringBuilder.ToString() });
            }
        }

        // Draw axis lines and labels
        for (var i = 0; i < numAxes; i++)
        {
            var angle = currentAngle + i * angleStep;
            var xOuter = Math.Cos(angle) * radius;
            var yOuter = Math.Sin(angle) * radius;

            _axisLines.Add(new SvgPath { Data = $"M 0 0 L {ToS(xOuter)} {ToS(yOuter)}", LabelX = Math.Cos(angle) * (radius * 1.06), LabelY = Math.Sin(angle) * (radius * 1.08), LabelYValue = labelData.Length > i ? labelData[i] : $"Axis {i + 1}" });
        }

        var axisMaxValue = globalMaxValue;

        // Draw axis values
        if (ChartOptions.ShowAxisValues && numAxes > 0)
        {
            axisMaxValue = CalculateAxisMaxValue(globalMaxValue);

            var axisAngle = currentAngle; // First axis (vertical upward)
            var gridLevels = ChartOptions.GridLevels;
            var stepValue = axisMaxValue / gridLevels;

            for (var i = 1; i <= gridLevels; i++)
            {
                var value = i * stepValue;
                var valueRadius = radius * (i / (double)gridLevels);
                var x = Math.Cos(axisAngle) * valueRadius;
                var y = Math.Sin(axisAngle) * valueRadius;

                _axisValues.Add(new SvgPath
                {
                    LabelX = x + 5, // Offset slightly to avoid overlapping the axis line
                    LabelY = y - 1,
                    LabelYValue = ((int)value).ToString()
                });
            }
        }

        // Draw data series
        for (var seriesIndex = 0; seriesIndex < seriesData.Count; seriesIndex++)
        {
            var series = seriesData[seriesIndex];

            if (series.Data == null || !series.Data.Any() || !series.Visible || HiddenIndices.Contains(seriesIndex))
                continue;

            var pathStringBuilder = new StringBuilder();
            pathStringBuilder.Append("M ");
            var seriesPoints = new List<SvgPathPoint>();

            for (var i = 0; i < Math.Min(series.Data.Values.Length, numAxes); i++) // Ensure we don't go beyond numAxes
            {
                var value = series.Data[i];
                var scale = radius * (value / axisMaxValue); // Scale based on axis max value
                scale = Math.Max(0, scale); // Ensure non-negative radius

                var angle = currentAngle + i * angleStep;
                var x = Math.Cos(angle) * scale;
                var y = Math.Sin(angle) * scale;
                pathStringBuilder.Append($"{ToS(x)} {ToS(y)} L ");
                seriesPoints.Add(new SvgPathPoint()
                {
                    Index = seriesIndex,
                    PointIndex = i,
                    LabelX = x,
                    LabelY = y,
                    LabelXValue = value.ToS(),
                    LabelYValue = series.Name
                });
            }
            pathStringBuilder.Length -= 2; // Remove last "L "
            pathStringBuilder.Append('Z'); // Close path

            var path = new SvgPolygon
            {
                Index = seriesIndex,
                Data = pathStringBuilder.ToString(),
                Points = seriesPoints,
                LabelXValue = ChartOptions.ShowAsPercentage ? Math.Round(normalizedData[seriesIndex] * 100, 1).ToInvariantString() + "%" : series.Data.Values.Sum().ToS(),
                LabelYValue = series.Name,
            };

            _paths.Add(path);
        }
    }

    private static (List<ChartSeries> Series, string[] Labels) GroupDataSet(string[] labels, List<ChartSeries> dataSet, bool groupByDataSet = false)
    {
        if (groupByDataSet)
            return (dataSet, labels);

        var groupedData = new List<ChartSeries>();
        var dataLength = dataSet[0].Data.Values.Length;

        for (var i = 0; i < dataLength; i++)
        {
            var data = dataSet.Select(series => series.Data[i]).ToArray();
            var label = i < labels.Length ? labels[i] : $"Axis {i + 1}";

            groupedData.Add(new ChartSeries
            {
                Name = label,
                Data = data
            });
        }

        var newLabels = dataSet.Select(ds => ds.Name).ToArray();

        return (groupedData, newLabels);
    }


    private double CalculateAxisMaxValue(double actualMaxValue)
    {
        var gridLevels = ChartOptions.GridLevels;
        var minStep = actualMaxValue / gridLevels;

        var step = FindNextNiceStep(minStep);
        return step * gridLevels;
    }

    private static double FindNextNiceStep(double minStep)
    {
        return Math.Ceiling(minStep / 5) * 5;
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
