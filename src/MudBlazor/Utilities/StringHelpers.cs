using System.Globalization;

namespace MudBlazor.Utilities;

internal static partial class StringHelpers
{
    /// <summary>
    /// Converts a double value to its string representation, rounded to 4 decimal places.
    /// </summary>
    /// <param name="value">The double value to convert.</param>
    /// <param name="format">An optional format string.</param>
    /// <returns>The string representation of the double value.</returns>
    public static string ToS(double value, string? format = null)
    {
        return string.IsNullOrEmpty(format)
            ? Math.Round(value, 4).ToString(CultureInfo.InvariantCulture)
            : Math.Round(value, 4).ToString(format);
    }

    /// <summary>
    /// Converts a double value to its string representation, rounded to 4 decimal places.
    /// </summary>
    /// <param name="value">The double value to convert</param>
    /// <returns>
    /// The string representation of the double value. <br/>
    /// </returns>
    public static string ToStr(this double value)
    {
        return ToS(value, null);
    }

    /// <summary>
    /// Parses a CSS pixel length such as <c>640.5px</c> with the invariant culture.
    /// </summary>
    /// <param name="value">The length to parse.</param>
    /// <param name="pixels">The parsed number of pixels, or <c>0</c> when parsing fails.</param>
    /// <returns><c>true</c> when <paramref name="value"/> is a number followed by <c>px</c>.</returns>
    public static bool TryParsePixels(string value, out double pixels)
    {
        pixels = 0;
        return value.EndsWith("px", StringComparison.Ordinal)
            && double.TryParse(value.AsSpan(0, value.Length - 2), NumberStyles.Float, CultureInfo.InvariantCulture, out pixels);
    }
}
