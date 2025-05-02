using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Components.Chart;
using MudBlazor.Interpolation;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays series values as connected lines.
    /// </summary>
    /// <seealso cref="Bar"/>
    /// <seealso cref="Donut"/>
    /// <seealso cref="Pie"/>
    /// <seealso cref="StackedBar"/>
    /// <seealso cref="TimeSeries"/>
    partial class Line : MudAxisLineChartBase<LineChartOptions>
    {
        protected override bool ShouldInterpolate => MudChartParent is not null;

        protected override void RebuildChart()
        {
            if (MudChartParent != null)
                Series = MudChartParent.ChartSeries;

            SetBounds();
            ComputeUnitsAndNumberOfLines(out var gridYUnits, out var numHorizontalLines, out var lowestHorizontalLine, out var numVerticalLines);

            var horizontalSpace = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, numVerticalLines - 1);
            var verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);

            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, 0, horizontalSpace);
            GenerateChartLines(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
        }

        private void ComputeUnitsAndNumberOfLines(out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            gridYUnits = ChartOptions?.YAxisTicks ?? 20;
            gridYUnits = ChartOptions?.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;

            if (Series.SelectMany(series => series.Data.Values).Any())
            {
                var minY = Series.Where(series => series.Visible).SelectMany(series => series.Data.Values).Min();
                var maxY = Series.Where(series => series.Visible).SelectMany(series => series.Data.Values).Max();

                var hasAreaDisplay = ChartOptions?.LineDisplayType == LineDisplayType.Area || Series.Any(series => GetSeriesDisplayOverride(series)?.LineDisplayType == LineDisplayType.Area);
                var includeYAxisZeroPoint = ChartOptions?.YAxisRequireZeroPoint is true || hasAreaDisplay;
                if (includeYAxisZeroPoint)
                {
                    minY = Math.Min(minY, 0); // we want to include the 0 in the grid
                    maxY = Math.Max(maxY, 0); // we want to include the 0 in the grid
                }

                lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                var highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

                // this is a safeguard against millions of gridlines which might arise with very high values
                var maxYTicks = ChartOptions?.MaxNumYAxisTicks ?? 100;
                while (numHorizontalLines > maxYTicks)
                {
                    gridYUnits *= 2;
                    lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                    highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
                    numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
                }

                numVerticalLines = Series.Max(series => series.Data.Values.Length);
            }
            else
            {
                numHorizontalLines = 1;
                lowestHorizontalLine = 0;
                numVerticalLines = 1;
            }
        }

        protected override string GetVerticalGridLineLabel(int index)
        {
            return index < ChartLabels.Length? ChartLabels[index] : "";
        }

        protected override T GetDataValue<T>(int seriesIndex, int dataPointIndex)
        {
            return (T)Convert.ChangeType(Series[seriesIndex].Data.Values[dataPointIndex], typeof(T));
        }

        protected override string GetLabelXValue(int seriesIndex, int dataPointIndex)
        {
            return ChartLabels.Length > dataPointIndex ? ChartLabels[dataPointIndex] : string.Empty;
        }

        protected override (double x, double y) GetXYForDataPoint(int seriesIndex, int dataPointIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            var data = Series[seriesIndex].Data;
            var x = HorizontalStartSpace + (dataPointIndex * horizontalSpace);
            var gridValue = ((data[dataPointIndex] / gridYUnits) - lowestHorizontalLine) * verticalSpace;
            var y = _boundHeight - VerticalStartSpace - gridValue;
            return (x, y);
        }

        internal override ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            var series = Series[seriesIndex];
            var data = series.Data;
            var interpolationResolution = 10;

            var XValues = new double[data.Values.Length];
            var YValues = new double[data.Values.Length];

            for (var j = 0; j < data.Values.Length; j++)
            {
                var (x, y) = (XValues[j], YValues[j]) = GetXYForDataPoint(seriesIndex, j, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
            }

            var overrideSettings = GetSeriesDisplayOverride(series);
            var interpolationOption = overrideSettings?.InterpolationOption ?? ChartOptions?.InterpolationOption;

            ILineInterpolator interpolator = interpolationOption switch
            {
                InterpolationOption.NaturalSpline => new NaturalSpline(XValues, YValues, interpolationResolution),
                InterpolationOption.EndSlope => new EndSlopeSpline(XValues, YValues, interpolationResolution),
                InterpolationOption.Periodic => new PeriodicSpline(XValues, YValues, interpolationResolution),
                _ => throw new NotImplementedException("Interpolation option not implemented yet")
            };

            return interpolator;
        }

        //private void GenerateChartLines(int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        //{
        //    Legends.Clear();
        //    ChartLines.Clear();
        //    ChartAreas.Clear();
        //    ChartDataPoints.Clear();

        //    for (var i = 0; i < Series.Count; i++)
        //    {
        //        var series = Series[i];

        //        if (series.Visible)
        //        {
        //            var chartLine = new StringBuilder();
        //            var data = series.Data;
        //            var chartDataCircles = ChartDataPoints[i] = [];

        //            (double x, double y) GetXYForDataPoint(int index)
        //            {
        //                var x = HorizontalStartSpace + (index * horizontalSpace);
        //                var gridValue = ((data[index] / gridYUnits) - lowestHorizontalLine) * verticalSpace;
        //                var y = _boundHeight - VerticalStartSpace - gridValue;
        //                return (x, y);
        //            }
        //            double GetYForZeroPoint()
        //            {
        //                var gridValue = (0 / gridYUnits - lowestHorizontalLine) * verticalSpace;
        //                var y = _boundHeight - VerticalStartSpace - gridValue;

        //                return y;
        //            }

        //            var zeroPointY = GetYForZeroPoint();
        //            double firstPointX = 0;
        //            double firstPointY = 0;
        //            double lastPointX = 0;

        //            var overrideSettings = GetSeriesDisplayOverride(series);
        //            var interpolationOption = overrideSettings?.InterpolationOption ?? ChartOptions?.InterpolationOption;

        //            var interpolationEnabled = MudChartParent != null && interpolationOption != InterpolationOption.Straight;
        //            if (interpolationEnabled)
        //            {
        //                var interpolationResolution = 10;
        //                var XValues = new double[data.Values.Length];
        //                var YValues = new double[data.Values.Length];
        //                for (var j = 0; j < data.Values.Length; j++)
        //                {
        //                    var (x, y) = (XValues[j], YValues[j]) = GetXYForDataPoint(j);

        //                    var dataValue = data[j];

        //                    if (MudChartParent?.ChartOptions?.ShowToolTips != true)
        //                    {
        //                        continue;
        //                    }

        //                    chartDataCircles.Add(new()
        //                    {
        //                        Index = j,
        //                        CX = x,
        //                        CY = y,
        //                        LabelX = x,
        //                        LabelXValue = ChartLabels[j / interpolationResolution],
        //                        LabelY = y,
        //                        LabelYValue = dataValue.ToString(series.TooltipYValueFormat),
        //                    });
        //                }

        //                ILineInterpolator interpolator = interpolationOption switch
        //                {
        //                    InterpolationOption.NaturalSpline => new NaturalSpline(XValues, YValues, interpolationResolution),
        //                    InterpolationOption.EndSlope => new EndSlopeSpline(XValues, YValues, interpolationResolution),
        //                    InterpolationOption.Periodic => new PeriodicSpline(XValues, YValues, interpolationResolution),
        //                    _ => throw new NotImplementedException("Interpolation option not implemented yet")
        //                };

        //                var horizontalSpaceInterpolated = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / (interpolator.InterpolatedXs.Length - 1);

        //                for (var j = 0; j < interpolator.InterpolatedYs.Length; j++)
        //                {
        //                    var x = HorizontalStartSpace + (j * horizontalSpaceInterpolated);
        //                    var y = interpolator.InterpolatedYs[j];

        //                    if (j == 0)
        //                    {
        //                        chartLine.Append("M ");
        //                        firstPointX = x;
        //                        firstPointY = y;
        //                    }
        //                    else
        //                        chartLine.Append(" L ");

        //                    if (j == interpolator.InterpolatedYs.Length - 1)
        //                    {
        //                        lastPointX = x;
        //                    }

        //                    chartLine.Append(ToS(x));
        //                    chartLine.Append(' ');
        //                    chartLine.Append(ToS(y));
        //                }
        //            }
        //            else
        //            {
        //                for (var j = 0; j < data.Values.Length; j++)
        //                {
        //                    var (x, y) = GetXYForDataPoint(j);

        //                    if (j == 0)
        //                    {
        //                        chartLine.Append("M ");
        //                        firstPointX = x;
        //                        firstPointY = y;
        //                    }
        //                    else
        //                        chartLine.Append(" L ");

        //                    if (j == data.Values.Length - 1)
        //                    {
        //                        lastPointX = x;
        //                    }

        //                    chartLine.Append(ToS(x));
        //                    chartLine.Append(' ');
        //                    chartLine.Append(ToS(y));

        //                    var dataValue = data[j];

        //                    if (MudChartParent?.ChartOptions?.ShowToolTips == true)
        //                    {
        //                        chartDataCircles.Add(new()
        //                        {
        //                            Index = j,
        //                            CX = x,
        //                            CY = y,
        //                            LabelX = x,
        //                            LabelXValue = ChartLabels.Length > j ? ChartLabels[j] : string.Empty,
        //                            LabelY = y,
        //                            LabelYValue = dataValue.ToString(series.TooltipYValueFormat),
        //                        });
        //                    }
        //                }
        //            }
        //            var line = new SvgPath()
        //            {
        //                Index = i,
        //                Data = chartLine.ToString()
        //            };
        //            ChartLines.Add(line);

        //            var displayType = overrideSettings?.LineDisplayType ?? ChartOptions?.LineDisplayType;

        //            if (displayType == LineDisplayType.Area)
        //            {
        //                var chartArea = new StringBuilder();

        //                chartArea.Append(chartLine); // the line up to this point is the same as the area, so we can reuse it

        //                // add an extra point based on the x of the last point and 0 to add the area to the bottom

        //                chartArea.Append(" L ");
        //                chartArea.Append(ToS(lastPointX));
        //                chartArea.Append(' ');
        //                chartArea.Append(ToS(zeroPointY));

        //                // add an extra point based on the x of the first point and 0 to close the area

        //                chartArea.Append(" L ");
        //                chartArea.Append(ToS(firstPointX));
        //                chartArea.Append(' ');
        //                chartArea.Append(ToS(zeroPointY));

        //                // add an the first point again to close the area
        //                chartArea.Append(" L ");
        //                chartArea.Append(ToS(firstPointX));
        //                chartArea.Append(' ');
        //                chartArea.Append(ToS(firstPointY));

        //                var area = new SvgPath()
        //                {
        //                    Index = i,
        //                    Data = chartArea.ToString()
        //                };
        //                ChartAreas.Add(i, area);
        //            }
        //        }

        //        var legend = new SvgLegend()
        //        {
        //            Index = i,
        //            Labels = series.Label,
        //            Visible = series.Visible,
        //            OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
        //        };
        //        Legends.Add(legend);
        //    }
        //}
    }
}
