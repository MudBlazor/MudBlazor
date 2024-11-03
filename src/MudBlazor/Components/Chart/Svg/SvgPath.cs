using System.Diagnostics;

namespace MudBlazor.Charts.SVG.Models
{
    /// <summary>
    /// Represents an arbitrary SVG path.
    /// </summary>
    public class SvgPath
    {
        /// <summary>
        /// The position of this path within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The SVG path to draw.
        /// </summary>
        public string Data { get; set; }
    }
}
