using System;

namespace MudBlazor.Services;

#nullable enable
internal class DateOnlyConverter : IDateConverter<DateOnly>
{
    public DateTimeOffset? ConvertTo(DateOnly? date)
    {
        return date.HasValue ? new DateTimeOffset(date.Value.Year, date.Value.Month, date.Value.Day, 0, 0, 0, TimeSpan.Zero) : null;
    }

    public DateOnly? ConvertFrom(DateTimeOffset? date)
    {
        return date.HasValue ? new DateOnly(date.Value.Year, date.Value.Month, date.Value.Day) : null;
    }
}
