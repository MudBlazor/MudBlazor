using MudBlazor.Interpolation;

#nullable enable

namespace MudBlazor.Charts;

/// <summary>
/// A chart which displays values over time.
/// </summary>
partial class TimeSeries : MudAxisLineChartBase<TimeSeriesChartOptions>, IDisposable
{
    private DateTime _minDateTime;
    private DateTime _maxDateTime;
    private TimeSpan _minDateLabelOffset;
    private DataPoint[][]? _cachedDataPoints;

    private double _timeToPixelRatio;

    protected override bool ShouldInterpolate => false;

    protected override void OnInitialized()
    {
        ChartOptions ??= new TimeSeriesChartOptions();
        base.OnInitialized();
    }

    protected override void RebuildChart()
    {
        if (MudChartParent != null)
            Series = MudChartParent.ChartSeries;

        _cachedDataPoints = null;

        SetBounds();
        ComputeMinAndMaxDateTimes();
        ComputeUnitsAndNumberOfLines(out var gridYUnits, out var numHorizontalLines, out var lowestHorizontalLine, out var numVerticalLines);

        var horizontalSpace = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, (_maxDateTime - _minDateTime) / ChartOptions!.TimeLabelSpacing);
        var verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);
        var startOffset = 0.0;

        if (_minDateLabelOffset != TimeSpan.Zero)
        {
            startOffset = (_minDateLabelOffset.TotalMilliseconds / (_maxDateTime - _minDateTime).TotalMilliseconds) * (_boundWidth - HorizontalStartSpace - HorizontalEndSpace);
        }

        var fullDateTimeDiff = _maxDateTime - _minDateTime;
        if (fullDateTimeDiff.TotalMilliseconds > 0)
        {
            _timeToPixelRatio = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / fullDateTimeDiff.TotalMilliseconds;
        }

        GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
        GenerateVerticalGridLines(numVerticalLines, startOffset, horizontalSpace);

        if (Series.Count == 0) return;
        if ((_yAxisLabelSize?.Width ?? 0) == 0 || (_xAxisLabelSize?.Height ?? 0) == 0) return;

        GenerateChartLines(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
    }

    private void ComputeMinAndMaxDateTimes()
    {
        _minDateLabelOffset = TimeSpan.Zero;

        DateTime? minDate = null;
        DateTime? maxDate = null;

        foreach (var series in Series)
        {
            if (!series.Visible || series.Data.Points == null)
                continue;

            foreach (var point in series.Data.Points)
            {
                if (point.X is DateTime dateTime)
                {
                    minDate = minDate == null || dateTime < minDate ? dateTime : minDate;
                    maxDate = maxDate == null || dateTime > maxDate ? dateTime : maxDate;
                }
            }
        }

        var labelSpacing = ChartOptions!.TimeLabelSpacing;

        if (minDate == null || maxDate == null)
        {
            _minDateTime = DateTime.Now;
            _maxDateTime = labelSpacing.Days > 0 ? DateTime.Now.AddDays(1) :
                           labelSpacing.Minutes > 0 ? DateTime.Now.AddHours(1) :
                           DateTime.Now.AddMinutes(1);
            return;
        }

        _minDateTime = minDate.Value;
        _maxDateTime = maxDate.Value;

        if (!ChartOptions!.TimeLabelSpacingRounding) return;

        if (_minDateTime.Ticks % labelSpacing.Ticks != 0)
        {
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
            var offset = labelSpacing - new TimeSpan(_maxDateTime.Ticks % labelSpacing.Ticks);

            _maxDateTime = _maxDateTime.Add(offset);
        }
    }

    private void ComputeUnitsAndNumberOfLines(out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
    {
        gridYUnits = ChartOptions?.YAxisTicks ?? 20;
        if (gridYUnits <= 0)
            gridYUnits = 20;

        if (Series.SelectMany(series => series.Data.Points).Any())
        {
            var minY = double.MaxValue;
            var maxY = double.MinValue;

            foreach (var series in Series.Where(s => s.Visible))
            {
                foreach (var point in series.Data.Points)
                {
                    minY = Math.Min(minY, point.Y);
                    maxY = Math.Max(maxY, point.Y);
                }
            }

            if (minY == double.MaxValue)
            {
                minY = 0;
                maxY = 0;
            }

            var hasAreaDisplay = ChartOptions?.LineDisplayType == LineDisplayType.Area || Series.Any(series => GetSeriesDisplayOverride(series)?.LineDisplayType == LineDisplayType.Area);
            var includeYAxisZeroPoint = ChartOptions?.YAxisRequireZeroPoint is true || hasAreaDisplay;
            if (includeYAxisZeroPoint)
            {
                minY = Math.Min(minY, 0);
                maxY = Math.Max(maxY, 0);
            }

            maxY = ChartOptions?.YAxisSuggestedMax is null ? maxY : Math.Max(ChartOptions.YAxisSuggestedMax.Value, maxY);

            lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
            var highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
            numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

            var maxYTicks = ChartOptions?.MaxNumYAxisTicks ?? 100;
            while (numHorizontalLines > maxYTicks)
            {
                gridYUnits *= 2;
                lowestHorizontalLine = (int)Math.Floor(minY / gridYUnits);
                highestHorizontalLine = (int)Math.Ceiling(maxY / gridYUnits);
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
            }

            var labelSpacing = ChartOptions!.TimeLabelSpacing;
            numVerticalLines = (int)Math.Ceiling((_maxDateTime - _minDateTime) / labelSpacing) + 1;
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

    private DataPoint[][] GetCachedDataPoints()
    {
        if (_cachedDataPoints != null)
            return _cachedDataPoints;

        _cachedDataPoints = new DataPoint[Series.Count][];

        for (var i = 0; i < Series.Count; i++)
        {
            var series = Series[i];
            var points = series.Data.Points;
            var data = new DataPoint[points.Count];

            for (var j = 0; j < points.Count; j++)
            {
                var point = points[j];
                data[j] = new DataPoint(
                    DateTime.TryParse(point.X?.ToString(), out var date) ? date : DateTime.MinValue,
                    point.Y
                );
            }

            _cachedDataPoints[i] = data;
        }

        return _cachedDataPoints;
    }

    protected override T GetDataValue<T>(int seriesIndex, int dataPointIndex)
    {
        var data = GetCachedDataPoints()[seriesIndex];
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
        return dataValue.DateTime.ToString(ChartOptions?.TooltipTimeLabelFormat ?? "G");
    }

    protected override (double x, double y) GetXYForDataPoint(int seriesIndex, int dataPointIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
    {
        var dataPoint = GetCachedDataPoints()[seriesIndex][dataPointIndex];

        var gridValue = (dataPoint.Value / gridYUnits - lowestHorizontalLine) * verticalSpace;
        var y = _boundHeight - VerticalStartSpace - gridValue;

        var diffFromMin = dataPoint.DateTime - _minDateTime;
        var x = HorizontalStartSpace + (diffFromMin.TotalMilliseconds * _timeToPixelRatio);

        return (x, y);
    }

    internal override ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, double gridYUnits, double horizontalSpace, double verticalSpace)
    {
        throw new NotImplementedException("Interpolation not implemented yet for timeseries charts");
    }

    public readonly record struct DataPoint(DateTime DateTime, double Value);
}
