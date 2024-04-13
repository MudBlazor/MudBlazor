using System;

namespace MudBlazor.Services.DateOperations;

internal class DateTimeOffsetConverter : IDateConverter<DateTimeOffset>
{
    public DateTimeOffset ConvertTo(DateTimeOffset date) => date;
    public DateTimeOffset? ConvertTo(DateTimeOffset? date) => date;

    public DateTimeOffset ConvertFrom(DateTimeOffset date) => date;
    public DateTimeOffset? ConvertFrom(DateTimeOffset? date) => date;
}
