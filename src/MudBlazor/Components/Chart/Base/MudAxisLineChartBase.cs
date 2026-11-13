// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Interpolation;

namespace MudBlazor.Charts;

/// <summary>
/// Base class for line-based charts such as <see cref="Line{T}"/>, <see cref="ScatterPlot{T}"/>, and <see cref="TimeSeries{T}"/>.
/// Generates the SVG line paths, area fills, and data-point markers, with optional interpolation.
/// </summary>
/// <typeparam name="T">The data type of the chart.</typeparam>
/// <typeparam name="TOptions">The type of options for the chart.</typeparam>
public abstract class MudAxisLineChartBase<T, TOptions> : MudAxisChartBase<T, TOptions>
    where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    where TOptions : IAxisLineChartOptions
{
    /// <summary>
    /// The SVG paths for the chart lines.
    /// </summary>
    protected List<SvgPath> ChartLines { get; set; } = [];

    /// <summary>
    /// The SVG paths for the chart areas.
    /// </summary>
    protected Dictionary<int, SvgPath> ChartAreas { get; set; } = [];

    /// <summary>
    /// The SVG paths for readings that have no neighbour to draw a segment to, keyed by series index.
    /// </summary>
    /// <remarks>
    /// These are zero-length segments and only paint under a round line cap.
    /// So they are kept apart from the series line, which keeps its own caps.
    /// </remarks>
    protected Dictionary<int, SvgPath> ChartDots { get; set; } = [];

    private readonly StringBuilder _chartDots = new();
    private readonly StringBuilder _chartLine = new();

    /// <summary>
    /// The SVG circles for the chart data points.
    /// </summary>
    protected Dictionary<int, List<SvgCircle>> ChartDataPoints { get; set; } = [];

    /// <summary>
    /// The SVG path for the hovered data point.
    /// </summary>
    protected SvgPath? HoveredDataPointPath { get; set; }

    /// <summary>
    /// Wraps a tooltip so the <see cref="ChartTooltip"/> inside it, including one from a <c>TooltipTemplate</c>, knows the room around the hovered point.
    /// </summary>
    /// <param name="tooltip">The tooltip to wrap.</param>
    /// <param name="labelOffset">How far above the point the tooltip is anchored.</param>
    private protected RenderFragment WithTooltipRoom(RenderFragment tooltip, double labelOffset) =>
        WithTooltipRoom(tooltip, HoveredDataPointPath, labelOffset);

    /// <summary>
    /// Indicates whether the chart should be interpolated.
    /// </summary>
    protected abstract bool ShouldInterpolate { get; }

    /// <summary>
    /// When <c>true</c>, data points are always added to <see cref="ChartDataPoints"/> regardless of
    /// <see cref="IChartOptions.ShowToolTips"/>. Override to <c>true</c> for charts (e.g. scatter plot)
    /// that use <see cref="ChartDataPoints"/> to render visible markers, not only tooltips.
    /// </summary>
    protected virtual bool ShouldAlwaysPopulateDataPoints => false;

    /// <summary>
    /// Whether the data points behind the tooltips and the visible markers need to be collected on this render.
    /// </summary>
    /// <remarks>
    /// The markers are drawn from the same data points as the tooltips, so a chart with markers on and tooltips off used to draw nothing.
    /// </remarks>
    private bool ShouldPopulateDataPoints =>
        ChartOptions?.ShowToolTips == true
        || ChartOptions?.ShowDataMarkers == true
        || ShouldAlwaysPopulateDataPoints;

    /// <summary>
    /// Gets the data value for a specific series and data point.
    /// </summary>
    /// <typeparam name="TReturn">The type of the return value.</typeparam>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="dataPointIndex">The index of the data point.</param>
    /// <returns>The data value.</returns>
    protected abstract TReturn GetDataValue<TReturn>(int seriesIndex, int dataPointIndex);

    /// <summary>
    /// Gets the X-axis label for a specific series and data point.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="dataPointIndex">The index of the data point.</param>
    /// <returns>The X-axis label.</returns>
    protected abstract string GetLabelXValue(int seriesIndex, int dataPointIndex);

    /// <summary>
    /// Gets the label for a vertical grid line.
    /// </summary>
    /// <param name="index">The index of the grid line.</param>
    /// <returns>The label for the vertical grid line.</returns>
    protected abstract string GetVerticalGridLineLabel(int index);

    /// <summary>
    /// Gets the X and Y coordinates for a data point.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="dataPointIndex">The index of the data point.</param>
    /// <param name="lowestHorizontalLine">The lowest horizontal line.</param>
    /// <param name="gridYUnits">The Y-axis grid units.</param>
    /// <param name="horizontalSpace">The horizontal space between points.</param>
    /// <param name="verticalSpace">The vertical space between points.</param>
    /// <returns>A tuple containing the X and Y coordinates.</returns>
    protected abstract (double x, double y) GetXYForDataPoint(int seriesIndex, int dataPointIndex, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace);

    /// <summary>
    /// Generates the vertical grid lines for the chart.
    /// </summary>
    /// <param name="numVerticalLines">The number of vertical lines.</param>
    /// <param name="startOffset">The starting offset.</param>
    /// <param name="horizontalSpace">The horizontal space between lines.</param>
    protected void GenerateVerticalGridLines(int numVerticalLines, double startOffset, double horizontalSpace)
    {
        VerticalLines.Clear();
        VerticalValues.Clear();

        if (numVerticalLines == 0 || !Series.Any(x => x.Data != null && x.Data.Count != 0))
        {
            return;
        }

        for (var i = 0; i < numVerticalLines; i++)
        {
            var x = startOffset + HorizontalStartSpace + (i * horizontalSpace);

            if (x > _boundWidth - HorizontalEndSpace)
            {
                break; // we are out of bounds
            }

            var line = new SvgPath
            {
                Index = i,
                Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
            };
            VerticalLines.Add(line);

            var xLabels = ShouldLabelVerticalGridLine(i) ? GetVerticalGridLineLabel(i) : string.Empty;

            var lineValue = new SvgText
            {
                X = x,
                Y = _boundHeight - XAxisLabelOffset,
                Value = xLabels,
            };
            VerticalValues.Add(lineValue);
        }
    }

    /// <summary>
    /// Generates the chart lines.
    /// </summary>
    /// <param name="lowestHorizontalLine">The lowest horizontal line.</param>
    /// <param name="gridYUnits">The Y-axis grid units.</param>
    /// <param name="horizontalSpace">The horizontal space between points.</param>
    /// <param name="verticalSpace">The vertical space between points.</param>
    protected void GenerateChartLines(int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
    {
        ChartLines.Clear();
        ChartAreas.Clear();
        ChartDots.Clear();
        ChartDataPoints.Clear();

        if (Series.Count == 0)
        {
            return;
        }

        for (var i = 0; i < Series.Count; i++)
        {
            var series = Series[i];

            if (!series.Visible || series.Data is null || series.Data.Points is null || !series.Data.Points.Any(p => p.HasValue))
            {
                ChartDataPoints[i] = new List<SvgCircle>();
                continue;
            }

            var chartLine = _chartLine.Clear();
            var chartDataCircles = new List<SvgCircle>();
            ChartDataPoints[i] = chartDataCircles;
            _chartDots.Clear();

            var overrideSettings = GetSeriesDisplayOverride(series);
            var interpolationOption = overrideSettings?.InterpolationOption ?? ChartOptions?.InterpolationOption;

            var interpolationEnabled = ShouldInterpolate && interpolationOption is not InterpolationOption.Straight and not null && series.Data.Count > 2;

            var (firstPointX, firstPointY, lastPointX) = interpolationEnabled
                ? GenerateInterpolatedLines(i, chartLine, chartDataCircles, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace)
                : GenerateStraightLines(i, chartLine, chartDataCircles, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);

            var line = new SvgPath
            {
                Index = i,
                Data = chartLine.ToString()
            };
            ChartLines.Add(line);

            if (_chartDots.Length > 0)
            {
                ChartDots[i] = new SvgPath
                {
                    Index = i,
                    Data = _chartDots.ToString()
                };
            }

            var displayType = overrideSettings?.LineDisplayType ?? ChartOptions?.LineDisplayType;

            if (displayType == LineDisplayType.Area)
            {
                GenerateAreaChart(i, chartLine, lowestHorizontalLine, firstPointX, firstPointY, lastPointX);
            }
        }
    }

    // Writes the number straight into the path, where ToS would make a string for every coordinate first.
    private static void AppendNumber(StringBuilder builder, double value) =>
        builder.Append(CultureInfo.InvariantCulture, $"{Math.Round(value, 4)}");

    private void AppendDot(double x, double y)
    {
        _chartDots.Append(_chartDots.Length > 0 ? " M " : "M ");
        AppendNumber(_chartDots, x);
        _chartDots.Append(' ');
        AppendNumber(_chartDots, y);
        _chartDots.Append(" L ");
        AppendNumber(_chartDots, x);
        _chartDots.Append(' ');
        AppendNumber(_chartDots, y);
    }

    /// <summary>
    /// Generates straight lines for a series.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="chartLine">The string builder for the chart line.</param>
    /// <param name="chartDataCircles">The list of circles for data points.</param>
    /// <param name="lowestHorizontalLine">The lowest horizontal line.</param>
    /// <param name="gridYUnits">The Y-axis grid units.</param>
    /// <param name="horizontalSpace">The horizontal space between points.</param>
    /// <param name="verticalSpace">The vertical space between points.</param>
    /// <returns>A tuple containing the first X, first Y, and last X coordinates.</returns>
    protected (double firstX, double firstY, double lastX) GenerateStraightLines(int seriesIndex,
                                                                                 StringBuilder chartLine,
                                                                                 List<SvgCircle> chartDataCircles,
                                                                                 int lowestHorizontalLine,
                                                                                 T gridYUnits,
                                                                                 double horizontalSpace,
                                                                                 double verticalSpace)
    {
        double firstPointX = 0, firstPointY = 0, lastPointX = 0;

        var series = Series[seriesIndex];
        var dataLength = series.Data.Points.Count;
        var needsMoveTo = true;

        // A run of one point produces a bare move, which draws nothing at all.
        // So the point goes to the dots path as well, where a zero-length segment under a round cap paints it.
        double segmentX = 0, segmentY = 0;
        var segmentLength = 0;

        void CloseSegment()
        {
            if (segmentLength == 1)
            {
                AppendDot(segmentX, segmentY);
            }

            segmentLength = 0;
        }

        for (var j = 0; j < dataLength; j++)
        {
            if (!series.Data[j].HasValue)
            {
                if (ChartOptions?.ConnectNullPoints != true)
                {
                    CloseSegment();
                    needsMoveTo = true;
                }
                continue;
            }

            var (x, y) = GetXYForDataPoint(seriesIndex, j, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);

            if (needsMoveTo)
            {
                chartLine.Append(chartLine.Length > 0 ? " M " : "M ");
                needsMoveTo = false;

                if (firstPointX == 0 && firstPointY == 0)
                {
                    firstPointX = x;
                    firstPointY = y;
                }
            }
            else
            {
                chartLine.Append(" L ");
            }

            lastPointX = x;
            segmentX = x;
            segmentY = y;
            segmentLength++;

            AppendNumber(chartLine, x);
            chartLine.Append(' ');
            AppendNumber(chartLine, y);

            if (ShouldPopulateDataPoints)
            {
                chartDataCircles.Add(new SvgCircle
                {
                    Index = seriesIndex,
                    CX = x,
                    CY = y,
                    LabelX = x,
                    LabelXValue = GetLabelXValue(seriesIndex, j),
                    LabelY = y,
                    LabelYValue = GetDataValueAsString(seriesIndex, j),
                    YValue = GetDataValueAsDouble(seriesIndex, j)
                });
            }
        }

        CloseSegment();

        return (firstPointX, firstPointY, lastPointX);
    }

    /// <summary>
    /// Generates interpolated lines for a series.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="chartLine">The string builder for the chart line.</param>
    /// <param name="chartDataCircles">The list of circles for data points.</param>
    /// <param name="lowestHorizontalLine">The lowest horizontal line.</param>
    /// <param name="gridYUnits">The Y-axis grid units.</param>
    /// <param name="horizontalSpace">The horizontal space between points.</param>
    /// <param name="verticalSpace">The vertical space between points.</param>
    /// <returns>A tuple containing the first X, first Y, and last X coordinates.</returns>
    protected (double firstX, double firstY, double lastX) GenerateInterpolatedLines(int seriesIndex,
                                                                                     StringBuilder chartLine,
                                                                                     List<SvgCircle> chartDataCircles,
                                                                                     int lowestHorizontalLine,
                                                                                     T gridYUnits,
                                                                                     double horizontalSpace,
                                                                                     double verticalSpace)
    {
        double firstPointX = 0, firstPointY = 0, lastPointX = 0;

        var series = Series[seriesIndex];
        var data = series.Data;
        var isPositiveOnly = data.Points.Where(p => p.HasValue).All(p => p.Y >= T.Zero);
        var zeroPointY = GetYForZeroPoint(lowestHorizontalLine);

        // Split data into contiguous segments of non-null points
        var segments = ChartOptions?.ConnectNullPoints == true
            ? [Enumerable.Range(0, data.Count).Where(i => data[i].HasValue).ToList()]
            : GetContiguousSegments(data);

        foreach (var segment in segments)
        {
            if (segment.Count <= 1)
            {
                // Single point, so there is nothing to interpolate and the move below paints nothing on its own.
                var (sx, sy) = GetXYForDataPoint(seriesIndex, segment[0], lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
                chartLine.Append(chartLine.Length > 0 ? " M " : "M ");
                AppendNumber(chartLine, sx);
                chartLine.Append(' ');
                AppendNumber(chartLine, sy);
                AppendDot(sx, sy);

                if (firstPointX == 0 && firstPointY == 0)
                {
                    firstPointX = sx;
                    firstPointY = sy;
                }
                lastPointX = sx;

                if (ShouldPopulateDataPoints)
                {
                    chartDataCircles.Add(new SvgCircle
                    {
                        Index = seriesIndex,
                        CX = sx,
                        CY = sy,
                        LabelX = sx,
                        LabelXValue = GetLabelXValue(seriesIndex, segment[0]),
                        LabelY = sy,
                        LabelYValue = GetDataValueAsString(seriesIndex, segment[0]),
                        YValue = GetDataValueAsDouble(seriesIndex, segment[0])
                    });
                }

                continue;
            }

            var interpolator = CreateInterpolator(seriesIndex, segment, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
            const int InterpolationResolution = 10;

            for (var j = 0; j < interpolator.InterpolatedYs.Length; j++)
            {
                var x = interpolator.InterpolatedXs[j];
                var y = interpolator.InterpolatedYs[j];

                if (ChartOptions?.ClampToZero is true && isPositiveOnly && y > zeroPointY)
                {
                    y = zeroPointY;
                }

                if (j == 0)
                {
                    chartLine.Append(chartLine.Length > 0 ? " M " : "M ");

                    if (firstPointX == 0 && firstPointY == 0)
                    {
                        firstPointX = x;
                        firstPointY = y;
                    }
                }
                else
                {
                    chartLine.Append(" L ");
                }

                lastPointX = x;

                AppendNumber(chartLine, x);
                chartLine.Append(' ');
                AppendNumber(chartLine, y);

                var localIndex = j / InterpolationResolution;
                // Collect the data point that this stretch of the interpolated line came from
                if (j % InterpolationResolution == 0 && ShouldPopulateDataPoints &&
                    localIndex < segment.Count)
                {
                    var originalIndex = segment[localIndex];
                    chartDataCircles.Add(new SvgCircle
                    {
                        Index = seriesIndex,
                        CX = x,
                        CY = y,
                        LabelX = x,
                        LabelXValue = GetLabelXValue(seriesIndex, originalIndex),
                        LabelY = y,
                        LabelYValue = GetDataValueAsString(seriesIndex, originalIndex),
                        YValue = GetDataValueAsDouble(seriesIndex, originalIndex)
                    });
                }
            }
        }

        return (firstPointX, firstPointY, lastPointX);
    }

    /// <summary>
    /// Splits data points into contiguous segments of non-null (HasValue) indices.
    /// </summary>
    private static List<List<int>> GetContiguousSegments<TData>(ChartData<TData> data)
        where TData : struct, INumber<TData>, IMinMaxValue<TData>, IFormattable
    {
        var segments = new List<List<int>>();
        List<int>? current = null;

        for (var i = 0; i < data.Count; i++)
        {
            if (data[i].HasValue)
            {
                current ??= [];
                current.Add(i);
            }
            else
            {
                if (current is not null && current.Count > 0)
                {
                    segments.Add(current);
                    current = null;
                }
            }
        }

        if (current is not null && current.Count > 0)
        {
            segments.Add(current);
        }

        return segments;
    }

    /// <summary>
    /// Creates a line interpolator for a contiguous segment of data point indices.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="segment">The segment of data point indices.</param>
    /// <param name="lowestHorizontalLine">The lowest horizontal line.</param>
    /// <param name="gridYUnits">The Y-axis grid units.</param>
    /// <param name="horizontalSpace">The horizontal space between points.</param>
    /// <param name="verticalSpace">The vertical space between points.</param>
    /// <returns>An instance of <see cref="ILineInterpolator"/>.</returns>
    internal virtual ILineInterpolator CreateInterpolator(int seriesIndex, List<int> segment, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
    {
        var xValues = new double[segment.Count];
        var yValues = new double[segment.Count];

        for (var i = 0; i < segment.Count; i++)
        {
            (xValues[i], yValues[i]) = GetXYForDataPoint(seriesIndex, segment[i], lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
        }

        var series = Series[seriesIndex];
        var overrideSettings = GetSeriesDisplayOverride(series);
        var interpolationOption = overrideSettings?.InterpolationOption ?? ChartOptions?.InterpolationOption;
        const int interpolationResolution = 10;

        return interpolationOption switch
        {
            InterpolationOption.NaturalSpline => new NaturalSpline(xValues, yValues, interpolationResolution),
            InterpolationOption.EndSlope => new EndSlopeSpline(xValues, yValues, interpolationResolution),
            InterpolationOption.Periodic => new PeriodicSpline(xValues, yValues, interpolationResolution),
            _ => throw new NotImplementedException("Interpolation option not implemented yet")
        };
    }

    /// <summary>
    /// Gets the data value as a string for a specific series and data point.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="dataPointIndex">The index of the data point.</param>
    /// <returns>The data value as a string.</returns>
    protected virtual string GetDataValueAsString(int seriesIndex, int dataPointIndex)
    {
        var value = GetDataValue<double>(seriesIndex, dataPointIndex);
        return value.ToString(Series[seriesIndex].TooltipYValueFormat);
    }

    /// <summary>
    /// Gets the data value as a number, so a tooltip can write it the way the Y axis does.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="dataPointIndex">The index of the data point.</param>
    /// <returns>The data value.</returns>
    internal virtual double GetDataValueAsDouble(int seriesIndex, int dataPointIndex) => GetDataValue<double>(seriesIndex, dataPointIndex);

    /// <summary>
    /// Adds a legend for a series.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="series">The chart series.</param>
    protected void AddLegend(int seriesIndex, ChartSeries<T> series)
    {
        var legend = new SvgLegend
        {
            Index = seriesIndex,
            Labels = series.Name,
            Visible = series.Visible,
            OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
        };

        Legends.Add(legend);
    }

    /// <summary>
    /// Generates the area chart for a series.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="chartLine">The string builder for the chart line.</param>
    /// <param name="lowestHorizontalLine">The lowest horizontal line.</param>
    /// <param name="firstPointX">The X coordinate of the first point.</param>
    /// <param name="firstPointY">The Y coordinate of the first point.</param>
    /// <param name="lastPointX">The X coordinate of the last point.</param>
    protected void GenerateAreaChart(int seriesIndex, StringBuilder chartLine, int lowestHorizontalLine,
                                     double firstPointX, double firstPointY, double lastPointX)
    {
        var chartArea = new StringBuilder();
        var zeroPointY = GetYForZeroPoint(lowestHorizontalLine);

        chartArea.Append(chartLine); // the line up to this point is the same as the area, so we can reuse it

        // add an extra point based on the x of the last point and 0 to add the area to the bottom
        chartArea.Append(" L ");
        AppendNumber(chartArea, lastPointX);
        chartArea.Append(' ');
        AppendNumber(chartArea, zeroPointY);

        // add an extra point based on the x of the first point and 0 to close the area
        chartArea.Append(" L ");
        AppendNumber(chartArea, firstPointX);
        chartArea.Append(' ');
        AppendNumber(chartArea, zeroPointY);

        // add an the first point again to close the area
        chartArea.Append(" L ");
        AppendNumber(chartArea, firstPointX);
        chartArea.Append(' ');
        AppendNumber(chartArea, firstPointY);
        chartArea.Append(" Z");

        var area = new SvgPath
        {
            Index = seriesIndex,
            Data = chartArea.ToString()
        };
        ChartAreas.Add(seriesIndex, area);
    }

    /// <summary>
    /// Gets the Y coordinate for the zero point.
    /// </summary>
    /// <param name="lowestHorizontalLine">The lowest horizontal line.</param>
    /// <returns>The Y coordinate for the zero point.</returns>
    protected double GetYForZeroPoint(int lowestHorizontalLine)
    {
        var gridValue = -lowestHorizontalLine * GetVerticalSpace();
        var y = _boundHeight - VerticalStartSpace - gridValue;

        return y;
    }

    /// <summary>
    /// Gets the vertical space of the chart.
    /// </summary>
    /// <returns>The vertical space.</returns>
    protected double GetVerticalSpace()
    {
        return _boundHeight - VerticalStartSpace;
    }

    /// <summary>
    /// Gets the series display override for a series.
    /// </summary>
    /// <param name="series">The chart series.</param>
    /// <returns>The series display override, or null if not found.</returns>
    protected SeriesDisplayOverride? GetSeriesDisplayOverride(ChartSeries<T> series)
    {
        return ChartOptions?.SeriesDisplayOverrides?.TryGetValue(series, out var overrideData) is true
            ? overrideData
            : null;
    }

    /// <summary>
    /// Handles the mouse over event for a data point.
    /// </summary>
    /// <param name="_">The mouse event arguments.</param>
    /// <param name="hoveredPoint">The hovered data point path.</param>
    protected void OnDataPointMouseOver(MouseEventArgs _, SvgPath hoveredPoint)
    {
        HoveredDataPointPath = hoveredPoint;

        if (IsOverlayChart && ChartReference is IMudStateHasChanged chart)
        {
            chart.StateHasChanged();
        }
    }

    /// <summary>
    /// Handles the mouse out event for a data point.
    /// </summary>
    protected void OnDataPointMouseOut()
    {
        HoveredDataPointPath = null;

        if (IsOverlayChart && ChartReference is IMudStateHasChanged chart)
        {
            chart.StateHasChanged();
        }
    }
}
