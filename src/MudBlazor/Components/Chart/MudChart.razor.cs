// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Charts;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a graphic display of data values in a line, bar, stacked bar, pie, heat map, or donut shape.
/// </summary>
public partial class MudChart
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ChartOptions is null)
        {
            ChartOptions = GetDefaultOptionsForChart();
        }
        else if (ChartOptions is ChartOptions options)
        {
            ChartOptions = GetChartTypeOptions(options);
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && ChartReference is { })
        {
            StateHasChanged();
        }

        base.OnAfterRender(firstRender);
    }

    private IChartOptions GetChartTypeOptions(ChartOptions options) => ChartType switch
    {
        ChartType.Pie => (PieChartOptions)options,
        ChartType.Bar => (BarChartOptions)options,
        ChartType.Line => (LineChartOptions)options,
        ChartType.Donut => (DonutChartOptions)options,
        ChartType.HeatMap => (HeatMapChartOptions)options,
        ChartType.StackedBar => (StackedBarChartOptions)options,
        _ => ChartOptions!
    };

    private IChartOptions GetDefaultOptionsForChart() => ChartType switch
    {
        ChartType.Pie => new PieChartOptions(),
        ChartType.Bar => new BarChartOptions(),
        ChartType.Line => new LineChartOptions(),
        ChartType.Donut => new DonutChartOptions(),
        ChartType.HeatMap => new HeatMapChartOptions(),
        ChartType.StackedBar => new StackedBarChartOptions(),
        _ => throw new NotImplementedException($"{ChartType} chart is not supported")
    };
}
