using System.Diagnostics;

namespace MudBlazor.Charts.SVG.Models
{
    /// <summary>
    /// Represents a circular shape drawn as an SVG path.
    /// </summary>
    [DebuggerDisplay("Index={Index}, CX={CX}, CY={CY}, Radius={Radius}")]
    public class SvgCircle
    {
        /// <summary>
        /// Gets or sets the position of this path within a list.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the horizontal position of the center of the circle.
        /// </summary>
        public double CX { get; set; }

        /// <summary>
        /// Gets or sets the vertical position of the center of the circle.
        /// </summary>
        public double CY { get; set; }

        /// <summary>
        /// Gets or sets the distance from the center of the circle to the edge.
        /// </summary>
        public double Radius { get; set; }

        /// <summary>
        /// Gets or sets the pattern of dashes and gaps used to paint the outline of the circle.
        /// </summary>
        public string StrokeDashArray { get; set; }

        /// <summary>
        /// Gets or sets an offset applied to the <see cref="StrokeDashArray"/>.
        /// </summary>
        public double StrokeDashOffset { get; set; }
    }
}
