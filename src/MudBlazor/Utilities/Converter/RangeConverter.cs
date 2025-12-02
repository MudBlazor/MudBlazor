// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MudBlazor.Utilities.Converter;

#nullable enable
/// <summary>
/// Converter that serializes and deserializes a <see cref="Range{T}"/> to and from a string representation.
/// </summary>
/// <typeparam name="T">The element type of the range.</typeparam>
/// <remarks>
/// The string representation is in the form <c>[{start};{end}]</c>. Empty or null ranges are represented as an empty string.
/// This converter delegates element-level conversions to a <see cref="DefaultConverter{T}"/> instance and implements
/// <see cref="ICultureAwareConverter"/> so components (for example via <c>MudFormComponent.Converter</c>) can inject
/// the <see cref="Culture"/> and <see cref="Format"/> providers automatically.
/// </remarks>
public sealed class RangeConverter<T> : IReversibleConverter<Range<T>?, string?>, ICultureAwareConverter
{
    private readonly DefaultConverter<T> _inner;

    /// <summary>
    /// A function that provides the optional format string used by inner converters.
    /// When used as a component converter the host will supply this delegate automatically.
    /// </summary>
    public Func<string?> Format { get; set; } = () => null;

    /// <summary>
    /// A function that returns the <see cref="CultureInfo"/> used by inner converters.
    /// When used as a component converter the host will supply this delegate automatically.
    /// </summary>
    public Func<CultureInfo> Culture { get; set; } = () => CultureInfo.InvariantCulture;

    /// <summary>
    /// Initializes a new instance of <see cref="RangeConverter{T}"/> and configures the inner element converter.
    /// </summary>
    public RangeConverter()
    {
        _inner = new DefaultConverter<T>
        {
            Culture = Culture,
            Format = Format
        };
    }

    /// <inheritdoc />
    public string Convert(Range<T>? input)
    {
        if (input is null)
        {
            return string.Empty;
        }

        return Join(_inner.Convert(input.Start), _inner.Convert(input.End));
    }

    /// <inheritdoc />
    public Range<T>? ConvertBack(string? input)
    {
        if (!Split(input, out var startString, out var endString))
        {
            return null;
        }

        var startRange = _inner.ConvertBack(startString);
        var endRange = _inner.ConvertBack(endString);

        return new Range<T>(startRange, endRange);
    }

    /// <summary>
    /// Joins two element string values into the canonical range representation <c>[{start};{end}]</c>.
    /// </summary>
    /// <param name="valueStart">The string representation of the start value (maybe <c>null</c> or empty).</param>
    /// <param name="valueEnd">The string representation of the end value (maybe <c>null</c> or empty).</param>
    /// <returns>The joined range string or an empty string when both parts are empty.</returns>
    public static string Join(string? valueStart, string? valueEnd)
    {
        if (string.IsNullOrEmpty(valueStart) && string.IsNullOrEmpty(valueEnd))
        {
            return string.Empty;
        }

        return $"[{valueStart};{valueEnd}]";
    }

    /// <summary>
    /// Splits a canonical range string into its start and end parts.
    /// </summary>
    /// <param name="value">The range string to split (expected in the form <c>[{start};{end}]</c>).</param>
    /// <param name="valueStart">Output parameter that receives the start part on success; otherwise empty string.</param>
    /// <param name="valueEnd">Output parameter that receives the end part on success; otherwise empty string.</param>
    /// <returns><c>true</c> when <paramref name="value"/> is a valid canonical range and parts were extracted; otherwise <c>false</c>.</returns>
    public static bool Split(string? value, out string valueStart, out string valueEnd)
    {
        valueStart = valueEnd = string.Empty;

        if (string.IsNullOrEmpty(value) || value[0] != '[' || value[^1] != ']')
        {
            return false;
        }

        var idx = value.IndexOf(';');
        if (idx < 1)
        {
            return false;
        }

        valueStart = value[1..idx];
        valueEnd = value[(idx + 1)..^1];

        return true;
    }
}
