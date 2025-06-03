// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

#nullable enable
namespace MudBlazor.Charts;

public partial class BaseRadialChart<TChartOptions> : MudComponentBase where TChartOptions : IRadialChartOptions, new()
{
    private ElementReference _svgRef;

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Width { get; set; } = string.Empty;

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Height { get; set; } = string.Empty;

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double Radius { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<SvgPath> Paths { get; set; } = [];

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public TChartOptions ChartOptions { get; set; } = new();

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string ChartClass { get; set; } = string.Empty;

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<ChartSeries> ChartSeries { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string[] ChartLabels { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public SvgPath? HoveredSegment { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback OnMouseOut { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<int> OnPathClick { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<(MouseEventArgs Args, SvgPath Segment)> OnMouseOver { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<ElementReference> ElementRefChanged { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? CustomGraphics { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? ChartGrid { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment<SvgPolygon>? ChartDataPoints { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment<(SvgPath Segment, string Color)>? TooltipTemplate { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public Func<SvgPath, (double X, double Y)>? TooltipPositionFunc { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && ElementRefChanged.HasDelegate)
            await ElementRefChanged.InvokeAsync(_svgRef);
    }

    private string GetColor(int index)
    {
        if (ChartOptions?.ChartPalette is null || ChartOptions?.ChartPalette.Length == 0)
            return string.Empty;

        return ChartOptions!.ChartPalette.GetValue(index % ChartOptions.ChartPalette.Length)?.ToString() ?? string.Empty;
    }

    private (string? title, string? subtitle) BuildTooltipFormat()
    {
        if (HoveredSegment == null)
            return (string.Empty, string.Empty);

        var series = ChartOptions.AggregationOption == AggregationOption.GroupByDataSet && HoveredSegment.Index >= 0 && HoveredSegment.Index < ChartSeries.Count
                     ? ChartSeries[HoveredSegment.Index]
                     : null;
        var tooltipTitleFormat = series?.TooltipTitleFormat ?? ChartOptions.TooltipTitleFormat;
        var tooltipSubtitleFormat = series?.TooltipSubtitleFormat ?? ChartOptions.TooltipSubtitleFormat;

        if (string.IsNullOrWhiteSpace(tooltipTitleFormat))
            return (string.Empty, string.Empty);

        var title = tooltipTitleFormat?
            .Replace("{{SERIES_NAME}}", GetSeriesName(ChartOptions.AggregationOption))
            .Replace("{{X_VALUE}}", HoveredSegment.LabelXValue)
            .Replace("{{Y_VALUE}}", HoveredSegment.LabelYValue);

        var subtitle = tooltipSubtitleFormat?
            .Replace("{{SERIES_NAME}}", GetSeriesName(ChartOptions.AggregationOption))
            .Replace("{{X_VALUE}}", HoveredSegment.LabelXValue)
            .Replace("{{Y_VALUE}}", HoveredSegment.LabelYValue);

        return (title, subtitle);
    }

    public string GetSeriesName(AggregationOption aggregation)
    {
        if (ChartSeries is null || ChartSeries.Count == 0)
            return string.Empty;

        switch (aggregation)
        {
            case AggregationOption.GroupByLabel:
                var chartSeries = ChartSeries.Where(x => x.Visible).ToArray();

                if (chartSeries.Length == 1)
                    return chartSeries[0].Name;

                if (HoveredSegment is SvgPathPoint point)
                    return chartSeries[point.PointIndex].Name;

                return chartSeries.Length.ToString();

            case AggregationOption.GroupByDataSet:
                if (ChartLabels.Length == 1)
                    return ChartLabels[0];

                if (HoveredSegment is SvgPathPoint hoveredPoint)
                    return ChartLabels[hoveredPoint.PointIndex];

                return ChartLabels.Length.ToString();

            default:
                throw new ArgumentOutOfRangeException(nameof(aggregation), $"Unsupported aggregation: {aggregation}");
        }
    }
}
