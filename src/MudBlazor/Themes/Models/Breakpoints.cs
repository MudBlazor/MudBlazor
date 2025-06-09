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
        /// Gets or sets the breakpoint value for extra small screens (xs).
        /// Default value is "0px".
        /// </summary>
        public string xs { get; set; } = "0px";

        /// <summary>
        /// Gets or sets the breakpoint value for small screens (sm).
        /// Default value is "600px".
        /// </summary>
        public string sm { get; set; } = "600px";

        /// <summary>
        /// Gets or sets the breakpoint value for medium screens (md).
        /// Default value is "960px".
        /// </summary>
        public string md { get; set; } = "960px";

        /// <summary>
        /// Gets or sets the breakpoint value for large screens (lg).
        /// Default value is "1280px".
        /// </summary>
        public string lg { get; set; } = "1280px";

        /// <summary>
        /// Gets or sets the breakpoint value for extra large screens (xl).
        /// Default value is "1920px".
        /// </summary>
        public string xl { get; set; } = "1920px";

        /// <summary>
        /// Gets or sets the breakpoint value for extra extra large screens (xxl).
        /// Default value is "2560px".
        /// </summary>
        public string xxl { get; set; } = "2560px";

        // ReSharper restore InconsistentNaming
    }
}
