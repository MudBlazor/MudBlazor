using System.Diagnostics;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    /// <summary>
    /// A single legend entry describing one chart series, with its label, value, and visibility toggle.
    /// </summary>
    [DebuggerDisplay("{Index} = {Labels}")]
    public class SvgLegend
    {
        /// <summary>
        /// The position of this path within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The series labels to display.
        /// </summary>
        public string Labels { get; set; } = string.Empty;

        /// <summary>
        /// The data values to display.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Whether the legend is displayed.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Occurs when the <see cref="Visible"/> property has changed.
        /// </summary>
        public EventCallback<SvgLegend> OnVisibilityChanged { get; set; }

        /// <summary>
        /// Toggles the visibility of this legend.
        /// </summary>
        public async Task HandleCheckboxChangeAsync()
        {
            Visible = !Visible;
            await OnVisibilityChanged.InvokeAsync(this);
        }
    }
}
