using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Interop;

#nullable enable
#pragma warning disable CS0618

namespace MudBlazor.Charts
{
    /// <summary>
    /// A chart which displays values over time.
    /// </summary>
    partial class TimeSeries : MudTimeSeriesChartBase, IDisposable
    {
        private const double Epsilon = 1e-6;
        private const double BoundWidthDefault = 800;
        private const double BoundHeightDefault = 350;
        private const double HorizontalStartSpaceBuffer = 10.0;
        protected double HorizontalStartSpace => Math.Max(HorizontalStartSpaceBuffer + (_yAxisLabelSize?.Width ?? 0), 30);
        private const double HorizontalEndSpace = 30.0;
        private const double VerticalStartSpaceBuffer = 10.0;
        protected double VerticalStartSpace => Math.Max(VerticalStartSpaceBuffer + (_xAxisLabelSize?.Height ?? 0), 30);
        private const double VerticalEndSpace = 25.0;
        protected double XAxisLabelOffset => Math.Ceiling(_xAxisLabelSize?.Height ?? 20) / 2;

        private double _boundWidth = BoundWidthDefault;
        private double _boundHeight = BoundHeightDefault;
        private ElementSize? _elementSize = null;
        private ElementSize? _yAxisLabelSize;
        private ElementSize? _xAxisLabelSize;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        [CascadingParameter]
        public MudTimeSeriesChartBase? MudChartParent { get; set; }

        private readonly List<SvgPath> _horizontalLines = [];
        private readonly List<SvgText> _horizontalValues = [];

        private readonly List<SvgPath> _verticalLines = [];
        private readonly List<SvgText> _verticalValues = [];

        private readonly List<SvgLegend> _legends = [];
        private List<TimeSeriesChartSeries> _series = [];

        private readonly List<SvgPath> _chartLines = [];
        private readonly Dictionary<int, SvgPath> _chartAreas = [];
        private readonly Dictionary<int, List<SvgCircle>> _chartDataPoints = [];
        private SvgCircle? _hoveredDataPoint;
        private SvgPath? _hoverDataPointChartLine;

        private DateTime _minDateTime;
        private DateTime _maxDateTime;
        private TimeSpan _minDateLabelOffset;
        private readonly DotNetObjectReference<TimeSeries> _dotNetObjectReference;
        private ElementReference _elementReference;
        protected ElementReference? _xAxisGroupElementReference;
        protected ElementReference? _yAxisGroupElementReference;

        public TimeSeries()
        {
            _dotNetObjectReference = DotNetObjectReference.Create(this);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            RebuildChart();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var elementSize = await JsRuntime.InvokeAsync<ElementSize>("mudObserveElementSize", _dotNetObjectReference, _elementReference);

                OnElementSizeChanged(elementSize);
            }

            var yAxisLabelSize = _yAxisGroupElementReference != null ? await JsRuntime.InvokeAsync<ElementSize>("mudGetSvgBBox", _yAxisGroupElementReference) : null;
            var xAxisLabelSize = _xAxisGroupElementReference != null ? await JsRuntime.InvokeAsync<ElementSize>("mudGetSvgBBox", _xAxisGroupElementReference) : null;

            var axisChanged = false;
            if (yAxisLabelSize != null && (_yAxisLabelSize == null || !DoubleEpsilonEqualityComparer.Default.Equals(yAxisLabelSize.Height, _yAxisLabelSize.Height)))
            {
                _yAxisLabelSize = yAxisLabelSize;
                axisChanged = true;
            }

            if (xAxisLabelSize != null && (_xAxisLabelSize == null || !DoubleEpsilonEqualityComparer.Default.Equals(xAxisLabelSize.Width, _xAxisLabelSize.Width)))
            {
                _xAxisLabelSize = xAxisLabelSize;
                axisChanged = true;
            }

            // maybe there should be some kind of cancellation token here to prevent multiple rebuilds when the invokeasync takes time in server mode and subsequent renders have started to take place
            if (axisChanged)
            {
                RebuildChart();
                StateHasChanged();
            }
        }

