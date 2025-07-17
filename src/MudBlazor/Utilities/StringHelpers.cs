using System.Globalization;

namespace MudBlazor.Utilities;

#nullable enable
internal static partial class StringHelpers
{
    /// <summary>
    /// Converts a double value to its string representation, rounded to 4 decimal places.
    /// </summary>
    /// <param name="value">The double value to convert.</param>
    /// <param name="format">An optional format string.</param>
    /// <param name="culture">An optional culture.</param>
    /// <returns>The string representation of the double value.</returns>
    public static string ToS(double value, string? format = null, CultureInfo? culture = null)
    {
        culture ??= CultureInfo.InvariantCulture;

        var result = string.IsNullOrEmpty(format)
            ? Math.Round(value, 4).ToString(culture)
            : Math.Round(value, 4).ToString(format, culture);

        return result;
    }
}
