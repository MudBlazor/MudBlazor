using System;
using MudBlazor.Extensions;

namespace MudBlazor
{
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
        public string ToString(Converter<DateTime?, string> converter)
        {
            if (Start == null || End == null)
                return string.Empty;

            return RangeConverter<DateTime>.Join(converter.Set(Start.Value), converter.Set(End.Value));
        }

        /// <summary>
        /// Formats this range as an ISO 8601 string.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public string ToIsoDateString()
        {
            if (Start == null || End == null)
                return string.Empty;

            return RangeConverter<DateTime>.Join(Start.ToIsoDateString(), End.ToIsoDateString());
        }

        /// <summary>
        /// Parses the specified string value into a date range.
        /// </summary>
        /// <param name="value">A string with both the start and end dates.</param>
        /// <param name="converter">The converter for parsing string values.</param>
        /// <param name="date">The result of the parse.</param>
        /// <returns><c>true</c> if the string was successfully interpreted as a date.</returns>
        public static bool TryParse(string value, Converter<DateTime?, string> converter, out DateRange date)
        {
            date = null;

            if (!RangeConverter<DateTime>.Split(value, out var start, out var end))
                return false;

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
        public static bool TryParse(string start, string end, Converter<DateTime?, string> converter, out DateRange date)
        {
            date = null;

            var endDate = converter.Get(end);
            if (converter.GetError)
                return false;

            var startDate = converter.Get(start);
            if (converter.GetError)
                return false;

            date = new DateRange(startDate, endDate);
            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Start, End);

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as DateRange);

        /// <inheritdoc />
        public bool Equals(DateRange other) => other != null && Start == other.Start && End == other.End;

        public static bool operator ==(DateRange dateRange1, DateRange dateRange2)
        {
            if (ReferenceEquals(dateRange1, dateRange2))
                return true;
            if (dateRange1 is null || dateRange2 is null)
                return false;

            return dateRange1.Equals(dateRange2);
        }

        public static bool operator !=(DateRange dateRange1, DateRange dateRange2) => !(dateRange1 == dateRange2);
    }
}
