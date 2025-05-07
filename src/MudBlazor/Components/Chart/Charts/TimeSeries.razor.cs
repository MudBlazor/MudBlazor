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
        internal override ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
        {
            throw new NotImplementedException("Interpolation not implemented yet for timeseries charts");
        }

        public record DataPoint(DateTime DateTime, double Value);
    }
}
