// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

#nullable enable
namespace MudBlazor.Charts;

/// <summary>
/// Represents a base component for radial charts.
/// </summary>
/// <typeparam name="T">The data type of the chart.</typeparam>
/// <typeparam name="TChartOptions">The type of chart options.</typeparam>
public partial class BaseRadialChart<T, TChartOptions> : MudComponentBase
    where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    where TChartOptions : IRadialChartOptions, new()
{
    private ElementReference _svgRef;

    /// <summary>
    /// The width of the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Width { get; set; } = string.Empty;

    /// <summary>
    /// The height of the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Height { get; set; } = string.Empty;

    /// <summary>
    /// The radius of the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double Radius { get; set; }

    /// <summary>
    /// The SVG paths for the chart segments.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<SvgPath> Paths { get; set; } = [];

    /// <summary>
    /// The options for the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public TChartOptions ChartOptions { get; set; } = new();

    /// <summary>
    /// The CSS class of the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string ChartClass { get; set; } = string.Empty;

    /// <summary>
    /// The series of data for the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<ChartSeries<T>> ChartSeries { get; set; } = [];

    /// <summary>
    /// The labels for the chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string[] ChartLabels { get; set; } = [];

    /// <summary>
    /// The currently hovered segment.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public SvgPath? HoveredSegment { get; set; }

    /// <summary>
    /// The event callback for when the mouse leaves a segment.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback OnMouseOut { get; set; }

    /// <summary>
    /// The event callback for when a path is clicked.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<int> OnPathClick { get; set; }

    /// <summary>
    /// The event callback for when the mouse enters a segment.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<(MouseEventArgs Args, SvgPath Segment)> OnMouseOver { get; set; }

    /// <summary>
    /// The event callback for when the element reference changes.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<ElementReference> ElementRefChanged { get; set; }

    /// <summary>
    /// The custom graphics for the chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? CustomGraphics { get; set; }

    /// <summary>
    /// The grid area for the chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? ChartGrid { get; set; }

    /// <summary>
    /// The data points for the chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment<SvgPolygon>? ChartDataPoints { get; set; }

    /// <summary>
    /// The template for the tooltip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment<(SvgPath Segment, string Color)>? TooltipTemplate { get; set; }

    /// <summary>
    /// The function to determine the position of the tooltip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public Func<SvgPath, (double X, double Y)>? TooltipPositionFunc { get; set; }

    private string HoveredStylename =>
        new StyleBuilder()
            .AddStyle("overflow", "visible", HoveredSegment is not null)
            .Build();

    /// <summary>
    /// Called after the component has been rendered.
    /// </summary>
    /// <param name="firstRender">True if this is the first time the component is rendering; otherwise, false.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await ElementRefChanged.InvokeAsync(_svgRef);
    }

    private string GetColor(int index)
    {
        if (ChartOptions?.ChartPalette is null || ChartOptions.ChartPalette.Length == 0)
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

        var title = tooltipTitleFormat
            .Replace("{{SERIES_NAME}}", GetSeriesName(ChartOptions.AggregationOption))
            .Replace("{{X_VALUE}}", HoveredSegment.LabelXValue)
            .Replace("{{Y_VALUE}}", HoveredSegment.LabelYValue);

        var subtitle = tooltipSubtitleFormat?
            .Replace("{{SERIES_NAME}}", GetSeriesName(ChartOptions.AggregationOption))
            .Replace("{{X_VALUE}}", HoveredSegment.LabelXValue)
            .Replace("{{Y_VALUE}}", HoveredSegment.LabelYValue);

        return (title, subtitle);
    }

    /// <summary>
    /// Gets the name of the series based on the aggregation option.
    /// </summary>
    /// <param name="aggregation">The aggregation option.</param>
    /// <returns>The name of the series.</returns>
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

    private static double EstimateFontSize(string text, double radius, double angleRadians)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 8;

        const int MinTextLength = 2;
        const int MaxTextLength = 6;
        const double ArcLengthDivisor = 3;
        const double FontSizeOffset = 1.6;
        const double MinFontSize = 6;
        const double MaxFontSize = 30;

        var arcLength = angleRadians * radius;
        var avgCharWidth = 0.6;
        var estimatedTextWidth = text.Length.EnsureRange(MinTextLength, MaxTextLength)
                                 * avgCharWidth
                                 * Math.Sqrt(arcLength / ArcLengthDivisor);
        var size = (arcLength / estimatedTextWidth) + FontSizeOffset;

        return size.EnsureRange(MinFontSize, MaxFontSize);
    }
}
