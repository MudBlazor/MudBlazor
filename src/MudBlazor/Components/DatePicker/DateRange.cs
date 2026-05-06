using System.Diagnostics.CodeAnalysis;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Represents a date range used by a <see cref="MudDateRangePicker{T}"/>.
/// </summary>
/// <typeparam name="T">The date type bound by the range. Supported: <see cref="DateTime"/>, <see cref="DateTime"/>?, <see cref="DateOnly"/>, <see cref="DateOnly"/>?, <see cref="DateTimeOffset"/>, <see cref="DateTimeOffset"/>?.</typeparam>
public sealed class DateRange<T> : Range<T>, IEquatable<DateRange<T>?>
{
    private static readonly Type _underlyingType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public DateRange() : this(default, default)
    {
    }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="start">The earliest date.</param>
    /// <param name="end">The most recent date.</param>
    public DateRange(T? start, T? end) : base(start, end)
    {
    }

    /// <summary>
    /// Formats this range as a string.
    /// </summary>
    /// <param name="converter">The converter used to convert to a <c>string</c>.</param>
    /// <returns>The formatted string.</returns>
    public string ToString(IConverter<T?, string?> converter)
    {
        if (Start is null || End is null)
        {
            return string.Empty;
        }

        return RangeUtility.Join(converter.Convert(Start), converter.Convert(End));
    }

    /// <summary>
    /// Formats this range as an ISO 8601 string.
    /// </summary>
    /// <returns>The formatted string.</returns>
    public string ToIsoDateString()
    {
        var startDt = AsDateTime(Start);
        var endDt = AsDateTime(End);
        if (startDt is null || endDt is null)
        {
            return string.Empty;
        }

        return RangeUtility.Join(startDt.ToIsoDateString(), endDt.ToIsoDateString());
    }

    /// <summary>
    /// Parses the specified string value into a date range.
    /// </summary>
    /// <param name="value">A string with both the start and end dates.</param>
    /// <param name="converter">The converter for parsing string values.</param>
    /// <param name="date">The result of the parse.</param>
    /// <returns><c>true</c> if the string was successfully interpreted as a date.</returns>
    public static bool TryParse(string? value, IConverter<T?, string?> converter, [NotNullWhen(true)] out DateRange<T>? date)
    {
        if (!RangeUtility.Split(value, out var start, out var end))
        {
            date = null;
            return false;
        }

        return TryParse(start, end, converter, out date);
    }

    /// <summary>
    /// Parses the specified string value into a date range.
    /// </summary>
    /// <param name="start">The minimum date to parse.</param>
    /// <param name="end">The maximum date to parse.</param>
    /// <param name="converter">The converter for parsing string values.</param>
    /// <param name="date">The result of the parse.</param>
    /// <returns><c>true</c> if the string was successfully interpreted as a date.</returns>
    public static bool TryParse(string? start, string? end, IConverter<T?, string?> converter, [NotNullWhen(true)] out DateRange<T>? date)
    {
        var endDate = converter.TryConvertBack(end);
        if (!endDate.Success)
        {
            date = null;
            return false;
        }

        var startDate = converter.TryConvertBack(start);
        if (!startDate.Success)
        {
            date = null;
            return false;
        }

        date = new DateRange<T>(startDate.Value, endDate.Value);
        return true;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is DateRange<T> dateRange && Equals(dateRange);

    /// <inheritdoc />
    public bool Equals(DateRange<T>? other)
        => other is not null
        && EqualityComparer<T?>.Default.Equals(other.Start, Start)
        && EqualityComparer<T?>.Default.Equals(other.End, End);

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Start, End);

    public static bool operator ==(DateRange<T>? dateRange1, DateRange<T>? dateRange2)
    {
        if (ReferenceEquals(dateRange1, dateRange2))
            return true;
        if (dateRange1 is null || dateRange2 is null)
            return false;

        return dateRange1.Equals(dateRange2);
    }

    public static bool operator !=(DateRange<T>? dateRange1, DateRange<T>? dateRange2) => !(dateRange1 == dateRange2);

    /// <summary>
    /// Convert <typeparamref name="T"/> to a <see cref="DateTime"/> for ISO formatting and other date helpers.
    /// Mirrors <c>MudBaseDatePicker&lt;T&gt;.ToDateTime</c>.
    /// </summary>
    private static DateTime? AsDateTime(T? value)
    {
        if (value is null) return null;
        if (_underlyingType == typeof(DateTime)) return (DateTime)(object)value;
        if (_underlyingType == typeof(DateOnly)) return ((DateOnly)(object)value).ToDateTime(TimeOnly.MinValue);
        if (_underlyingType == typeof(DateTimeOffset)) return ((DateTimeOffset)(object)value).DateTime;
        throw new InvalidOperationException($"DateRange does not support T = {typeof(T)}. Use DateTime, DateOnly, or DateTimeOffset (or their nullable variants).");
    }
}
