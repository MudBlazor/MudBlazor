using System;

namespace MudBlazor.Services;

#nullable enable
internal interface IDateConverter<T>
{
    DateTimeOffset? ConvertTo(T? date);

    T? ConvertFrom(DateTimeOffset? date);
}
