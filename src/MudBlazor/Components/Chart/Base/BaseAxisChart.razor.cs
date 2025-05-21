// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

#nullable enable
namespace MudBlazor.Charts;

public partial class BaseAxisChart<TChartOptions> : MudComponentBase where TChartOptions : IAxisChartOptions, new()
{
    private ElementReference _svgRef;
    private ElementReference _xAxisGroupElementReference;
    private ElementReference _yAxisGroupElementReference;

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
    public string ChartClass { get; set; } = string.Empty;

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<ChartSeries> ChartSeries { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string[] ChartLabels { get; set; } = [];

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public TChartOptions ChartOptions { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<SvgPath> Paths { get; set; } = [];

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double ViewBoxWidth { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double ViewBoxHeight { get; set; }

    [Parameter]
    public IList<SvgPath> HorizontalLines { get; set; } = new List<SvgPath>();

    [Parameter]
    public IList<SvgPath> VerticalLines { get; set; } = new List<SvgPath>();

    [Parameter]
    public IList<SvgValue> HorizontalValues { get; set; } = new List<SvgValue>();

    [Parameter]
    public IList<SvgValue> VerticalValues { get; set; } = new List<SvgValue>();

    [Parameter]
    public bool IsDataPointHovered { get; set; }

    [Parameter]
    public string? XAxisTitle { get; set; }

    [Parameter]
    public string? YAxisTitle { get; set; }

    [Parameter]
    public double XAxisLabelRotation { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public EventCallback<ElementReference> ElementRefChanged { get; set; }
}
