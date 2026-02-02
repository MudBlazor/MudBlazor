using System.Globalization;
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
    [Inject]
    private TimeProvider TimeProvider { get; set; } = null!;

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
        ChartType = ChartType.Timeseries;
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
        RenderOverlay();
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

        var (minDate, maxDate) = GetVisibleDateRange();

        var labelSpacing = ChartOptions!.TimeLabelSpacing;

        if (minDate == null || maxDate == null)
        {
            SetDefaultDateRange(labelSpacing);
            return;
        }

        _minDateTime = minDate.Value;
        _maxDateTime = maxDate.Value;

        if (!ChartOptions.TimeLabelSpacingRounding)
            return;

        ApplyLabelRounding(labelSpacing);
    }

    private (DateTime? Min, DateTime? Max) GetVisibleDateRange()
    {
        DateTime? min = null, max = null;

        foreach (var series in Series.Where(s => s.Visible && s.Data.Points != null))
        {
            foreach (var dt in series.Data.Points.Select(p => p.X).OfType<DateTime>())
            {
                if (min == null || dt < min) min = dt;
                if (max == null || dt > max) max = dt;
            }
        }

        return (min, max);
    }

    private void SetDefaultDateRange(TimeSpan spacing)
    {
        var now = TimeProvider.GetLocalNow().DateTime;
        _minDateTime = now;
        _maxDateTime =
            spacing.Days > 0 ? now.AddDays(1) :
            spacing.Minutes > 0 ? now.AddHours(1) :
            now.AddMinutes(1);
    }

    private void ApplyLabelRounding(TimeSpan spacing)
    {
        if (_minDateTime.Ticks % spacing.Ticks != 0)
        {
            var offset = new TimeSpan(_minDateTime.Ticks % spacing.Ticks);

            if (ChartOptions!.TimeLabelSpacingRoundingPadSeries)
                _minDateTime = _minDateTime.Subtract(offset);
            else
                _minDateLabelOffset = spacing - offset;
        }

        if (ChartOptions!.TimeLabelSpacingRoundingPadSeries && _maxDateTime.Ticks % spacing.Ticks != 0)
        {
            var offset = spacing - new TimeSpan(_maxDateTime.Ticks % spacing.Ticks);
            _maxDateTime = _maxDateTime.Add(offset);
        }
    }

    private void ComputeUnitsAndNumberOfLines(out T gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
    {
        gridYUnits = GetInitialGridUnit();

        if (!HasSeriesData())
        {
            numHorizontalLines = 1;
            lowestHorizontalLine = 0;
            numVerticalLines = 1;
            return;
        }

        var (minY, maxY) = GetYRangeWithPadding();

        AdjustSuggestedMax(ref maxY);

        lowestHorizontalLine = GetLowestLine(minY, gridYUnits);
        var highestHorizontalLine = GetHighestLine(maxY, gridYUnits);
        numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

        ClampHorizontalLines(ref gridYUnits, minY, maxY, ref numHorizontalLines, ref lowestHorizontalLine);

        numVerticalLines = CalculateVerticalLines();
    }

    private T GetInitialGridUnit()
    {
        var yAxisTicks = ChartOptions?.YAxisTicks;
        return yAxisTicks is > 0
            ? T.CreateSaturating(yAxisTicks.Value)
            : T.CreateSaturating(20);
    }

    private bool HasSeriesData() =>
        Series.Any(series => series.Data.Points is { Count: > 0 });

    private (T minY, T maxY) GetYRangeWithPadding()
    {
        var minY = T.MaxValue;
        var maxY = T.MinValue;

        foreach (var point in Series.Where(s => s.Visible).SelectMany(s => s.Data.Points))
        {
            minY = T.Min(minY, point.Y);
            maxY = T.Max(maxY, point.Y);
        }

        if (minY.Equals(T.MaxValue))
            return (T.Zero, T.Zero);

        var requireZero = ChartOptions?.YAxisRequireZeroPoint == true || HasAreaSeries();
        if (requireZero)
        {
            minY = T.Min(minY, T.Zero);
            maxY = T.Max(maxY, T.Zero);
        }

        return (minY, maxY);
    }

    private bool HasAreaSeries() =>
        ChartOptions?.LineDisplayType == LineDisplayType.Area ||
        Series.Any(s => GetSeriesDisplayOverride(s)?.LineDisplayType == LineDisplayType.Area);

    private void AdjustSuggestedMax(ref T maxY)
    {
        if (ChartOptions?.YAxisSuggestedMax is { } suggested)
            maxY = T.Max(T.CreateSaturating(suggested), maxY);
    }

    private static int GetLowestLine(T minY, T unit) =>
        (int)Math.Floor(double.CreateSaturating(minY / unit));

    private static int GetHighestLine(T maxY, T unit) =>
        (int)Math.Ceiling(double.CreateSaturating(maxY / unit));

    private void ClampHorizontalLines(ref T unit, T minY, T maxY, ref int numLines, ref int lowestLine)
    {
        var maxTicks = ChartOptions?.MaxNumYAxisTicks ?? 100;

        while (numLines > maxTicks)
        {
            unit *= T.CreateSaturating(2);
            lowestLine = GetLowestLine(minY, unit);

            var highestLine = GetHighestLine(maxY, unit);

            numLines = highestLine - lowestLine + 1;
        }
    }

    private int CalculateVerticalLines()
    {
        var spacing = ChartOptions!.TimeLabelSpacing;
        return (int)Math.Ceiling((_maxDateTime - _minDateTime) / spacing) + 1;
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
