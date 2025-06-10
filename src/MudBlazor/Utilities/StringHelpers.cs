using System.Globalization;
using System.Text.RegularExpressions;

namespace MudBlazor.Utilities;

#nullable enable
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
    /// Replaces named tokens in a template string with values from a dictionary.
    /// </summary>
    /// <param name="template">The string template containing tokens, e.g., "Hello, {name}!".</param>
    /// <param name="values">A dictionary where the key is the token name and the value is the replacement object.</param>
    /// <returns>A formatted string with all tokens replaced.</returns>
    public static string ReplaceTokens(this string template, Dictionary<string, object> values)
    {
        if (string.IsNullOrEmpty(template) || values == null || values.Count == 0)
        {
            return template;
        }

        return _tokenRegex.Replace(template, match =>
        {
            var tokenName = match.Groups["name"].Value;

            if (values.TryGetValue(tokenName, out var value))
            {
                // Check if a format string was provided (e.g., ":N0").
                if (match.Groups["format"].Success)
                {
                    var formatString = match.Groups["format"].Value;

                    return string.Format(CultureInfo.CurrentCulture, $"{{0:{formatString}}}", value);
                }

                return value?.ToString() ?? string.Empty;
            }

            // If the token is not found in the dictionary, leave it as is in the template.
            return match.Value;
        });
    }
    private static readonly Regex _tokenRegex = CreateTokenRegex();

    [GeneratedRegex(@"\{(?<name>\w+)(:(?<format>[^}]+))?\}", RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
    private static partial Regex CreateTokenRegex();
}
