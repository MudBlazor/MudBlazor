using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Components.Chart;
using MudBlazor.Interpolation;

#nullable enable
#pragma warning disable CS0618

namespace MudBlazor.Charts
{
    /// <summary>
    /// A chart which displays values over time.
    /// </summary>
    partial class TimeSeries : MudAxisLineChartBase<TimeSeriesChartOptions>, IDisposable
    {
        [Inject]
        private IJSRuntime JsRuntime { get; set; } = null!;

        private DateTime _minDateTime;
        private DateTime _maxDateTime;
        private TimeSpan _minDateLabelOffset;

        protected override bool ShouldInterpolate => false;

        protected override void RebuildChart()
        {
            if (MudChartParent != null)
                Series = MudChartParent.ChartSeries;

            SetBounds();
            ComputeMinAndMaxDateTimes();
            ComputeUnitsAndNumberOfLines(out var gridYUnits, out var numHorizontalLines, out var lowestHorizontalLine, out var numVerticalLines);

            var horizontalSpace = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, (_maxDateTime - _minDateTime) / ChartOptions!.TimeLabelSpacing);
            var verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);
            var startOffset = 0.0;

            if (_minDateLabelOffset != TimeSpan.Zero)
            {
                // offset the first label to be _minDateLabelOffset away from the minDateTime
                startOffset = (_minDateLabelOffset.TotalMilliseconds / (_maxDateTime - _minDateTime).TotalMilliseconds) * (_boundWidth - HorizontalStartSpace - HorizontalEndSpace);
            }

            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, startOffset, horizontalSpace);
            GenerateChartLines(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
        }

        private void ComputeMinAndMaxDateTimes()
        {
            _minDateLabelOffset = TimeSpan.Zero;

            if (Series.SelectMany(series => series.Data.Points).All(x => x.X is DateTime))
            {
                _minDateTime = Series.SelectMany(series => series.Data.Points).Min(point => (DateTime)point.X);
                _maxDateTime = Series.SelectMany(series => series.Data.Points).Max(point => (DateTime)point.X);
                var labelSpacing = ChartOptions!.TimeLabelSpacing;

                if (!ChartOptions!.TimeLabelSpacingRounding) return;

                if (_minDateTime.Ticks % labelSpacing.Ticks != 0)
                {
                    // subtract the remainder of the ticks from the minDateTime to get the first tick before or equal to the minDateTime, if the first label is over half the labelSpacing away from the first timestamp, offset the label instead.
                    var offset = new TimeSpan(_minDateTime.Ticks % labelSpacing.Ticks);

                    if (ChartOptions!.TimeLabelSpacingRoundingPadSeries)
                    {
                        _minDateTime = _minDateTime.Subtract(offset);
                    }
                    else
                        _minDateLabelOffset = labelSpacing - offset;
                }

                if (ChartOptions!.TimeLabelSpacingRoundingPadSeries && _maxDateTime.Ticks % labelSpacing.Ticks != 0)
                {
                    // add the remainder of the ticks to the maxDateTime to get the first tick after or equal to the maxDateTime
                    var offset = labelSpacing - new TimeSpan(_maxDateTime.Ticks % labelSpacing.Ticks);

                    _maxDateTime = _maxDateTime.Add(offset);
                }
            }
        }

        private void ComputeUnitsAndNumberOfLines(out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            gridYUnits = ChartOptions?.YAxisTicks ?? 20;
            if (gridYUnits <= 0)
                gridYUnits = 20;

            if (Series.SelectMany(series => series.Data.Points).Any())
            {
                var minY = Series.Where(series => series.Visible).SelectMany(series => series.Data.Points).Min(point => point.Y);
                var maxY = Series.Where(series => series.Visible).SelectMany(series => series.Data.Points).Max(point => point.Y);

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

                var labelSpacing = ChartOptions!.TimeLabelSpacing;

                numVerticalLines = (int)((_maxDateTime - _minDateTime) / labelSpacing) + 1;
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
            var minDateTimeWithOffset = _minDateTime.Add(_minDateLabelOffset);

            return minDateTimeWithOffset.Add(ChartOptions!.TimeLabelSpacing * index).ToString(ChartOptions!.TimeLabelFormat);
        }

        protected override T GetDataValue<T>(int seriesIndex, int dataPointIndex)
        {
            var series = Series[seriesIndex];
            var data = series.Data.Points.Select(point => new DataPoint(DateTime.TryParse(point.X?.ToString(), out var date) ? date : DateTime.MinValue, point.Y)).ToArray();

            return (T)(object)data[dataPointIndex];
        }

        protected override string GetDataValueAsString(int seriesIndex, int dataPointIndex)
        {
            var dataValue = GetDataValue<DataPoint>(seriesIndex, dataPointIndex);
            return dataValue.Value.ToString(Series[seriesIndex].TooltipYValueFormat);
        }

        protected override string GetLabelXValue(int seriesIndex, int dataPointIndex)
        {
            var dataValue = GetDataValue<DataPoint>(seriesIndex, dataPointIndex);
            return dataValue.DateTime.ToString(ChartOptions?.TooltipTimeLabelFormat ?? "{0}");
        }

        protected override (double x, double y) GetXYForDataPoint(int seriesIndex, int dataPointIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            var series = Series[seriesIndex];
            var data = series.Data.Points.Select(point => new {
                DateTime = DateTime.TryParse(point.X?.ToString(), out var date) ? date : DateTime.MinValue,
                Value = point.Y
            }).ToArray();

            var dateTime = data[dataPointIndex].DateTime;
            var diffFromMin = dateTime - _minDateTime;

            var gridValue = (data[dataPointIndex].Value / gridYUnits - lowestHorizontalLine) * verticalSpace;
            var y = _boundHeight - VerticalStartSpace - gridValue;

            var fullDateTimeDiff = _maxDateTime - _minDateTime;

            if (fullDateTimeDiff.TotalMilliseconds == 0)
                return (HorizontalStartSpace, y);

            var x = HorizontalStartSpace + (diffFromMin.TotalMilliseconds / fullDateTimeDiff.TotalMilliseconds *
                    (_boundWidth - HorizontalStartSpace - HorizontalEndSpace));

            return (x, y);
        }

        //private void GenerateChartLines(int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        //{
        //    Legends.Clear();
        //    ChartLines.Clear();
        //    ChartAreas.Clear();
        //    ChartDataPoints.Clear();

        //    if (Series.Count == 0)
        //        return;

        //    var fullDateTimeDiff = _maxDateTime - _minDateTime;

        //    for (var i = 0; i < Series.Count; i++)
        //    {
        //        var series = Series[i];

        //        if (series.Visible)
        //        {
        //            var chartLine = new StringBuilder();
        //            var data = series.Data.Points.Select(point => new { DateTime = DateTime.TryParse(point.X?.ToString(), out var date) ? date : DateTime.MinValue, Value = point.Y }).ToArray();
        //            var chartDataCirlces = ChartDataPoints[i] = [];

        //            if (data.Length == 0) continue;

        //            (double x, double y) GetXYForDataPoint(int index)
        //            {
        //                var dateTime = data[index].DateTime;

        //                var diffFromMin = dateTime - _minDateTime;

        //                var gridValue = (data[index].Value / gridYUnits - lowestHorizontalLine) * verticalSpace;
        //                var y = _boundHeight - VerticalStartSpace - gridValue;

        //                if (fullDateTimeDiff.TotalMilliseconds == 0)
        //                    return (HorizontalStartSpace, y);

        //                var x = HorizontalStartSpace + (diffFromMin.TotalMilliseconds / fullDateTimeDiff.TotalMilliseconds * (_boundWidth - HorizontalStartSpace - HorizontalEndSpace));

        //                return (x, y);
        //            }
        //            double GetYForZeroPoint()
        //            {
        //                var gridValue = (0 / gridYUnits - lowestHorizontalLine) * verticalSpace;
        //                var y = _boundHeight - VerticalStartSpace - gridValue;

        //                return y;
        //            }

        //            var overrideSettings = GetSeriesDisplayOverride(series);
        //            var interpolationOption = overrideSettings?.InterpolationOption ?? ChartOptions?.InterpolationOption;

        //            var interpolationEnabled = MudChartParent != null && interpolationOption != InterpolationOption.Straight;
        //            if (interpolationEnabled)
        //            {
        //                // TODO this is not simple to implement, as the x values are not linearly spaced
        //                // and the interpolation should be done based on the datetime
        //                // so we need to find a way to interpolate the x values based on the datetime
        //                // and then interpolate the y values based on the x values
        //                // this is not trivial and needs to be done in a separate PR

        //                throw new NotImplementedException("Interpolation not implemented yet for timeseries charts");
        //            }
        //            else
        //            {
        //                for (var j = 0; j < data.Length; j++)
        //                {
        //                    var (x, y) = GetXYForDataPoint(j);

        //                    if (j == 0)
        //                    {
        //                        chartLine.Append("M ");
        //                    }
        //                    else
        //                        chartLine.Append(" L ");

        //                    chartLine.Append(ToS(x));
        //                    chartLine.Append(' ');
        //                    chartLine.Append(ToS(y));

        //                    var dataValue = data[j];

        //                    if (ChartOptions?.ShowToolTips is not true)
        //                        continue;

        //                    chartDataCirlces.Add(new()
        //                    {
        //                        Index = j,
        //                        CX = x,
        //                        CY = y,
        //                        LabelX = x,
        //                        LabelXValue = dataValue.DateTime.ToString(ChartOptions?.TooltipTimeLabelFormat ?? "{0}"),
        //                        LabelY = y,
        //                        LabelYValue = dataValue.Value.ToString(series.TooltipYValueFormat),
        //                    });
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

        //                var zeroPointY = GetYForZeroPoint();
        //                var (firstPointX, firstPointY) = GetXYForDataPoint(0);
        //                var (lastPointX, _) = GetXYForDataPoint(data.Length - 1);

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

        internal override ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            throw new NotImplementedException("Interpolation not implemented yet for timeseries charts");
        }

        public record DataPoint(DateTime DateTime, double Value);
    }
}
