// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor.Charts;

public record MouseOverArgs(MouseEventArgs MouseEvent, SvgPath Path);

public partial class BaseRadialChart<TChartOptions> : MudComponentBase where TChartOptions : IRadialChartOptions, new()
{
    private ElementReference _svgRef;

    [CascadingParameter]
    private MudChart MudChartParent { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public double Radius { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public string ChartClass { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Behavior)]
    public SvgPath HoveredSegment { get; set; }

    [Parameter]
    [EditorRequired]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<SvgPath> Paths { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public List<SvgLegend> Legends { get; set; } = [];

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public (string title, string subtitle) TooltipFormat { get; set; }

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
    public RenderFragment<(SvgPath Segment, string Color)> TooltipTemplate { get; set; }

    [Parameter]
    [Category(CategoryTypes.Chart.Appearance)]
    public Func<SvgPath, (double X, double Y)> TooltipPositionFunc { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
            await ElementRefChanged.InvokeAsync(_svgRef);
    }

    private string GetColor(int index)
    {
        var palette = MudChartParent?.ChartOptions?.ChartPalette;

        if (palette is null || palette.Length == 0)
            return string.Empty;

        return palette.GetValue(index % palette.Length)?.ToString() ?? string.Empty;
    }
}
