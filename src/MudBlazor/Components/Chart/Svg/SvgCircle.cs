using System.Diagnostics;

namespace MudBlazor.Charts.SVG.Models
{
    /// <summary>
    /// Represents a circular shape drawn as an SVG path.
    /// </summary>
    [DebuggerDisplay("{Index} = {CX},{CY}, R={Radius}")]
    public class SvgCircle
    {
        /// <summary>
        /// The position of this path within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The horizontal position of the center of the circle.
        /// </summary>
        public double CX { get; set; }

        /// <summary>
        /// The vertical position of the center of the circle.
        /// </summary>
        public double CY { get; set; }

        /// <summary>
        /// The distance from the center of the circle to the edge.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// The pattern of dashes and gaps used to paint the outline of the circle.
        /// </summary>
        public string StrokeDashArray { get; set; }

        /// <summary>
        /// The offset applied to the <see cref="StrokeDashArray"/>.
        /// </summary>
        public double StrokeDashOffset { get; set; }
    }
}
