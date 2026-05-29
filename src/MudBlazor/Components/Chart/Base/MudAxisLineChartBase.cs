// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Interpolation;

namespace MudBlazor.Charts;

/// <summary>
/// Represents a base class for axis-based line charts.
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
    /// The SVG circles for the chart data points.
    /// </summary>
    protected Dictionary<int, List<SvgCircle>> ChartDataPoints { get; set; } = [];

    /// <summary>
    /// The SVG path for the hovered data point.
    /// </summary>
    protected SvgPath? HoveredDataPointPath { get; set; }

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
    /// Creates a line interpolator for a series.
    /// </summary>
    /// <param name="seriesIndex">The index of the series.</param>
    /// <param name="lowestHorizontalLine">The lowest horizontal line.</param>
    /// <param name="gridYUnits">The Y-axis grid units.</param>
    /// <param name="horizontalSpace">The horizontal space between points.</param>
    /// <param name="verticalSpace">The vertical space between points.</param>
    /// <returns>The line interpolator.</returns>
    internal abstract ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace);

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

        if (numVerticalLines == 0 || !Series.Any(x => x.Data.Values.Any()))
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

            var xLabels = GetVerticalGridLineLabel(i);

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
        ChartDataPoints.Clear();

        if (Series.Count == 0)
        {
            return;
        }

        for (var i = 0; i < Series.Count; i++)
        {
            var series = Series[i];

            if (!series.Visible || !series.Data.Points.Any())
            {
                continue;
            }

            var chartLine = new StringBuilder();
            var chartDataCircles = new List<SvgCircle>();
            ChartDataPoints[i] = chartDataCircles;

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

            var displayType = overrideSettings?.LineDisplayType ?? ChartOptions?.LineDisplayType;

            if (displayType == LineDisplayType.Area)
            {
                GenerateAreaChart(i, chartLine, lowestHorizontalLine, firstPointX, firstPointY, lastPointX);
            }
        }
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

        for (var j = 0; j < dataLength; j++)
        {
            var (x, y) = GetXYForDataPoint(seriesIndex, j, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);

            if (j == 0)
            {
                chartLine.Append("M ");
                firstPointX = x;
                firstPointY = y;
            }
            else
            {
                chartLine.Append(" L ");
            }

            if (j == dataLength - 1)
            {
                lastPointX = x;
            }

            chartLine.Append(ToS(x));
            chartLine.Append(' ');
            chartLine.Append(ToS(y));

            if (ChartOptions?.ShowToolTips == true || ShouldAlwaysPopulateDataPoints)
            {
                chartDataCircles.Add(new SvgCircle
                {
                    Index = seriesIndex,
                    CX = x,
                    CY = y,
                    LabelX = x,
                    LabelXValue = GetLabelXValue(seriesIndex, j),
                    LabelY = y,
                    LabelYValue = GetDataValueAsString(seriesIndex, j)
                });
            }
        }

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

        const int InterpolationResolution = 10;
        var interpolator = CreateInterpolator(seriesIndex, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);

        var series = Series[seriesIndex];
        var isPositiveOnly = series.Data.Values.All(v => v >= T.Zero);
        var zeroPointY = GetYForZeroPoint(lowestHorizontalLine);

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
                chartLine.Append("M ");
                firstPointX = x;
                firstPointY = y;
            }
            else
            {
                chartLine.Append(" L ");
            }

            if (j == interpolator.InterpolatedYs.Length - 1)
            {
                lastPointX = x;
            }

            chartLine.Append(ToS(x));
            chartLine.Append(' ');
            chartLine.Append(ToS(y));

            var originalIndex = j / InterpolationResolution;
            // Add tooltip points for interpolated data if needed
            if (j % InterpolationResolution == 0 && ChartOptions?.ShowToolTips == true &&
                Series[seriesIndex].Data != null && originalIndex < Series[seriesIndex].Data.Points.Count)
            {

                chartDataCircles.Add(new SvgCircle
                {
                    Index = seriesIndex,
                    CX = x,
                    CY = y,
                    LabelX = x,
                    LabelXValue = GetLabelXValue(seriesIndex, originalIndex),
                    LabelY = y,
                    LabelYValue = GetDataValueAsString(seriesIndex, originalIndex)
                });
            }
        }

        return (firstPointX, firstPointY, lastPointX);
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
        chartArea.Append(ToS(lastPointX));
        chartArea.Append(' ');
        chartArea.Append(ToS(zeroPointY));

        // add an extra point based on the x of the first point and 0 to close the area
        chartArea.Append(" L ");
        chartArea.Append(ToS(firstPointX));
        chartArea.Append(' ');
        chartArea.Append(ToS(zeroPointY));

        // add an the first point again to close the area
        chartArea.Append(" L ");
        chartArea.Append(ToS(firstPointX));
        chartArea.Append(' ');
        chartArea.Append(ToS(firstPointY));
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
