using System.Globalization;

namespace MudBlazor.Extensions
{
    internal static class NumberExtensions
    {
        internal static string ToPixels(this double? value) => value?.ToString(CultureInfo.InvariantCulture) + "px";

        internal static string ToPixels(this double value) => value.ToString(CultureInfo.InvariantCulture) + "px";
    }
}
