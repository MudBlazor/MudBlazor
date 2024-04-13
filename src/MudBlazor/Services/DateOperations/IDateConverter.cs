using System;

namespace MudBlazor.Services.DateOperations;

internal interface IDateConverter<T> where T : struct
{
    DateTimeOffset ConvertTo(T date);
    DateTimeOffset? ConvertTo(T? date);

    T ConvertFrom(DateTimeOffset date);
    T? ConvertFrom(DateTimeOffset? date);
}
