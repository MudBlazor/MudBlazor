using System.Numerics;
using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a set of text labels which describe data values in a <see cref="MudChart{T}"/>.
    /// </summary>
    public partial class Legend<T> : MudChartBase<T, IChartOptions> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    {
        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart<T>? ChartContainer { get; set; }

        /// <summary>
        /// The data labels for this legend.
        /// </summary>
        [Parameter]
        [EditorRequired]
        public List<SvgLegend> Data { get; set; } = [];

        /// <summary>
        /// Whether the chart legend should be displayed.
        /// </summary>
        [Parameter]
        public bool? ShowLegend { get; set; }

        /// <summary>
        /// The palette of colors to use for the chart series.
        /// </summary>
        [Parameter]
        public string[]? ChartPalette { get; set; }

        /// <summary>
        /// Raised when a legend item is clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public EventCallback<int> OnLegendSelected { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            CanHideSeries = ChartContainer?.CanHideSeries ?? CanHideSeries;
            ShowLegend ??= ChartContainer?.ChartOptions?.ShowLegend ?? true;
            ChartPalette ??= ChartContainer?.ChartOptions?.ChartPalette ?? new ChartOptions().ChartPalette;

            if (!OnLegendSelected.HasDelegate && ChartContainer is not null)
                OnLegendSelected = EventCallback.Factory.Create<int>(this, async index => await ChartContainer!.SetSelectedIndexAsync(index));
        }

        private string GetCheckBoxStyle(int index)
        {
            var color = ChartPalette?.GetValue(index % ChartPalette.Length)?.ToString() ?? string.Empty;
            return $"--checkbox-color: {color};";
        }

        public override void RebuildChart() => ChartContainer?.RebuildChart();
    }
}
