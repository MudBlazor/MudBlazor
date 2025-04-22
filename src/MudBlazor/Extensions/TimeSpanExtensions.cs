// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// ReSharper disable once CheckNamespace
namespace MudBlazor;

#nullable enable
internal static class TimeSpanExtensions
{
    public static string ToIsoString(this TimeSpan self, bool seconds = false, bool ms = false)
    {
        if (!seconds)
        {
            return $"{self.Hours:D2}:{self.Minutes:D2}";
        }

        if (!ms)
        {
            return $"{self.Hours:D2}:{self.Minutes:D2}-{self.Seconds:D2}";
        }

        return $"{self.Hours:D2}:{self.Minutes:D2}-{self.Seconds:D2},{self.Milliseconds}";
    }

    public static int ToAmPmHour(this TimeSpan time)
    {
        var h = time.Hours % 12;
        if (h == 0)
        {
            h = 12;
        }

        return h;
    }
}
