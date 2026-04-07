// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interpolation;

namespace MudBlazor.Charts;

/// <summary>
/// Represents a chart which displays series values as individual data points plotted by X and Y coordinates.
/// </summary>
/// <remarks>
/// Each data point must have an <c>X</c> value set on the <see cref="ChartPoint{T}"/>.
/// Use the <c>(T x, T y)</c> constructor or the implicit conversion from a <c>(T, T)</c> tuple.
/// </remarks>
/// <seealso cref="Line{T}"/>
/// <seealso cref="Bar{T}"/>
/// <seealso cref="TimeSeries{T}"/>
partial class ScatterPlot<T> : MudAxisLineChartBase<T, ScatterPlotChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    public override RenderFragment? OverlayContent { get; set; }

    protected override bool ShouldInterpolate => false;

    protected override bool ShouldAlwaysPopulateDataPoints => true;

    // X-axis scale information, computed during RebuildChart
    private T _gridXUnits;
    private int _lowestVerticalLine;
    private double _horizontalSpacePerXUnit;

    protected override void OnInitialized()
    {
        ChartType = ChartType.ScatterPlot;
        ChartOptions ??= new ScatterPlotChartOptions();

        base.OnInitialized();
    }

    public override void RebuildChart()
    {
        Series = (ChartContainer != null && ChartReference is MudChart<T>)
            ? ChartContainer.ChartSeries
            : ChartSeries;

        SetBounds();
        ComputeXAxisScale(out _gridXUnits, out _lowestVerticalLine, out var numVerticalLines);
        ComputeYAxisScale(out var gridYUnits, out var lowestHorizontalLine, out var numHorizontalLines);

        var verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);
        _horizontalSpacePerXUnit = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, numVerticalLines - 1);

        GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
        GenerateVerticalGridLines(numVerticalLines, 0, _horizontalSpacePerXUnit);
        GenerateChartLines(lowestHorizontalLine, gridYUnits, _horizontalSpacePerXUnit, verticalSpace);
        GenerateLegends();
    }

    private void ComputeYAxisScale(out T gridYUnits, out int lowestHorizontalLine, out int numHorizontalLines)
    {
        gridYUnits = ChartOptions?.YAxisTicks > 0
            ? T.CreateSaturating(ChartOptions.YAxisTicks)
            : T.CreateSaturating(20);

        var visiblePoints = Series
            .Where(s => s.Visible)
            .SelectMany(s => s.Data.Points)
            .ToArray();

        if (visiblePoints.Length == 0)
        {
            lowestHorizontalLine = 0;
            numHorizontalLines = 1;
            return;
        }

        var minY = visiblePoints.Min(p => p.Y);
        var maxY = visiblePoints.Max(p => p.Y);

        if (ChartOptions?.YAxisSuggestedMax is { } suggestedMax)
        {
            maxY = T.Max(T.CreateSaturating(suggestedMax), maxY);
        }

        if (ChartOptions?.YAxisRequireZeroPoint is true)
        {
            minY = T.Min(minY, T.Zero);
            maxY = T.Max(maxY, T.Zero);
        }

        lowestHorizontalLine = (int)Math.Floor(double.CreateSaturating(minY) / double.CreateSaturating(gridYUnits));
        var highestHorizontalLine = (int)Math.Ceiling(double.CreateSaturating(maxY) / double.CreateSaturating(gridYUnits));
        numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

        var maxYTicks = ChartOptions?.MaxNumYAxisTicks ?? 20;
        while (numHorizontalLines > maxYTicks)
        {
            gridYUnits *= T.CreateSaturating(2);
            lowestHorizontalLine = (int)Math.Floor(double.CreateSaturating(minY) / double.CreateSaturating(gridYUnits));
            highestHorizontalLine = (int)Math.Ceiling(double.CreateSaturating(maxY) / double.CreateSaturating(gridYUnits));
            numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
        }
    }

    private void ComputeXAxisScale(out T gridXUnits, out int lowestVerticalLine, out int numVerticalLines)
    {
        gridXUnits = ChartOptions?.XAxisTicks > 0
            ? T.CreateSaturating(ChartOptions.XAxisTicks)
            : T.CreateSaturating(20);

        var xValues = Series
            .Where(s => s.Visible)
            .SelectMany(s => s.Data.Points)
            .Select(p => p.X)
            .OfType<T>()
            .ToArray();

        if (xValues.Length == 0)
        {
            lowestVerticalLine = 0;
            numVerticalLines = 1;
            return;
        }

        var minX = xValues.Min();
        var maxX = xValues.Max();

        lowestVerticalLine = (int)Math.Floor(double.CreateSaturating(minX) / double.CreateSaturating(gridXUnits));
        var highestVerticalLine = (int)Math.Ceiling(double.CreateSaturating(maxX) / double.CreateSaturating(gridXUnits));
        numVerticalLines = highestVerticalLine - lowestVerticalLine + 1;

        var maxXTicks = ChartOptions?.MaxNumXAxisTicks ?? 20;
        while (numVerticalLines > maxXTicks)
        {
            gridXUnits *= T.CreateSaturating(2);
            lowestVerticalLine = (int)Math.Floor(double.CreateSaturating(minX) / double.CreateSaturating(gridXUnits));
            highestVerticalLine = (int)Math.Ceiling(double.CreateSaturating(maxX) / double.CreateSaturating(gridXUnits));
            numVerticalLines = highestVerticalLine - lowestVerticalLine + 1;
        }
    }

    protected override string GetVerticalGridLineLabel(int index)
    {
        var value = T.CreateSaturating(_lowestVerticalLine + index) * _gridXUnits;
        return ChartOptions?.XAxisFormat is { } fmt
            ? value.ToString(fmt, null)
            : value.ToString(null, null);
    }

    protected override TReturn GetDataValue<TReturn>(int seriesIndex, int dataPointIndex)
    {
        return (TReturn)Convert.ChangeType(Series[seriesIndex].Data.Points[dataPointIndex].Y, typeof(TReturn));
    }

    protected override string GetLabelXValue(int seriesIndex, int dataPointIndex)
    {
        var x = Series[seriesIndex].Data.Points[dataPointIndex].X;
        if (x is T xVal)
        {
            return ChartOptions?.XAxisFormat is { } fmt
                ? xVal.ToString(fmt, null)
                : xVal.ToString(null, null);
        }

        return x?.ToString() ?? string.Empty;
    }

    protected override (double x, double y) GetXYForDataPoint(int seriesIndex, int dataPointIndex, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
    {
        var point = Series[seriesIndex].Data.Points[dataPointIndex];

        // Map Y to screen coordinate
        var gridValueY = ((double.CreateSaturating(point.Y) / double.CreateSaturating(gridYUnits)) - lowestHorizontalLine) * verticalSpace;
        var screenY = _boundHeight - VerticalStartSpace - gridValueY;

        // Map X to screen coordinate using the X-axis scale
        double screenX;
        if (point.X is T xVal)
        {
            var gridValueX = ((double.CreateSaturating(xVal) / double.CreateSaturating(_gridXUnits)) - _lowestVerticalLine) * _horizontalSpacePerXUnit;
            screenX = HorizontalStartSpace + gridValueX;
        }
        else
        {
            // Scatter plots require an X value of type T for every point; non-numeric or missing X values are not supported.
            throw new InvalidOperationException($"Scatter plot data point at index {dataPointIndex} in series {seriesIndex} has a non-numeric or missing X value. Each data point must have an X value of type {typeof(T).Name}.");
        }

        return (screenX, screenY);
    }

    internal override ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
    {
        throw new NotSupportedException("Interpolation is not supported for scatter plot charts.");
    }
}
