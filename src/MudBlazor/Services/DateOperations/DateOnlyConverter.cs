using System;

namespace MudBlazor.Services.DateOperations;

internal class DateOnlyConverter : IDateConverter<DateOnly>
{
    public DateTimeOffset ConvertTo(DateOnly date)
    {
        return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, TimeSpan.Zero);
    }

    public DateTimeOffset? ConvertTo(DateOnly? date)
    {
        return date.HasValue ? ConvertTo(date.Value) : null;
    }

    public DateOnly ConvertFrom(DateTimeOffset date)
    {
        return new DateOnly(date.Year, date.Month, date.Day);
    }

    public DateOnly? ConvertFrom(DateTimeOffset? date)
    {
        return date.HasValue ? ConvertFrom(date.Value) : null;
    }
}
