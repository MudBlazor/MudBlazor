using System.Numerics;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interpolation;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a chart which displays series values as connected lines.
    /// </summary>
    /// <seealso cref="Bar{T}"/>
    /// <seealso cref="Donut{T}"/>
    /// <seealso cref="Pie{T}"/>
    /// <seealso cref="StackedBar{T}"/>
    /// <seealso cref="TimeSeries{T}"/>
    partial class Line<T> : MudAxisLineChartBase<T, LineChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        public override RenderFragment? OverlayContent { get; set; }

        protected override bool ShouldInterpolate => true;

        protected override void OnInitialized()
        {
            ChartType = ChartType.Line;
            ChartOptions ??= new LineChartOptions();

            if (ChartReference is IMudAxisChart<T> axisChart)
            {
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

            GeneratePlotArea(out var gridYUnits, out var lowestHorizontalLine, out var numHorizontalLines, out var horizontalSpace, out var verticalSpace);

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
            ComputeUnitsAndNumberOfLines(out gridYUnits, out numHorizontalLines, out lowestHorizontalLine, out var numVerticalLines);

            var horizontalLines = IsOverlayChart ? SharedData!.Value.HorizontalLineCount : numHorizontalLines - 1;

            horizontalSpace = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, numVerticalLines - 1);
            verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, horizontalLines);

            // If this is an overlay chart, we do not generate the grid lines
            if (IsOverlayChart) return;

            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, 0, horizontalSpace);
        }

        private void ComputeUnitsAndNumberOfLines(out T gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
            var yAxisTicks = ChartOptions?.YAxisTicks;
            if (yAxisTicks.HasValue && yAxisTicks.Value > 0)
                gridYUnits = T.CreateSaturating(yAxisTicks.Value);
            else
                gridYUnits = T.CreateSaturating(20);

            var visibleSeries = Series.Where(series => series.Visible).ToArray();
            var values = visibleSeries.SelectMany(series => series.Data.Values);

            if (visibleSeries.Length > 0 && values.Any())
            {
                var minY = values.Min();
                var maxY = ChartOptions?.YAxisSuggestedMax is null
                    ? values.Max()
                    : T.Max(T.CreateSaturating(ChartOptions.YAxisSuggestedMax.Value), values.Max());

                var hasAreaDisplay = ChartOptions?.LineDisplayType == LineDisplayType.Area || visibleSeries.Any(series => GetSeriesDisplayOverride(series)?.LineDisplayType == LineDisplayType.Area);
                var includeYAxisZeroPoint = ChartOptions?.YAxisRequireZeroPoint is true || hasAreaDisplay;

                if (includeYAxisZeroPoint)
                {
                    minY = T.Min(minY, T.Zero); // we want to include the 0 in the grid
                    maxY = T.Max(maxY, T.Zero); // we want to include the 0 in the grid
                }

                lowestHorizontalLine = (int)Math.Floor(double.CreateSaturating(minY / gridYUnits));
                var highestHorizontalLine = (int)Math.Ceiling(double.CreateSaturating(maxY / gridYUnits));
                numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;

                // this is a safeguard against millions of gridlines which might arise with very high values
                var maxYTicks = ChartOptions?.MaxNumYAxisTicks ?? 100;
                while (numHorizontalLines > maxYTicks)
                {
                    gridYUnits *= T.CreateSaturating(2);
                    lowestHorizontalLine = (int)Math.Floor(double.CreateSaturating(minY / gridYUnits));
                    highestHorizontalLine = (int)Math.Ceiling(double.CreateSaturating(maxY / gridYUnits));
                    numHorizontalLines = highestHorizontalLine - lowestHorizontalLine + 1;
                }

                numVerticalLines = visibleSeries.Max(series => series.Data.Values.Count);
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

        protected override TReturn GetDataValue<TReturn>(int seriesIndex, int dataPointIndex)
        {
            return (TReturn)Convert.ChangeType(Series[seriesIndex].Data.Values[dataPointIndex], typeof(T));
        }

        protected override string GetLabelXValue(int seriesIndex, int dataPointIndex)
        {
            return ChartLabels.Length > dataPointIndex ? ChartLabels[dataPointIndex] : string.Empty;
        }

        protected override (double x, double y) GetXYForDataPoint(int seriesIndex, int dataPointIndex, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
        {
            var data = Series[seriesIndex].Data;
            var x = HorizontalStartSpace + (dataPointIndex * horizontalSpace);
            var gridValue = (double.CreateSaturating(data[dataPointIndex].Y / gridYUnits) - lowestHorizontalLine) * verticalSpace;
            var y = _boundHeight - VerticalStartSpace - double.CreateSaturating(gridValue);
            return (x, y);
        }

        internal override ILineInterpolator CreateInterpolator(int seriesIndex, int lowestHorizontalLine, T gridYUnits, double horizontalSpace, double verticalSpace)
        {
            var series = Series[seriesIndex];
            var data = series.Data;
            var interpolationResolution = 10;

            var xValues = new double[data.Values.Count];
            var yValues = new double[data.Values.Count];

            for (var j = 0; j < data.Values.Count; j++)
            {
                (xValues[j], yValues[j]) = GetXYForDataPoint(seriesIndex, j, lowestHorizontalLine, gridYUnits, horizontalSpace, verticalSpace);
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
