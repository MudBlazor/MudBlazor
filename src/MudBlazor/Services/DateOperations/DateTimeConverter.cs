using System;

namespace MudBlazor.Services.DateOperations;

internal class DateTimeConverter : IDateConverter<DateTime>
{
    public DateTimeOffset ConvertTo(DateTime date)
    {
        return new DateTimeOffset(date);
    }

    public DateTime ConvertFrom(DateTimeOffset date)
    {
        return date.DateTime;
    }
}
