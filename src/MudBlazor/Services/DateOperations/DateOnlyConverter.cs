using System;

namespace MudBlazor.Services.DateOperations;

internal class DateOnlyConverter : IDateConverter<DateOnly>
{
    public DateTimeOffset ConvertTo(DateOnly date)
    {
        return new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, TimeSpan.Zero);
    }

    public DateOnly ConvertFrom(DateTimeOffset date)
    {
        return new DateOnly(date.Year, date.Month, date.Day);
    }
}
