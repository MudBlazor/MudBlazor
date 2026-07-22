using System.Diagnostics;

namespace MudBlazor.Charts.SVG.Models
{
    /// <summary>
    /// Represents a piece of text as an SVG path.
    /// </summary>
    [DebuggerDisplay("X={X}, Y={Y}, Value={Value}")]
    public class SvgText
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
        public string Value { get; set; }
    }
}
