using System.Globalization;

namespace MudBlazor.Extensions
{
    internal static class NumberExtensions
    {
        /// <summary>
        /// Converts double to string using dot as decimal separator independently of the culture and ads "px"
        /// </summary>
        internal static string ToPixels(this double? value) => value?.ToString(CultureInfo.InvariantCulture) + "px";

        /// <summary>
        /// Converts double to string using dot as decimal separator independently of the culture and ads "px"
        /// </summary>
        internal static string ToPixels(this double value) => value.ToString(CultureInfo.InvariantCulture) + "px";
    }
}
