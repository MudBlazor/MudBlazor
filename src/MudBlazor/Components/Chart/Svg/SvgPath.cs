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

        /// <summary>
        /// The label text for on hover.
        /// </summary>
        public string LabelXValue { get; set; } = string.Empty;

        /// <summary>
        /// The label text for on hover.
        /// </summary>
        public string LabelYValue { get; set; } = string.Empty;

        /// <summary>
        /// The label X position for on hover.
        /// </summary>
        public double LabelX { get; set; }

        /// <summary>
        /// The label Y position for on hover.
        /// </summary>
        public double LabelY { get; set; }
    }
}
