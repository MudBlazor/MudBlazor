// Copyright (c) MudBlazor

using System;

// ReSharper disable once CheckNamespace
internal static class DateTimeExtensions
{
    public static string ToIsoDateString(this DateTime self)
    {
        return $"{self.Year:D4}-{self.Month:D2}-{self.Day:D2}";
    }

    public static string ToIsoDateString(this DateTime? self)
    {
        if(self==null)
            return null;
        return $"{self.Value.Year:D4}-{self.Value.Month:D2}-{self.Value.Day:D2}";
    }

    public static DateTime StartOfMonth(this DateTime self)
    {
        return new DateTime(self.Year, self.Month, 1);
    }

    public static DateTime EndOfMonth(this DateTime self)
    {
        return new DateTime(self.Year, self.Month, DateTime.DaysInMonth(self.Year, self.Month));
    }

    public static DateTime StartOfWeek(this DateTime self, DayOfWeek firstDayOfWeek)
    {
        int diff = (7 + (self.DayOfWeek - firstDayOfWeek)) % 7;
        return self.AddDays(-1 * diff).Date;
    }
    
}
