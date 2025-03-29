using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
    partial class Line : MudCategoryAxisChartBase
    {
        private readonly List<SvgPath> _horizontalLines = [];
        private readonly List<SvgText> _horizontalValues = [];

        private readonly List<SvgPath> _verticalLines = [];
        private readonly List<SvgText> _verticalValues = [];

        private readonly List<SvgLegend> _legends = [];
        private List<ChartSeries> _series = [];

        private readonly List<SvgPath> _chartLines = [];
        private readonly Dictionary<int, SvgPath> _chartAreas = [];
        private readonly Dictionary<int, List<SvgCircle>> _chartDataPoints = [];
        private SvgCircle? _hoveredDataPoint;
        private SvgPath? _hoverDataPointChartLine;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            RebuildChart();
        }

        protected override void RebuildChart()
        {
            if (MudChartParent != null)
                _series = MudChartParent.ChartSeries;

            SetBounds();
            ComputeUnitsAndNumberOfLines(out var gridXUnits, out var gridYUnits, out var numHorizontalLines, out var lowestHorizontalLine, out var numVerticalLines);

            var horizontalSpace = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, numVerticalLines - 1);
            var verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);

            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, gridXUnits, horizontalSpace);
            GenerateChartLines(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
        }

        private void ComputeUnitsAndNumberOfLines(out double gridXUnits, out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            gridXUnits = 30;

            gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;

            if (_series.SelectMany(series => series.Data).Any())
            {
                var minY = _series.SelectMany(series => series.Data).Min();
                var maxY = _series.SelectMany(series => series.Data).Max();

                var includeYAxisZeroPoint = MudChartParent?.ChartOptions.YAxisRequireZeroPoint ?? false;
                if (includeYAxisZeroPoint)
                {
                    minY = Math.Min(minY, 0); // we want to include the 0 in the grid
                    maxY = Math.Max(maxY, 0); // we want to include the 0 in the grid
                }

                lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                var highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

                // this is a safeguard against millions of gridlines which might arise with very high values
                var maxYTicks = MudChartParent?.ChartOptions.MaxNumYAxisTicks ?? 100;
                while (numHorizontalLines > maxYTicks)
                {
                    gridYUnits *= 2;
                    lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                    highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
                    numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
                }

                numVerticalLines = _series.Max(series => series.Data.Length);
            }
            else
            {
                numHorizontalLines = 1;
                lowestHorizontalLine = 0;
                numVerticalLines = 1;
            }
        }

        private void GenerateHorizontalGridLines(int numHorizontalLines, int lowestHorizontalLine, double gridYUnits, double verticalSpace)
        {
            _horizontalLines.Clear();
            _horizontalValues.Clear();

            for (var i = 0; i < numHorizontalLines; i++)
            {
                var y = VerticalStartSpace + (i * verticalSpace);
                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(HorizontalStartSpace)} {ToS(_boundHeight - y)} L {ToS(_boundWidth - HorizontalEndSpace)} {ToS(_boundHeight - y)}"
                };
                _horizontalLines.Add(line);

                var startGridY = (lowestHorizontalLine + i) * gridYUnits;
                var lineValue = new SvgText()
                {
                    X = HorizontalStartSpace - 10,
                    Y = _boundHeight - y + 5,
                    Value = ToS(startGridY, MudChartParent?.ChartOptions.YAxisFormat)
                };
                _horizontalValues.Add(lineValue);
            }
        }

        private void GenerateVerticalGridLines(int numVerticalLines, double gridXUnits, double horizontalSpace)
        {
            _verticalLines.Clear();
            _verticalValues.Clear();

            for (var i = 0; i < numVerticalLines; i++)
            {
                var x = HorizontalStartSpace + (i * horizontalSpace);
                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
                };
                _verticalLines.Add(line);

                var xLabels = i < XAxisLabels.Length ? XAxisLabels[i] : "";
                var lineValue = new SvgText()
                {
                    X = x,
                    Y = _boundHeight - 10,
                    Value = xLabels
                };
                _verticalValues.Add(lineValue);
            }
        }

        private void GenerateChartLines(int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            _legends.Clear();
            _chartLines.Clear();
            _chartAreas.Clear();
            _chartDataPoints.Clear();

            for (var i = 0; i < _series.Count; i++)
            {
                var series = _series[i];

                if (series.Visible)
                {
                    var chartLine = new StringBuilder();
                    var data = series.Data;
                    var chartDataCircles = _chartDataPoints[i] = [];

                    (double x, double y) GetXYForDataPoint(int index)
                    {
                        var x = HorizontalStartSpace + (index * horizontalSpace);
                        var gridValue = ((data[index] / gridYUnits) - lowestHorizontalLine) * verticalSpace;
                        var y = _boundHeight - VerticalStartSpace - gridValue;
                        return (x, y);
                    }
                    double GetYForZeroPoint()
                    {
                        var gridValue = (0 / gridYUnits - lowestHorizontalLine) * verticalSpace;
                        var y = _boundHeight - VerticalStartSpace - gridValue;

                        return y;
                    }

                    var zeroPointY = GetYForZeroPoint();
                    double firstPointX = 0;
                    double firstPointY = 0;
                    double lastPointX = 0;

                    var interpolationEnabled = MudChartParent != null && MudChartParent.ChartOptions.InterpolationOption != InterpolationOption.Straight;
                    if (interpolationEnabled)
                    {
                        var interpolationResolution = 10;
                        var XValues = new double[data.Length];
                        var YValues = new double[data.Length];
                        for (var j = 0; j < data.Length; j++)
                        {
                            var (x, y) = (XValues[j], YValues[j]) = GetXYForDataPoint(j);

                            var dataValue = data[j];

                            if (MudChartParent?.ChartOptions.ShowToolTips != true)
                            {
                                continue;
                            }

                            chartDataCircles.Add(new()
                            {
                                Index = j,
                                CX = x,
                                CY = y,
                                LabelX = x,
                                LabelXValue = XAxisLabels[j / interpolationResolution],
                                LabelY = y,
                                LabelYValue = dataValue.ToString(series.DataMarkerTooltipYValueFormat),
                            });
                        }

                        ILineInterpolator interpolator = MudChartParent?.ChartOptions.InterpolationOption switch
                        {
                            InterpolationOption.NaturalSpline => new NaturalSpline(XValues, YValues, interpolationResolution),
                            InterpolationOption.EndSlope => new EndSlopeSpline(XValues, YValues, interpolationResolution),
                            InterpolationOption.Periodic => new PeriodicSpline(XValues, YValues, interpolationResolution),
                            _ => throw new NotImplementedException("Interpolation option not implemented yet")
                        };

                        var horizontalSpaceInterpolated = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / (interpolator.InterpolatedXs.Length - 1);

                        for (var j = 0; j < interpolator.InterpolatedYs.Length; j++)
                        {
                            var x = HorizontalStartSpace + (j * horizontalSpaceInterpolated);
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
                        }
                    }
                    else
                    {
                        for (var j = 0; j < data.Length; j++)
                        {
                            var (x, y) = GetXYForDataPoint(j);

                            if (j == 0)
                            {
                                chartLine.Append("M ");
                                firstPointX = x;
                                firstPointY = y;
                            }
                            else
                                chartLine.Append(" L ");

                            if (j == data.Length - 1)
                            {
                                lastPointX = x;
                            }

                            chartLine.Append(ToS(x));
                            chartLine.Append(' ');
                            chartLine.Append(ToS(y));

                            var dataValue = data[j];

                            if (MudChartParent?.ChartOptions.ShowToolTips == true)
                            {
                                chartDataCircles.Add(new()
                                {
                                    Index = j,
                                    CX = x,
                                    CY = y,
                                    LabelX = x,
                                    LabelXValue = XAxisLabels.Length > j ? XAxisLabels[j] : string.Empty,
                                    LabelY = y,
                                    LabelYValue = dataValue.ToString(series.DataMarkerTooltipYValueFormat),
                                });
                            }
                        }
                    }
                    var line = new SvgPath()
                    {
                        Index = i,
                        Data = chartLine.ToString()
                    };
                    _chartLines.Add(line);

                    if (series.LineDisplayType == LineDisplayType.Area)
                    {
                        var chartArea = new StringBuilder();

                        chartArea.Append(chartLine.ToString()); // the line up to this point is the same as the area, so we can reuse it

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

                        var area = new SvgPath()
                        {
                            Index = i,
                            Data = chartArea.ToString()
                        };
                        _chartAreas.Add(i, area);
                    }
                }

                var legend = new SvgLegend()
                {
                    Index = i,
                    Labels = series.Name,
                    Visible = series.Visible,
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                };
                _legends.Add(legend);
            }
        }

        private void HandleLegendVisibilityChanged(SvgLegend legend)
        {
            var series = _series[legend.Index];
            series.Visible = legend.Visible;
            RebuildChart();
        }

        private void OnDataPointMouseOver(MouseEventArgs _, SvgCircle dataPoint, SvgPath seriesPath)
        {
            _hoveredDataPoint = dataPoint;
            _hoverDataPointChartLine = seriesPath;
        }

        private void OnDataPointMouseOut(MouseEventArgs _)
        {
            _hoveredDataPoint = null;
            _hoverDataPointChartLine = null;
        }
    }
}
