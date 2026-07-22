using System.Diagnostics;

namespace MudBlazor
{
    /// <summary>
    /// A text label rendered as an SVG element in a chart, positioned by X and Y coordinates.
    /// </summary>
    [DebuggerDisplay("X={X}, Y={Y}, Value={Value}")]
    public sealed class SvgText
    {
        /// <summary>
        /// The horizontal position of the text.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// The vertical position of the text.
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// The text to display.
        /// </summary>
        public string? Value { get; set; }
    }
}
