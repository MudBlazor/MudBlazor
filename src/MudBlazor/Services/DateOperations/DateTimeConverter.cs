using System;

namespace MudBlazor.Services.DateOperations;

internal class DateTimeConverter : IDateConverter<DateTime>
{
    public DateTimeOffset ConvertTo(DateTime date)
    {
        return new DateTimeOffset(date);
    }

    public DateTimeOffset? ConvertTo(DateTime? date)
    {
        return date.HasValue ? ConvertTo(date.Value) : null;
    }

    public DateTime ConvertFrom(DateTimeOffset date)
    {
        return date.DateTime;
    }

    public DateTime? ConvertFrom(DateTimeOffset? date)
    {
        return date.HasValue ? ConvertFrom(date.Value) : null;
    }
}
