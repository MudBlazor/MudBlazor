// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Interop;

#nullable enable
namespace MudBlazor.Charts;

public partial class BaseAxisChart<TChartOptions> : MudComponentBase where TChartOptions : IAxisChartOptions, new()
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    private ElementReference _svgRef;
    private ElementReference? _xAxisGroupElementReference;
    private ElementReference? _yAxisGroupElementReference;

    protected ElementSize? _yAxisLabelSize;
    protected ElementSize? _xAxisLabelSize;

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Width { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string Height { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public ChartType? ChartType { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public string ChartClass { get; set; } = string.Empty;

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<ChartSeries> ChartSeries { get; set; } = [];

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public TChartOptions ChartOptions { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string[] ChartLabels { get; set; } = [];

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double ViewBoxWidth { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double ViewBoxHeight { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? SeriesContent { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? TooltipContent { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public RenderFragment? CustomGraphics { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public SvgPath? HoveredSegment { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public IList<SvgPath> HorizontalLines { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public IList<SvgPath> VerticalLines { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public IList<SvgText> HorizontalValues { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public IList<SvgText> VerticalValues { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string? XAxisTitle { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string? YAxisTitle { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback AxisChanged { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<ElementReference> ElementRefChanged { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && ElementRefChanged.HasDelegate)
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
