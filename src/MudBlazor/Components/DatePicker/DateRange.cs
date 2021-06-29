﻿using System;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class DateRange : Range<DateTime?>
    {
        public DateRange() : base(null, null)
        {
        }

        public DateRange(DateTime? start, DateTime? end) : base(start, end)
        {
        }

        public string ToString(Converter<DateTime?, string> converter)
        {
            return Start is null || End is null ? string.Empty : RangeConverter<DateTime>.Join(converter.Set(Start.Value), converter.Set(End.Value));
        }

        public string ToIsoDateString()
        {
            return Start is null || End is null ? string.Empty : RangeConverter<DateTime>.Join(Start.ToIsoDateString(), End.ToIsoDateString());
        }

        public static bool TryParse(string value, Converter<DateTime?, string> converter, out DateRange date)
        {
            date = null;

            if (!RangeConverter<DateTime>.Split(value, out string start, out string end))
                return false;

            return TryParse(start, end, converter, out date);
        }

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
    }
}
