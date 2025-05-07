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
        public List<SvgLegend> Data { get; set; } = [];

        private string GetCheckBoxStyle(int index)
        {
            var color = MudChartParent?.ChartOptions!.ChartPalette.GetValue(index % MudChartParent.ChartOptions!.ChartPalette.Length);
            return $"--checkbox-color: {color};";
        }
    }
}
