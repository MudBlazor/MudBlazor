// Copyright (c) MudBlazor

using System;

// ReSharper disable once CheckNamespace
internal static class TimeSpanExtensions
{
    public static string ToIsoString(this TimeSpan self, bool seconds = false, bool ms = false)
    {
        if (!seconds)
            return $"{self.Hours:D2}:{self.Minutes:D2}";
        if (!ms)
            return $"{self.Hours:D2}:{self.Minutes:D2}-{self.Seconds:D2}";
        return $"{self.Hours:D2}:{self.Minutes:D2}-{self.Seconds:D2},{self.Milliseconds}";
    }

    public static string ToIsoString(this TimeSpan? self, bool seconds = false, bool ms = false)
    {
        if (self == null)
            return null;
        return self.Value.ToIsoString(seconds, ms);
    }

    public static string ToAmPmString(this TimeSpan time, bool seconds = false)
    {
        var pm = time.Hours >= 12;
        var h = time.Hours % 12;
        if (h == 0)
            h = 12;
        if (!seconds)
            return $"{h:D2}:{time.Minutes:D2} {(pm ? "PM" : "AM")}";
        return $"{h:D2}:{time.Minutes:D2}{time.Seconds:D2} {(pm ? "PM" : "AM")}";
    }

    public static string ToAmPmString(this TimeSpan? self, bool seconds = false)
    {
        if (self == null)
            return null;
        return self.Value.ToAmPmString(seconds);
    }

    public static int ToAmPmHour(this TimeSpan time)
    {
        var h = time.Hours % 12;
        if (h == 0)
            h = 12;
        return h;
    }
}