        private void RebuildChart()
        {
            if (MudChartParent != null)
                _series = MudChartParent.ChartSeries;

            SetBounds();
            ComputeMinAndMaxDateTimes();
            ComputeUnitsAndNumberOfLines(out var gridXUnits, out var gridYUnits, out var numHorizontalLines, out var lowestHorizontalLine, out var numVerticalLines);

            var horizontalSpace = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, (_maxDateTime - _minDateTime) / TimeLabelSpacing);
            var verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);

            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, gridXUnits, horizontalSpace);
            GenerateChartLines(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
        }

        private void SetBounds()
        {
            _boundWidth = BoundWidthDefault;
            _boundHeight = BoundHeightDefault;

            if (MudChartParent != null && (MudChartParent.AxisChartOptions.MatchBoundsToSize || MudChartParent.MatchBoundsToSize)) // backwards compatibilitly to the mudchartparent approach
            {
                if (_elementSize != null)
                {
                    _boundWidth = _elementSize.Width;
                    _boundHeight = _elementSize.Height;
                }
                else if (MudChartParent.Width.EndsWith("px")
                    && MudChartParent.Height.EndsWith("px")
                    && double.TryParse(MudChartParent.Width.AsSpan(0, MudChartParent.Width.Length - 2), out var width)
                    && double.TryParse(MudChartParent.Height.AsSpan(0, MudChartParent.Height.Length - 2), out var height))
                {
                    _boundWidth = width;
                    _boundHeight = height;
                }
            }
        }

        [JSInvokable]
        public void OnElementSizeChanged(ElementSize? elementSize)
        {
            if (elementSize == null || elementSize.Timestamp <= _elementSize?.Timestamp)
                return;

            _elementSize = elementSize;


            if (!AxisChartOptions.MatchBoundsToSize)
                return;

            if (Math.Abs(_boundWidth - _elementSize.Width) < Epsilon &&
                Math.Abs(_boundHeight - _elementSize.Height) < Epsilon)
            {
                return;
            }

            RebuildChart();

            StateHasChanged();
        }

        private void ComputeMinAndMaxDateTimes()
        {
            _minDateLabelOffset = TimeSpan.Zero;

            if (_series.SelectMany(series => series.Data).Any())
            {
                _minDateTime = _series.SelectMany(series => series.Data).Min(x => x.DateTime);
                _maxDateTime = _series.SelectMany(series => series.Data).Max(x => x.DateTime);
                var labelSpacing = TimeLabelSpacing;

                if (!TimeLabelSpacingRounding) return;

                if (_minDateTime.Ticks % labelSpacing.Ticks != 0)
                {
                    // subtract the remainder of the ticks from the minDateTime to get the first tick before or equal to the minDateTime, if the first label is over half the labelSpacing away from the first timestamp, offset the label instead.
                    var offset = new TimeSpan(_minDateTime.Ticks % labelSpacing.Ticks);

                    if (TimeLabelSpacingRoundingPadSeries)
                    {
                        _minDateTime = _minDateTime.Subtract(offset);
                    }
                    else
                        _minDateLabelOffset = labelSpacing - offset;
                }

                if (TimeLabelSpacingRoundingPadSeries && _maxDateTime.Ticks % labelSpacing.Ticks != 0)
                {
                    // add the remainder of the ticks to the maxDateTime to get the first tick after or equal to the maxDateTime
                    var offset = labelSpacing - new TimeSpan(_maxDateTime.Ticks % labelSpacing.Ticks);

                    _maxDateTime = _maxDateTime.Add(offset);
                }
            }
        }

        private void ComputeUnitsAndNumberOfLines(out double gridXUnits, out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            gridXUnits = 30;

            gridYUnits = MudChartParent?.ChartOptions.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;

            if (_series.SelectMany(series => series.Data).Any())
            {
                var minY = _series.SelectMany(series => series.Data).Min(x => x.Value);
                var maxY = _series.SelectMany(series => series.Data).Max(x => x.Value);

                var includeYAxisZeroPoint = MudChartParent?.ChartOptions.YAxisRequireZeroPoint ?? _series.Any(x => x.LineDisplayType == LineDisplayType.Area);
                if (includeYAxisZeroPoint)
                {
                    minY = Math.Min(minY, 0); // we want to include the 0 in the grid
                    maxY = Math.Max(maxY, 0); // we want to include the 0 in the grid
                }

                lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                var highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

                // this is a safeguard against millions of gridlines which might arise with very high values
                int maxYTicks = MudChartParent?.ChartOptions.MaxNumYAxisTicks ?? 100;
                while (numHorizontalLines > maxYTicks)
                {
                    gridYUnits *= 2;
                    lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                    highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
                    numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
                }

                var labelSpacing = TimeLabelSpacing;

                numVerticalLines = (int)((_maxDateTime - _minDateTime) / labelSpacing) + 1;
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
                var y = VerticalStartSpace + i * verticalSpace;
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

            if (numVerticalLines == 0 || !_series.Any(x => x.Data.Any()))
                return;

            double startOffset = 0;

            var minDateTimeWithOffset = _minDateTime.Add(_minDateLabelOffset);

            if (_minDateLabelOffset != TimeSpan.Zero)
            {
                // offset the first label to be _minDateLabelOffset away from the minDateTime

                startOffset = (_minDateLabelOffset.TotalMilliseconds / (_maxDateTime - _minDateTime).TotalMilliseconds) * (_boundWidth - HorizontalStartSpace - HorizontalEndSpace);
            }

            for (var i = 0; i < numVerticalLines; i++)
            {
                var x = startOffset + HorizontalStartSpace + i * horizontalSpace;

                if (x > _boundWidth - HorizontalEndSpace)
                    break; // we are out of bounds

                var line = new SvgPath()
                {
                    Index = i,
                    Data = $"M {ToS(x)} {ToS(_boundHeight - VerticalStartSpace)} L {ToS(x)} {ToS(VerticalEndSpace)}"
                };
                _verticalLines.Add(line);

                var xLabels = minDateTimeWithOffset.Add(TimeLabelSpacing * i);

                var lineValue = new SvgText()
                {
                    X = x,
                    Y = _boundHeight - XAxisLabelOffset,
                    Value = xLabels.ToString(TimeLabelFormat),
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

            if (_series.Count == 0)
                return;

            var fullDateTimeDiff = _maxDateTime - _minDateTime;

            for (var i = 0; i < _series.Count; i++)
            {
                var series = _series[i];

                if (series.IsVisible)
                {
                    var chartLine = new StringBuilder();
                    var data = series.Data;
                    var chartDataCirlces = _chartDataPoints[i] = [];

                    if (data.Count <= 0)
                        continue;

                    (double x, double y) GetXYForDataPoint(int index)
                    {
                        var dateTime = data[index].DateTime;

                        var diffFromMin = dateTime - _minDateTime;

                        var gridValue = (data[index].Value / gridYUnits - lowestHorizontalLine) * verticalSpace;
                        var y = _boundHeight - VerticalStartSpace - gridValue;

                        if (fullDateTimeDiff.TotalMilliseconds == 0)
                            return (HorizontalStartSpace, y);

                        var x = HorizontalStartSpace + (diffFromMin.TotalMilliseconds / fullDateTimeDiff.TotalMilliseconds * (_boundWidth - HorizontalStartSpace - HorizontalEndSpace));

                        return (x, y);
                    }
                    double GetYForZeroPoint()
                    {
                        var gridValue = (0 / gridYUnits - lowestHorizontalLine) * verticalSpace;
                        var y = _boundHeight - VerticalStartSpace - gridValue;

                        return y;
                    }

                    bool interpolationEnabled = MudChartParent != null && MudChartParent.ChartOptions.InterpolationOption != InterpolationOption.Straight;
                    if (interpolationEnabled)
                    {
                        // TODO this is not simple to implement, as the x values are not linearly spaced
                        // and the interpolation should be done based on the datetime
                        // so we need to find a way to interpolate the x values based on the datetime
                        // and then interpolate the y values based on the x values
                        // this is not trivial and needs to be done in a separate PR

                        throw new NotImplementedException("Interpolation not implemented yet for timeseries charts");
                    }
                    else
                    {
                        for (var j = 0; j < data.Count; j++)
                        {
                            var (x, y) = GetXYForDataPoint(j);

                            if (j == 0)
                            {
                                chartLine.Append("M ");
                            }
                            else
                                chartLine.Append(" L ");

                            chartLine.Append(ToS(x));
                            chartLine.Append(' ');
                            chartLine.Append(ToS(y));

                            var dataValue = data[j];

                            if (MudChartParent?.ChartOptions.ShowToolTips != true)
                                continue;

                            chartDataCirlces.Add(new()
                            {
                                Index = j,
                                CX = x,
                                CY = y,
                                LabelX = x,
                                LabelXValue = dataValue.DateTime.ToString(MudChartParent?.DataMarkerTooltipTimeLabelFormat ?? "{0}"),
                                LabelY = y,
                                LabelYValue = dataValue.Value.ToString(series.DataMarkerTooltipYValueFormat),
                            });
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

                        var zeroPointY = GetYForZeroPoint();
                        var (firstPointX, firstPointY) = GetXYForDataPoint(0);
                        var (lastPointX, _) = GetXYForDataPoint(data.Count - 1);

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
                    Visible = series.IsVisible,
                    OnVisibilityChanged = EventCallback.Factory.Create<SvgLegend>(this, HandleLegendVisibilityChanged)
                };
                _legends.Add(legend);
            }
        }

        private void HandleLegendVisibilityChanged(SvgLegend legend)
        {
            var series = _series[legend.Index];
            series.IsVisible = legend.Visible;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _dotNetObjectReference.Dispose();
        }
    }
}
