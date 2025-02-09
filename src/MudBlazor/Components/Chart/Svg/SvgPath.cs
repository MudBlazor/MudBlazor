#nullable enable
namespace MudBlazor
{
    /// <summary>
    /// Represents an arbitrary SVG path.
    /// </summary>
    internal class SvgPath
    {
        /// <summary>
        /// The position of this path within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The SVG path to draw.
        /// </summary>
        public string? Data { get; set; }
    }
}
