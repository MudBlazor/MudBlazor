// Copyright (c) MudBlazor

using System;
using System.Globalization;

namespace MudBlazor.Extensions
{
#nullable enable
    public static class DateTimeExtensions
    {
        public static string ToIsoDateString(this DateTime self)
        {
            return $"{self.Year:D4}-{self.Month:D2}-{self.Day:D2}";
        }

        public static string? ToIsoDateString(this DateTime? self)
        {
            if (self is null)
            {
                return null;
            }

            return $"{self.Value.Year:D4}-{self.Value.Month:D2}-{self.Value.Day:D2}";
        }

        public static DateTime StartOfMonth(this DateTime self, CultureInfo culture)
        {
            var month = culture.Calendar.GetMonth(self);
            var year = culture.Calendar.GetYear(self);

            return culture.Calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);
        }

        public static DateTime EndOfMonth(this DateTime self, CultureInfo culture)
        {
            var month = culture.Calendar.GetMonth(self);
            var year = culture.Calendar.GetYear(self);
            var days = culture.Calendar.GetDaysInMonth(year, month);

            return culture.Calendar.ToDateTime(year, month, days, 0, 0, 0, 0);
        }

        public static DateTime StartOfWeek(this DateTime self, DayOfWeek firstDayOfWeek)
        {
            var diff = (7 + (self.DayOfWeek - firstDayOfWeek)) % 7;
            if (self.Year == 1 && self.Month == 1 && (self.Day - diff) < 1)
            {
                return self.Date;
            }

            return self.AddDays(-1 * diff).Date;
        }
    }
}
