using Microsoft.AspNetCore.Components;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public static new ChartType ChartType => ChartType.Line;

        public override RenderFragment? OverlayContent { get; set; }

        protected override bool ShouldInterpolate => true;

        protected override void OnInitialized()
        {
            ChartOptions ??= new LineChartOptions();

            if (ChartReference is IMudAxisChart axisChart)
            {
                axisChart.OverlayChart = this;
                axisChart.OverlayContent = this.Chart;
            }

            base.OnInitialized();
        }

        public override void RebuildChart()
        {
            if (ChartReference is IMudAxisChart && PlotArea is null) return;

            Series = (ChartContainer != null && ChartReference is MudChart)
                ? ChartContainer.ChartSeries
                : ChartSeries;

            if (ChartReference is not IMudAxisChart)
            {
                GeneratePlotArea(out var gridYUnits, out var lowestHorizontalLine, out var horizontalSpace, out var verticalSpace);

                PlotArea = new PlotArea(horizontalSpace, verticalSpace, lowestHorizontalLine, 0, gridYUnits);
            }

            if (PlotArea is not { } area) return;

            GenerateChartLines(area.LowestHorizontalLine, area.YAxisTicks, area.Width, area.Height);

            if (OverlayChart is IMudAxisChart overlay && PlotArea != overlay.PlotArea)
            {
                overlay.PlotArea = PlotArea;
                OverlayChart?.RebuildChart();
                StateHasChanged();
            }
        }

        private void GeneratePlotArea(out double gridYUnits, out int lowestHorizontalLine, out double horizontalSpace, out double verticalSpace)
        {
            SetBounds();
            ComputeUnitsAndNumberOfLines(out gridYUnits, out var numHorizontalLines, out lowestHorizontalLine, out var numVerticalLines);

            horizontalSpace = (_boundWidth - HorizontalStartSpace - HorizontalEndSpace) / Math.Max(1, numVerticalLines - 1);
            verticalSpace = (_boundHeight - VerticalStartSpace - VerticalEndSpace) / Math.Max(1, numHorizontalLines - 1);
            GenerateHorizontalGridLines(numHorizontalLines, lowestHorizontalLine, gridYUnits, verticalSpace);
            GenerateVerticalGridLines(numVerticalLines, 0, horizontalSpace);
        }

        private void ComputeUnitsAndNumberOfLines(out double gridYUnits, out int numHorizontalLines, out int lowestHorizontalLine, out int numVerticalLines)
        {
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
