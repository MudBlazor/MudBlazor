using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

#nullable enable
namespace MudBlazor.Charts;

public partial class ChartTooltip : ComponentBase
{
    private double _boxWidth = 40;
    private ElementReference? _hoverTextTitle = null;

    [Inject]
    protected IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The title of the tooltip.
    /// </summary>
    [Parameter, EditorRequired]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The subtitle of the tooltip.
    /// </summary>
    /// <remarks>
    /// When empty, the subtitle is not displayed.
    /// </remarks>
    [Parameter]
    public string Subtitle { get; set; } = string.Empty;

    /// <summary>
    /// The X coordinate of the tooltip anchor.
    /// </summary>
    [Parameter, EditorRequired]
    public double X { get; set; }

    /// <summary>
    /// The Y coordinate of the tooltip anchor.
    /// </summary>
    [Parameter, EditorRequired]
    public double Y { get; set; }

    /// <summary>
    /// The color of the tooltip.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"darkgrey"</c>.
    /// </remarks>
    [Parameter]
    public string Color { get; set; } = "darkgrey";

    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(BBox))]
    public ChartTooltip()
    {
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            // Uses interop to get the bounding box of the title text to determine the width of the tooltip box
            var bboxTitle = await JsRuntime.InvokeAsync<BBox>("mudGetSvgBBox", _hoverTextTitle);

            _boxWidth = Math.Max(bboxTitle?.Width ?? 0, 30) + 10; // Minimum width for the text of 30px with 10px padding (5px each side)

            StateHasChanged();
        }
    }

    private sealed class BBox
    {
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
