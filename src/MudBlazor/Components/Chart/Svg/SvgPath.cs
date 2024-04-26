
namespace MudBlazor.Charts.SVG.Models
{
    /// <summary>
    /// Represents an arbitrary SVG path.
    /// </summary>
    public class SvgPath
    {
        /// <summary>
        /// Gets or sets the position of this path within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the SVG path to draw.
        /// </summary>
        public string Data { get; set; }
    }
}
