using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor
{
    public abstract class MudTimeSeriesChartBase : MudChartBase
    {
        /// <summary>
        /// The series of values to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public List<TimeSeriesChartSeries> ChartSeries { get; set; } = [];

        /// <summary>
        /// A way to have minimum spacing between timestamp labels, default of 5 minutes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public TimeSpan TimeLabelSpacing { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// Determines whether timestamp labels should be rounded to the nearest spacing value. 
        /// </summary>
        /// <remarks>
        /// Default is <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public bool TimeLabelSpacingRounding { get; set; }

        /// <summary>
        /// Determines how timestamp labels are adjusted when <see cref="TimeLabelSpacingRounding"/> is enabled.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, the series is padded to allow rounding with labels before and after the series start and end.
        /// When <c>false</c>, labels are moved inward to align with the label spacing without altering the axis start and end times.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public bool TimeLabelSpacingRoundingPadSeries { get; set; }

        /// <summary>
        /// Specifies the datetime format for timestamp labels. 
        /// </summary>
        /// <remarks>
        /// Defaults to <c>"HH:mm"</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string TimeLabelFormat { get; set; } = "HH:mm";

        /// <summary>
        /// Specifies the DateTime format for Timestamp labels in DataPoint marker tooltips. 
        /// </summary>
        /// <remarks>
        /// Defaults to <c>"HH:mm"</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string DataMarkerTooltipTimeLabelFormat { get; set; } = "HH:mm";

        /// <summary>
        /// Specifies the title for the X axis.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string? XAxisTitle { get; set; }

        /// <summary>
        /// Specifies the title for the Y axis.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string? YAxisTitle { get; set; }

        /// <summary>
        /// Determines if the chart should derive its bounds from the parent chart.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        [Obsolete("Use MatchBoundsToSize from the MudChartParents AxisChartOptions.MatchBoundsToSize instead. This will be removed in a future major version.", false)]
        public bool MatchBoundsToSize { get; set; }
    }
}
