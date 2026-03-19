// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using MudBlazor.Charts;

namespace MudBlazor;

/// <summary>
/// Represents a graphic display of data values in a line, bar, stacked bar, pie, heat map, or donut shape.
/// </summary>
public partial class MudChart<T> where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
{
    private ChartType? _chartType;
    private IChartOptions? _chartOptions;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (ChartType != _chartType)
        {
            _chartType = ChartType;
            _chartOptions = null; // Reset options when chart type changes
        }

        _chartOptions = ChartOptions switch
        {
            null => GetDefaultOptionsForChart(),
            ChartOptions options => GetChartTypeOptions(options),
            _ => ChartOptions
        };
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && ChartReference is not null)
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
        ChartType.Timeseries => (TimeSeriesChartOptions)options,
        ChartType.Rose => (RoseChartOptions)options,
        ChartType.Radar => (RadarChartOptions)options,
        ChartType.Sankey => (SankeyChartOptions)options,
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
        ChartType.Timeseries => new TimeSeriesChartOptions(),
        ChartType.Rose => new RoseChartOptions(),
        ChartType.Radar => new RadarChartOptions(),
        ChartType.Sankey => new SankeyChartOptions(),
        _ => throw new NotImplementedException($"{ChartType} chart is not supported")
    };

    public override void RebuildChart() => ChartReference?.RebuildChart();
}
