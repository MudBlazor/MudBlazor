using System;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class DateRange : Range<DateTime?>
    {
        public DateRange(DateTime? start, DateTime? end) : base(start, end)
        {

        }

        public string ToString(string dateFormat)
        {
            if (Start == null || End == null)
                return "";

            return RangeConverter<DateTime>.Join(Start.Value.ToString(dateFormat), End.Value.ToString(dateFormat));
        }

        public string ToIsoDateString()
        {
            return RangeConverter<DateTime>.Join(Start.ToIsoDateString(), End.ToIsoDateString());
        }

        public static bool TryParse(string value, out DateRange date)
        {
            date = null;

            if (!RangeConverter<DateTime>.Split(value, out string start, out string end))
                return false;

            return TryParse(start, end, out date);
        }

        public static bool TryParse(string start, string end, out DateRange date)
        {
            date = null;

            if (!DateTime.TryParse(start, out DateTime startDate))
                return false;

            if (!DateTime.TryParse(end, out DateTime endDate))
                return false;

            date = new DateRange(startDate, endDate);
            return true;
        }
    }
}
