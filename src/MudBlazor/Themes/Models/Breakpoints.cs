using System.Diagnostics.CodeAnalysis;

namespace MudBlazor
{
#nullable enable
#pragma warning disable IDE1006 // must being with upper case
    /// <summary>
    /// Represents the breakpoints for responsive design.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Breakpoints
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// The breakpoint value for extra small screens (xs).
        /// Defaults to <c>0px</c>.
        /// </summary>
        public string xs { get; set; } = "0px";

        /// <summary>
        /// The breakpoint value for small screens (sm).
        /// Defaults to <c>600px</c>.
        /// </summary>
        public string sm { get; set; } = "600px";

        /// <summary>
        /// The breakpoint value for medium screens (md).
        /// Defaults to <c>960px</c>.
        /// </summary>
        public string md { get; set; } = "960px";

        /// <summary>
        /// The breakpoint value for large screens (lg).
        /// Defaults to <c>1280px</c>.
        /// </summary>
        public string lg { get; set; } = "1280px";

        /// <summary>
        /// The breakpoint value for extra large screens (xl).
        /// Defaults to <c>1920px</c>.
        /// </summary>
        public string xl { get; set; } = "1920px";

        /// <summary>
        /// The breakpoint value for extra extra large screens (xxl).
        /// Defaults to <c>2560px</c>.
        /// </summary>
        public string xxl { get; set; } = "2560px";

        // ReSharper restore InconsistentNaming
    }
}
