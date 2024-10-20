using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// Represents a base class for designing category <see cref="MudChart"/> components.
    /// </summary>
    public abstract class MudCategoryChartBase : MudChartBase
    {
        /// <summary>
        /// The data to be displayed.
        /// </summary>
        /// <remarks>
        /// Applies to <c>Pie</c> and <c>Donut</c> charts.  The number of values in this array should be the same as the number of labels in the <see cref="InputLabels"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public double[]? InputData { get; set; } = [];

        /// <summary>
        /// The labels describing data values.
        /// </summary>
        /// <remarks>
        /// Applies to <c>Pie</c> and <c>Donut</c> charts.  The number of labels in this array is typically the same as the number of values in the <see cref="InputData"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string[] InputLabels { get; set; } = [];

        /// <summary>
        /// The labels applied to the horizontal axis.
        /// </summary>
        /// <remarks>
        /// Applies to <c>Line</c>, <c>Bar</c>, and <c>StackedBar</c> charts.  The number of values in this array is typically equal to the number of values in the <see cref="ChartSeries.Data"/> property.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public string[] XAxisLabels { get; set; } = [];

        /// <summary>
        /// The series of values to display.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public List<ChartSeries> ChartSeries { get; set; } = [];

        /// <summary>
        /// Scales the input data to the range between 0 and 1
        /// </summary>
        protected double[] GetNormalizedData()
        {
            if (InputData == null || InputData.Length == 0)
                return [];

            var total = InputData.Sum();
            return InputData.Select(x => Math.Abs(x) / total).ToArray();
        }
    }
}
