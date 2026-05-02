// Copyright (c) MudBlazor

using System;
using System.Globalization;

namespace MudBlazor.Extensions;

#nullable enable
public static class DateTimeOffsetExtensions
{
    public static DateTimeOffset AddMonths(this DateTimeOffset self, int months, CultureInfo culture)
    {
        if (months <= 0)
        {
            return self;
        }

        return new DateTimeOffset(culture.Calendar.AddMonths(self.DateTime, months), self.Offset);
    }

    public static DateTimeOffset StartOfMonth(this DateTimeOffset self, CultureInfo culture)
    {
        var month = culture.Calendar.GetMonth(self.DateTime);
        var year = culture.Calendar.GetYear(self.DateTime);

        var dateTime = culture.Calendar.ToDateTime(year, month, 1, 0, 0, 0, 0);

        return new DateTimeOffset(dateTime, self.Offset);
    }

    public static DateTimeOffset EndOfMonth(this DateTimeOffset self, CultureInfo culture)
    {
        var month = culture.Calendar.GetMonth(self.DateTime);
        var year = culture.Calendar.GetYear(self.DateTime);
        var days = culture.Calendar.GetDaysInMonth(year, month);

        var dateTime = culture.Calendar.ToDateTime(year, month, days, 0, 0, 0, 0);
        return new DateTimeOffset(dateTime, self.Offset);
    }

    public static DateTimeOffset StartOfWeek(this DateTimeOffset self, DayOfWeek firstDayOfWeek)
    {
        var diff = (7 + (self.DayOfWeek - firstDayOfWeek)) % 7;
        if (self is { Year: 1, Month: 1 } && (self.Day - diff) < 1)
        {
            // Should the offset be preserved here?
            return new DateTimeOffset(self.Date, self.Offset);
        }

        // Should the offset be preserved here?
        return new DateTimeOffset(self.AddDays(-1 * diff).Date, self.Offset);
    }
}
