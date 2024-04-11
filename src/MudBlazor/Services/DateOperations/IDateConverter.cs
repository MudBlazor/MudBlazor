using System;

namespace MudBlazor.Services.DateOperations;

internal interface IDateConverter<T>
{
    DateTimeOffset ConvertTo(T date);

    T ConvertFrom(DateTimeOffset date);
}
