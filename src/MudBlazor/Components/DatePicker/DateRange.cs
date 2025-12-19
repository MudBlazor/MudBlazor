using System.Diagnostics.CodeAnalysis;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a date range used by a <see cref="MudDatePicker"/>.
    /// </summary>
    public class DateRange : Range<DateTime?>, IEquatable<DateRange>
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public DateRange() : base(null, null)
        {
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="start">The earliest date.</param>
        /// <param name="end">The most recent date.</param>
        public DateRange(DateTime? start, DateTime? end) : base(start, end)
        {
        }

        /// <summary>
        /// Formats this range as a string.
        /// </summary>
        /// <param name="converter">The converter used to convert to a <c>string</c>.</param>
        /// <returns>The formatted string.</returns>
        public string ToString(IConverter<DateTime?, string?> converter)
        {
            if (Start is null || End is null)
            {
                return string.Empty;
            }

            return RangeUtility.Join(converter.Convert(Start.Value), converter.Convert(End.Value));
        }

        /// <summary>
        /// Formats this range as an ISO 8601 string.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string ToIsoDateString()
        {
            if (Start is null || End is null)
            {
                return string.Empty;
            }

            return RangeUtility.Join(Start.ToIsoDateString(), End.ToIsoDateString());
        }

        /// <summary>
        /// Parses the specified string value into a date range.
        /// </summary>
        /// <param name="value">A string with both the start and end dates.</param>
        /// <param name="converter">The converter for parsing string values.</param>
        /// <param name="date">The result of the parse.</param>
        /// <returns><c>true</c> if the string was successfully interpreted as a date.</returns>
        public static bool TryParse(string? value, IConverter<DateTime?, string?> converter, [NotNullWhen(true)] out DateRange? date)
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
        public static bool TryParse(string? start, string? end, IConverter<DateTime?, string?> converter, [NotNullWhen(true)] out DateRange? date)
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

            date = new DateRange(startDate.Value, endDate.Value);
            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Start, End);

        /// <inheritdoc />
        public override bool Equals(object? obj) => Equals(obj as DateRange);

        /// <inheritdoc />
        public bool Equals(DateRange? other) => other is not null && Start == other.Start && End == other.End;

        public static bool operator ==(DateRange? dateRange1, DateRange? dateRange2)
        {
            if (ReferenceEquals(dateRange1, dateRange2))
                return true;
            if (dateRange1 is null || dateRange2 is null)
                return false;

            return dateRange1.Equals(dateRange2);
        }

        public static bool operator !=(DateRange? dateRange1, DateRange? dateRange2) => !(dateRange1 == dateRange2);
    }
}
