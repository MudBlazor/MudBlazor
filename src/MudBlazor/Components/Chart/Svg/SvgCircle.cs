using System.Diagnostics;

namespace MudBlazor
{
    /// <summary>
    /// Circle shape rendered as SVG, used to draw points and markers in a chart.
    /// </summary>
    [DebuggerDisplay("{Index} = {CX},{CY}, R={Radius}")]
    public sealed class SvgCircle : SvgPath
    {
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
        public string? StrokeDashArray { get; set; }

        /// <summary>
        /// The offset applied to the <see cref="StrokeDashArray"/>.
        /// </summary>
        public double StrokeDashOffset { get; set; }
    }
}
