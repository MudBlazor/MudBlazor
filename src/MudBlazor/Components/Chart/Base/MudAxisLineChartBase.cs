// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interpolation;

namespace MudBlazor.Charts;

public abstract class MudAxisLineChartBase<TOptions> : MudAxisChartBase<TOptions> where TOptions : IAxisLineChartOptions
{
    protected List<SvgPath> ChartLines { get; set; } = [];
    protected Dictionary<int, SvgPath> ChartAreas { get; set; } = [];
    protected Dictionary<int, List<SvgCircle>> ChartDataPoints { get; set; } = [];
    protected SvgCircle? HoveredDataPoint { get; set; }
    protected SvgPath? HoverDataPointChartLine { get; set; }
    protected abstract bool ShouldInterpolate { get; }

    protected abstract T GetDataValue<T>(int seriesIndex, int dataPointIndex);
    protected abstract string GetLabelXValue(int seriesIndex, int dataPointIndex);
    protected abstract string GetVerticalGridLineLabel(int index);
    protected abstract (double x, double y) GetXYForDataPoint(int seriesIndex, int dataPointIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace);
    internal abstract ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace);

    protected void GenerateHorizontalGridLines(int numHorizontalLines, int lowestHorizontalLine, double gridYUnits, double verticalSpace)
    {
        HorizontalLines.Clear();
        HorizontalValues.Clear();

        for (var i = 0; i < numHorizontalLines; i++)
        {
            var y = VerticalStartSpace + (i * verticalSpace);
            var line = new SvgPath()
            {
                Index = i,
                Data = $"M {ToS(HorizontalStartSpace)} {ToS(_boundHeight - y)} L {ToS(_boundWidth - HorizontalEndSpace)} {ToS(_boundHeight - y)}"
            };
            HorizontalLines.Add(line);

            var startGridY = (lowestHorizontalLine + i) * gridYUnits;
            var lineValue = new SvgText()
            {
                X = HorizontalStartSpace - 10,
                Y = _boundHeight - y + 5,
                Value = ToS(startGridY, ChartOptions?.YAxisFormat)
            };
            HorizontalValues.Add(lineValue);
        }
    }

    protected void GenerateVerticalGridLines(int numVerticalLines, double startOffset, double horizontalSpace)
    {
        VerticalLines.Clear();
        VerticalValues.Clear();

        if (numVerticalLines == 0 || !Series.Any(x => x.Data.Values.Length > 0))
            return;

        for (var i = 0; i < numVerticalLines; i++)
        {
            var x = startOffset + HorizontalStartSpace + (i * horizontalSpace);

            if (x > _boundWidth - HorizontalEndSpace)
                break; // we are out of bounds

            var line = new SvgPath()
            {
                Index = i,
                Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
            };
            VerticalLines.Add(line);

            var xLabels = GetVerticalGridLineLabel(i);

            var lineValue = new SvgText()
            {
                X = x,
                Y = _boundHeight - XAxisLabelOffset,
                Value = xLabels,
            };
            VerticalValues.Add(lineValue);
        }
    }

    protected void GenerateChartLines(int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
    {
        Legends.Clear();
        ChartLines.Clear();
        ChartAreas.Clear();
        ChartDataPoints.Clear();

        if (Series.Count == 0)
            return;

        for (var i = 0; i < Series.Count; i++)
        {
            var series = Series[i];

            if (!series.Visible)
            {
                // Still add legend even if series is not visible
                AddLegend(i, series);
                continue;
            }

            var chartLine = new StringBuilder();
            var chartDataCircles = new List<SvgCircle>();
            ChartDataPoints[i] = chartDataCircles;

            var dataLength = series.Data.Points.Count;
            if (dataLength == 0) continue;

            var firstPointX = 0.0;
            var firstPointY = 0.0;
            var lastPointX = 0.0;

            var overrideSettings = GetSeriesDisplayOverride(series);
            var interpolationOption = overrideSettings?.InterpolationOption ?? ChartOptions?.InterpolationOption;

            var interpolationEnabled = ShouldInterpolate && interpolationOption is not InterpolationOption.Straight and not null;

            if (interpolationEnabled)
            {
                GenerateInterpolatedLines(i, chartLine, chartDataCircles, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace, out firstPointX, out firstPointY, out lastPointX);
            }
            else
            {
                GenerateStraightLines(i, chartLine, chartDataCircles, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace, out firstPointX, out firstPointY, out lastPointX);
            }

            var line = new SvgPath()
            {
                Index = i,
                Data = chartLine.ToString()
            };
            ChartLines.Add(line);

            var displayType = overrideSettings?.LineDisplayType ?? ChartOptions?.LineDisplayType;

            if (displayType == LineDisplayType.Area)
            {
                GenerateAreaChart(i, chartLine, lowestHorizontalLine, gridYUnits, firstPointX, firstPointY, lastPointX);
            }

            AddLegend(i, series);
        }
    }

    protected void GenerateStraightLines(int seriesIndex, StringBuilder chartLine, List<SvgCircle> chartDataCircles,
                                 int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace,
                                 out double firstPointX, out double firstPointY, out double lastPointX)
    {
        firstPointX = 0;
        firstPointY = 0;
        lastPointX = 0;

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
                chartLine.Append(" L ");

            if (j == dataLength - 1)
            {
                lastPointX = x;
            }

            chartLine.Append(ToS(x));
            chartLine.Append(' ');
            chartLine.Append(ToS(y));

            if (ChartOptions?.ShowToolTips == true)
            {
                chartDataCircles.Add(new SvgCircle()
                {
                    Index = j,
                    CX = x,
                    CY = y,
                    LabelX = x,
                    LabelXValue = GetLabelXValue(seriesIndex, j),
                    LabelY = y,
                    LabelYValue = GetDataValueAsString(seriesIndex, j)
                });
            }
        }
    }


    protected void GenerateInterpolatedLines(int seriesIndex, StringBuilder chartLine, List<SvgCircle> chartDataCircles,
                                     int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace,
                                     out double firstPointX, out double firstPointY, out double lastPointX)
    {
        firstPointX = 0;
        firstPointY = 0;
        lastPointX = 0;

        var interpolationResolution = 10;
        var interpolator = CreateInterpolator(seriesIndex, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);

        for (var j = 0; j < interpolator.InterpolatedYs.Length; j++)
        {
            var x = interpolator.InterpolatedXs[j];
            var y = interpolator.InterpolatedYs[j];

            if (j == 0)
            {
                chartLine.Append("M ");
                firstPointX = x;
                firstPointY = y;
            }
            else
                chartLine.Append(" L ");

            if (j == interpolator.InterpolatedYs.Length - 1)
            {
                lastPointX = x;
            }

            chartLine.Append(ToS(x));
            chartLine.Append(' ');
            chartLine.Append(ToS(y));

            var originalIndex = j / interpolationResolution;
            // Add tooltip points for interpolated data if needed
            if (j % interpolationResolution == 0 && ChartOptions?.ShowToolTips == true && originalIndex < Series[seriesIndex].Data?.Points.Count)
            {
                
                chartDataCircles.Add(new SvgCircle()
                {
                    Index = originalIndex,
                    CX = x,
                    CY = y,
                    LabelX = x,
                    LabelXValue = GetLabelXValue(seriesIndex, originalIndex),
                    LabelY = y,
                    LabelYValue = GetDataValueAsString(seriesIndex, originalIndex)
                });
            }
        }
    }

    protected virtual string GetDataValueAsString(int seriesIndex, int dataPointIndex)
    {
        var value = GetDataValue<double>(seriesIndex, dataPointIndex);
        return value.ToString(Series[seriesIndex].TooltipYValueFormat) ?? string.Empty;
    }

    protected void AddLegend(int seriesIndex, ChartSeries series)
    {
        var legend = new SvgLegend()
        {
            Index = seriesIndex,
            Labels = series.Name,
            Visible = series.Visible,
            OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
        };

        Legends.Add(legend);
    }

    protected void GenerateAreaChart(int seriesIndex, StringBuilder chartLine,
                             int lowestHorizontalLine, double gridYUnits,
                             double firstPointX, double firstPointY, double lastPointX)
    {
        var chartArea = new StringBuilder();
        var zeroPointY = GetYForZeroPoint(lowestHorizontalLine, gridYUnits);

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

        var area = new SvgPath()
        {
            Index = seriesIndex,
            Data = chartArea.ToString()
        };
        ChartAreas.Add(seriesIndex, area);
    }

    protected double GetYForZeroPoint(int lowestHorizontalLine, double gridYUnits)
    {
        var gridValue = (0 / gridYUnits - lowestHorizontalLine) * GetVerticalSpace();
        var y = _boundHeight - VerticalStartSpace - gridValue;

        return y;
    }

    protected double GetVerticalSpace()
    {
        return _boundHeight - VerticalStartSpace;
    }

    protected SeriesDisplayOverride? GetSeriesDisplayOverride(ChartSeries series)
    {
        return ChartOptions?.SeriesDisplayOverrides.TryGetValue(series, out var overrideData) is true
            ? overrideData
            : null;
    }

    protected void OnDataPointMouseOver(MouseEventArgs _, SvgCircle dataPoint, SvgPath seriesPath)
    {
        HoveredDataPoint = dataPoint;
        HoverDataPointChartLine = seriesPath;
    }

    protected void OnDataPointMouseOut(MouseEventArgs _)
    {
        HoveredDataPoint = null;
        HoverDataPointChartLine = null;
    }
}
