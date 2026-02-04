// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Numerics;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;
using MudBlazor.Utilities;

namespace MudBlazor.Charts;

/// <summary>
/// Represents a base component for charts with axes.
/// </summary>
/// <typeparam name="T">The data type of the chart.</typeparam>
/// <typeparam name="TChartOptions">The type of chart options.</typeparam>
public partial class BaseAxisChart<T, TChartOptions> : MudComponentBase
    where T : struct, INumber<T>, IMinMaxValue<T>, IFormattable
    where TChartOptions : IAxisChartOptions, new()
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private ElementReference _svgRef;
    private ElementReference? _xAxisGroupElementReference;
    private ElementReference? _yAxisGroupElementReference;

    /// <summary>
    /// The size of the Y-axis labels.
    /// </summary>
    protected ElementSize? _yAxisLabelSize;

    /// <summary>
    /// The size of the X-axis labels.
    /// </summary>
    protected ElementSize? _xAxisLabelSize;

    /// <summary>
    /// The width of the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Width { get; set; }

    /// <summary>
    /// The height of the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Height { get; set; }

    /// <summary>
    /// The type of the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public ChartType? ChartType { get; set; }

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
    /// The options for the chart.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public TChartOptions ChartOptions { get; set; }

    /// <summary>
    /// The labels for the chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string[] ChartLabels { get; set; } = [];

    /// <summary>
    /// The width of the view box.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double ViewBoxWidth { get; set; }

    /// <summary>
    /// The height of the view box.
    /// </summary>
    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double ViewBoxHeight { get; set; }

    /// <summary>
    /// The child content of the component.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The content for the series.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? SeriesContent { get; set; }

    /// <summary>
    /// The content for the tooltip.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? TooltipContent { get; set; }

    /// <summary>
    /// The custom graphics for the chart.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? CustomGraphics { get; set; }

    /// <summary>
    /// The currently hovered segment.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public SvgPath? HoveredSegment { get; set; }

    /// <summary>
    /// The horizontal lines of the grid.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public IList<SvgPath> HorizontalLines { get; set; } = [];

    /// <summary>
    /// The vertical lines of the grid.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public IList<SvgPath> VerticalLines { get; set; } = [];

    /// <summary>
    /// The horizontal values of the grid.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public IList<SvgText> HorizontalValues { get; set; } = [];

    /// <summary>
    /// The vertical values of the grid.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public IList<SvgText> VerticalValues { get; set; } = [];

    /// <summary>
    /// The title of the X-axis.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string? XAxisTitle { get; set; }

    /// <summary>
    /// The title of the Y-axis.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string? YAxisTitle { get; set; }

    /// <summary>
    /// The event callback for when the axis changes.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback AxisChanged { get; set; }

    /// <summary>
    /// The event callback for when the element reference changes.
    /// </summary>
    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<ElementReference> ElementRefChanged { get; set; }

    private string HoveredStylename =>
        new StyleBuilder()
            .AddStyle("overflow", "visible", HoveredSegment is not null)
            .Build();

    private string TooltipStylename =>
        new StyleBuilder()
            .AddStyle("display", "block", HoveredSegment is not null)
            .AddStyle("display", "none", HoveredSegment is null)
            .Build();

    /// <summary>
    /// Called after the component has been rendered.
    /// </summary>
    /// <param name="firstRender">True if this is the first time the component is rendering; otherwise, false.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
            await ElementRefChanged.InvokeAsync(_svgRef);

        var yAxisLabelSize = _yAxisGroupElementReference != null ? await JsRuntime.InvokeAsync<ElementSize>("mudGetSvgBBox", _yAxisGroupElementReference) : null;
        var xAxisLabelSize = _xAxisGroupElementReference != null ? await JsRuntime.InvokeAsync<ElementSize>("mudGetSvgBBox", _xAxisGroupElementReference) : null;

        var axisChanged = false;
        var comparer = new DoubleEpsilonEqualityComparer(0.01);

        if (yAxisLabelSize != null && (_yAxisLabelSize == null || !comparer.Equals(yAxisLabelSize.Width, _yAxisLabelSize.Width)))
        {
            _yAxisLabelSize = yAxisLabelSize;
            axisChanged = true;
        }

        if (xAxisLabelSize != null && (_xAxisLabelSize == null || !comparer.Equals(xAxisLabelSize.Height, _xAxisLabelSize.Height)))
        {
            _xAxisLabelSize = xAxisLabelSize;
            axisChanged = true;
        }

        if (axisChanged && AxisChanged.HasDelegate)
            await AxisChanged.InvokeAsync();
    }
}
