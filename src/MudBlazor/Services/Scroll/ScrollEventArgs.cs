using MudBlazor.Interop;


namespace MudBlazor
{
    /// <summary>
    /// Payload for scroll events emitted by MudBlazor scroll listeners.
    /// </summary>
    /// <remarks>
    /// Components and services use this to react to scroll position and element geometry without extra JS interop calls, keeping UI behavior responsive.
    /// </remarks>
    public class ScrollEventArgs : EventArgs
    {
        /// <summary>
        /// The BoundingClientRect for the first child of the scrolled element.
        /// </summary>
        public BoundingClientRect? FirstChildBoundingClientRect { get; set; }

        /// <summary>
        /// The ScrollTop property gets or sets the number of pixels that an element's content is scrolled vertically.
        /// </summary>
        public double ScrollTop { get; set; }

        /// <summary>
        /// The ScrollLeft property gets or sets the number of pixels that an element's content is scrolled from its left edge.
        /// </summary>
        public double ScrollLeft { get; set; }

        /// <summary>
        /// The ScrollHeight  property is a measurement of the height of an element's content, including content not visible on the screen due to overflow.
        /// </summary>
        public double ScrollHeight { get; set; }

        /// <summary>
        /// The ScrollWidth property is a measurement of the width of an element's content, including content not visible on the screen due to overflow.
        /// </summary>
        public double ScrollWidth { get; set; }

        /// <summary>
        /// Inner height of the element in px including padding, margin, etc.
        /// </summary>
        public double ClientHeight { get; set; }

        /// <summary>
        /// Inner width of the element in px including padding, margin, etc.
        /// </summary>
        public double ClientWidth { get; set; }

        /// <summary>
        /// Node name of the scrolled element.
        /// </summary>
        public string? NodeName { get; set; }
    }
}
