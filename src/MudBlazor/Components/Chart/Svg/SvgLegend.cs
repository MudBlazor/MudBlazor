using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Charts.SVG.Models
{
    /// <summary>
    /// Represents a series of series labels as an SVG path.
    /// </summary>
    public class SvgLegend
    {
        /// <summary>
        /// Gets or sets the position of this path within a list.
        /// </summary>
        public int Index { get; set; }
        public string Labels { get; set; }
        public string Data { get; set; }
        public bool IsVisible { get; set; } = true;
        public EventCallback<SvgLegend> OnVisibilityChanged { get; set; }

        public async Task HandleCheckboxChangeAsync()
        {
            IsVisible = !IsVisible;
            await OnVisibilityChanged.InvokeAsync(this);
        }
    }
}
