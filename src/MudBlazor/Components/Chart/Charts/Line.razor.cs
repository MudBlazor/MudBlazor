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

        protected override void OnInitialized()
        {
            ChartOptions ??= new LineChartOptions();
            base.OnInitialized();
        }

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

            var visibleSeries = Series.Where(series => series.Visible).ToArray();
            var values = visibleSeries.SelectMany(series => series.Data.Values);

            if (visibleSeries.Length > 0 && values.Any())
            {
                var minY = values.Min();
                var maxY = ChartOptions?.YAxisSuggestedMax is null
                    ? values.Max()
                    : Math.Max(ChartOptions.YAxisSuggestedMax.Value, values.Max());

                var hasAreaDisplay = ChartOptions?.LineDisplayType == LineDisplayType.Area || visibleSeries.Any(series => GetSeriesDisplayOverride(series)?.LineDisplayType == LineDisplayType.Area);
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

                numVerticalLines = visibleSeries.Max(series => series.Data.Values.Length);
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
            return index < ChartLabels.Length ? ChartLabels[index] : "";
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

            var xValues = new double[data.Values.Length];
            var yValues = new double[data.Values.Length];

            for (var j = 0; j < data.Values.Length; j++)
            {
                var (x, y) = (xValues[j], yValues[j]) = GetXYForDataPoint(seriesIndex, j, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
            }

            var overrideSettings = GetSeriesDisplayOverride(series);
            var interpolationOption = overrideSettings?.InterpolationOption ?? ChartOptions?.InterpolationOption;

            ILineInterpolator interpolator = interpolationOption switch
            {
                InterpolationOption.NaturalSpline => new NaturalSpline(xValues, yValues, interpolationResolution),
                InterpolationOption.EndSlope => new EndSlopeSpline(xValues, yValues, interpolationResolution),
                InterpolationOption.Periodic => new PeriodicSpline(xValues, yValues, interpolationResolution),
                _ => throw new NotImplementedException("Interpolation option not implemented yet")
            };

            return interpolator;
        }
    }
}
