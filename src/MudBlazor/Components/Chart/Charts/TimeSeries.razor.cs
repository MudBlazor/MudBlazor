using System.Numerics;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interpolation;

#nullable enable

namespace MudBlazor.Charts;

/// <summary>
/// A chart which displays values over time.
/// </summary>
partial class TimeSeries<T> : MudAxisLineChartBase<T, TimeSeriesChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    public static new ChartType ChartType => ChartType.Timeseries;

    public override RenderFragment? OverlayContent { get; set; }

    private DateTime _minDateTime;
    private DateTime _maxDateTime;
    private TimeSpan _minDateLabelOffset;
    private TimeValue<T>[][]? _cachedDataPoints;

    private bool _generateChartLines;
    private double _timeToPixelRatio;

    protected override bool ShouldInterpolate => false;

    protected override void OnInitialized()
    {
        ChartOptions ??= new TimeSeriesChartOptions();

        if (ChartReference is IMudAxisChart<T> axisChart)
        {
            _generateChartLines = true;
            axisChart.OverlayChart = this;
            axisChart.OverlayContent = this.Chart;
        }

        base.OnInitialized();
    }

    public override void RebuildChart()
    {
        if (IsOverlayChart && SharedData is null) return;

        Series = (ChartContainer != null && ChartReference is MudChart<T>)
            ? ChartContainer.ChartSeries
            : ChartSeries;

        _cachedDataPoints = null;

        GeneratePlotArea(out var gridYUnits, out var lowestHorizontalLine, out var numHorizontalLines, out var horizontalSpace, out var verticalSpace);

        if (Series.Count == 0) return;
        if (!_generateChartLines) return;

        if (!IsOverlayChart)
        {
            // If this is not an overlay chart, we generate the shared plot points if an overlay exists
            SharedData = OverlayChart is IMudAxisChart<T> ? new AxisGridData<T>(lowestHorizontalLine, numHorizontalLines, gridYUnits, _boundWidth, _boundHeight) : null;
        }
        else
        {
            // If this is an overlay chart, we use the shared plot points from the main chart
            var area = SharedData!.Value;

            lowestHorizontalLine = SharedData.Value.LowestHorizontalLine;
            gridYUnits = SharedData.Value.YAxisTicks;

            _boundWidth = area.BoundWidth;
            _boundHeight = area.BoundHeight;
        }

        GenerateChartLines(lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
        GenerateLegends();

        if (OverlayChart is IMudAxisChart<T> overlay)
        {
            overlay.SharedData = SharedData;
            OverlayChart?.RebuildChart();
            StateHasChanged();
        }
    }

    private void GeneratePlotArea(out T gridYUnits, out int lowestHorizontalLine, out int numHorizontalLines, out double horizontalSpace, out double verticalSpace)
    {
        SetBounds();
        ComputeMinAndMaxDateTimes();
        ComputeUnitsAndNumberOfLines(out gridYUnits, out numHorizontalLines, out lowestHorizontalLine, out var numVerticalLines);

        var horizontalLines = IsOverlayChart ? SharedData!.Value.HorizontalLineCount : numHorizontalLines - 1;

        horizontalSpace = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, (_maxDateTime - _minDateTime) / ChartOptions!.TimeLabelSpacing);
        verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, horizontalLines);
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

    private void ComputeUnitsAndNumberOfLines(out T gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
    {
        var yAxisTicks = ChartOptions?.YAxisTicks;
        if (yAxisTicks.HasValue && yAxisTicks.Value > 0)
            gridYUnits = T.CreateSaturating(yAxisTicks.Value);
        else
            gridYUnits = T.CreateSaturating(20);

        if (Series.Any(series => series.Data.Points != null && series.Data.Points.Any()))
        {
            var minY = T.MaxValue;
            var maxY = T.MinValue;

            foreach (var series in Series.Where(s => s.Visible))
            {
                foreach (var point in series.Data.Points)
                {
                    minY = T.Min(minY, point.Y);
                    maxY = T.Max(maxY, point.Y);
                }
            }

            if (minY == T.MaxValue)
            {
                minY = T.Zero;
                maxY = T.Zero;
            }

            var hasAreaDisplay = ChartOptions?.LineDisplayType == LineDisplayType.Area || Series.Any(series => GetSeriesDisplayOverride(series)?.LineDisplayType == LineDisplayType.Area);
            var includeYAxisZeroPoint = ChartOptions?.YAxisRequireZeroPoint is true || hasAreaDisplay;
            if (includeYAxisZeroPoint)
            {
                minY = T.Min(minY, T.Zero);
                maxY = T.Max(maxY, T.Zero);
            }

            maxY = ChartOptions?.YAxisSuggestedMax is null ? maxY : T.Max(T.CreateSaturating(ChartOptions.YAxisSuggestedMax.Value), maxY);

            lowestHorizontalLine = (int)Math.Floor(double.CreateSaturating(minY / gridYUnits));
            var highestHorizontalLine = (int)Math.Ceiling(double.CreateSaturating(maxY / gridYUnits));
            numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

            var maxYTicks = ChartOptions?.MaxNumYAxisTicks ?? 100;
            while (numHorizontalLines > maxYTicks)
            {
                gridYUnits *= T.CreateSaturating(2);
                lowestHorizontalLine = (int)Math.Floor(double.CreateSaturating(minY / gridYUnits));
                highestHorizontalLine = (int)Math.Ceiling(double.CreateSaturating(maxY / gridYUnits));
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

    private TimeValue<T>[][] GetCachedDataPoints()
    {
        if (_cachedDataPoints != null)
            return _cachedDataPoints;

        _cachedDataPoints = new TimeValue<T>[Series.Count][];

        for (var i = 0; i < Series.Count; i++)
        {
            var series = Series[i];
            var points = series.Data.Points;
            var data = new TimeValue<T>[points.Count];

            for (var j = 0; j < points.Count; j++)
            {
                var point = points[j];
                var date = point.X switch
                {
                    DateTime dt => dt,
                    null => DateTime.MinValue,
                    string s when DateTime.TryParse(s, out var parsed) => parsed,
                    _ => throw new InvalidOperationException($"Unable to parse '{point.X}' as DateTime for time series chart")
                };

                data[j] = new TimeValue<T>(date, point.Y);
            }

            _cachedDataPoints[i] = data;
        }

        return _cachedDataPoints;
    }

    private void OnAxisChanged()
    {
        _generateChartLines = true;

        base.AxisChanged();
    }

    protected override TReturn GetDataValue<TReturn>(int seriesIndex, int dataPointIndex)
    {
        var data = GetCachedDataPoints()[seriesIndex];
        return (TReturn)(object)data[dataPointIndex];
    }

    protected override string GetDataValueAsString(int seriesIndex, int dataPointIndex)
    {
        var dataValue = GetDataValue<TimeValue<double>>(seriesIndex, dataPointIndex);
        return dataValue.Value.ToString(Series[seriesIndex].TooltipYValueFormat);
    }

    protected override string GetLabelXValue(int seriesIndex, int dataPointIndex)
    {
        var dataValue = GetDataValue<TimeValue<double>>(seriesIndex, dataPointIndex);
        return dataValue.DateTime.ToString(ChartOptions?.TooltipTimeLabelFormat ?? "G");
    }

    protected override (double x, double y) GetXYForDataPoint(int seriesIndex, int dataPointIndex, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
    {
        var dataPoint = GetCachedDataPoints()[seriesIndex][dataPointIndex];

        var gridValue = (dataPoint.Value / T.CreateSaturating(gridYUnits) - T.CreateSaturating(lowestHorizontalLine)) * T.CreateSaturating(verticalSpace);
        var y = _boundHeight - VerticalStartSpace - double.CreateSaturating(gridValue);

        var diffFromMin = dataPoint.DateTime - _minDateTime;
        var x = HorizontalStartSpace + (diffFromMin.TotalMilliseconds * _timeToPixelRatio);

        return (x, y);
    }

    internal override ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
    {
        throw new NotImplementedException("Interpolation not implemented yet for timeseries charts");
    }
}

/// <summary>
/// Represents a data point in a time series chart, containing a DateTime and a value.
/// </summary>
public readonly record struct TimeValue<TNumber>(DateTime DateTime, TNumber Value) where TNumber : INumber<TNumber>;
