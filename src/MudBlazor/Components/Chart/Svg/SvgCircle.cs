using System.Diagnostics;

namespace MudBlazor
{
    /// <summary>
    /// Circle shape rendered as SVG, used to draw points and markers in a chart.
    /// </summary>
    [DebuggerDisplay("{Index} = {CX},{CY}, R={Radius}")]
    public sealed class SvgCircle : SvgPath
    {
        private double _cx;
        private double _cy;
        private string? _cxText;
        private string? _cyText;

        /// <summary>
        /// The horizontal position of the center of the circle.
        /// </summary>
        public double CX
        {
            get => _cx;
            set
            {
                _cx = value;
                _cxText = null;
            }
        }

        /// <summary>
        /// The vertical position of the center of the circle.
        /// </summary>
        public double CY
        {
            get => _cy;
            set
            {
                _cy = value;
                _cyText = null;
            }
        }

        // A chart draws its points again on every render, hover included, so each position is written out once and kept.
        internal string CXText => _cxText ??= _cx.ToStr();

        internal string CYText => _cyText ??= _cy.ToStr();

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
