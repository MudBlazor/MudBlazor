using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor.Charts
{
    /// <summary>
    /// Represents a set of text labels which describe data values in a <see cref="MudChart"/>.
    /// </summary>
    public partial class Legend : MudChartBase<IChartOptions>
    {
        /// <summary>
        /// The chart, if any, containing this component.
        /// </summary>
        [CascadingParameter]
        public MudChart? MudChartParent { get; set; }

        /// <summary>
        /// The data labels for this legend.
        /// </summary>
        [Parameter]
        [EditorRequired]
        public List<SvgLegend> Data { get; set; } = [];

        [Parameter]
        public bool? ShowLegend { get; set; }

        [Parameter]
        public string[]? ChartPalette { get; set; }

        [Parameter]
        [Category(CategoryTypes.Chart.Behavior)]
        public EventCallback<int> OnLegendSelected { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            CanHideSeries = MudChartParent?.CanHideSeries ?? CanHideSeries;
            ShowLegend ??= MudChartParent?.ChartOptions?.ShowLegend ?? true;
            ChartPalette ??= MudChartParent?.ChartOptions?.ChartPalette ?? [];

            if (!OnLegendSelected.HasDelegate && MudChartParent is not null)
                OnLegendSelected = EventCallback.Factory.Create<int>(this, async index => await MudChartParent!.SetSelectedIndexAsync(index));
        }

        private string GetCheckBoxStyle(int index)
        {
            var color = ChartPalette?.GetValue(index % ChartPalette.Length)?.ToString() ?? string.Empty;
            return $"--checkbox-color: {color};";
        }
    }
}
