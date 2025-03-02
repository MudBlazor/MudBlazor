using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MudBlazor.Charts
{
    public partial class ChartTooltip
    {
        /// <summary>
        /// The title of the tooltip.
        /// </summary>
        [Parameter, EditorRequired] public string Title { get; set; } = string.Empty;

        /// <summary>
        /// The subtitle of the tooltip.
        /// </summary>
        /// <remarks>
        /// When empty, the subtitle is not displayed.
        /// </remarks>
        [Parameter] public string Subtitle { get; set; } = string.Empty;

        /// <summary>
        /// The X coordinate of the tooltip anchor.
        /// </summary>
        [Parameter, EditorRequired] public double X { get; set; }

        /// <summary>
        /// The Y coordinate of the tooltip anchor.
        /// </summary>
        [Parameter, EditorRequired] public double Y { get; set; }

        /// <summary>
        /// The color of the tooltip.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>"darkgrey"</c>.
        /// </remarks>
        [Parameter] public string Color { get; set; } = "darkgrey";

        private ElementReference? _hoverTextTitle = null;
        private double _boxWidth = -1;

        private sealed class BBox
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Uses interop to get the bounding box of the title text to determine the width of the tooltip box
                var bboxTitle = await JSRuntime.InvokeAsync<BBox>("mudGetSvgBBox", _hoverTextTitle);

                _boxWidth = Math.Max(bboxTitle.Width, 30) + 10; // Minimum width for the text of 30px with 10px padding (5px each side)

                StateHasChanged();
            }
        }
    }
}
