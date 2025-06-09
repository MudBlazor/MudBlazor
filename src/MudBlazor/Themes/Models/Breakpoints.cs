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
        /// Breakpoint value for extra small screens (xs).
        /// Default value is "0px".
        /// </summary>
        public string xs { get; set; } = "0px";

        /// <summary>
        /// Breakpoint value for small screens (sm).
        /// Default value is "600px".
        /// </summary>
        public string sm { get; set; } = "600px";

        /// <summary>
        /// Breakpoint value for medium screens (md).
        /// Default value is "960px".
        /// </summary>
        public string md { get; set; } = "960px";

        /// <summary>
        /// Breakpoint value for large screens (lg).
        /// Default value is "1280px".
        /// </summary>
        public string lg { get; set; } = "1280px";

        /// <summary>
        /// Breakpoint value for extra large screens (xl).
        /// Default value is "1920px".
        /// </summary>
        public string xl { get; set; } = "1920px";

        /// <summary>
        /// Breakpoint value for extra extra large screens (xxl).
        /// Default value is "2560px".
        /// </summary>
        public string xxl { get; set; } = "2560px";

        // ReSharper restore InconsistentNaming
    }
}
