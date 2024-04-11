// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using MudBlazor.Extensions;

namespace MudBlazor.Services.DateOperations;

internal class DateWrapper<T> where T : struct
{
    private readonly IDateConverter<T> _converter;

    public CultureInfo Culture { get; private set; }

    public DateWrapper(IDateConverter<T> converter, CultureInfo culture)
    {
        _converter = converter;
        Culture = culture;
    }

    public void SetCulture(CultureInfo culture)
    {
        Culture = culture;
    }

    public T EndOfMonth(T date, int month = 0)
    {
        var dateTime = _converter.ConvertTo(date)
            .AddMonths(month, Culture)
            .EndOfMonth(Culture);

        return _converter.ConvertFrom(dateTime);
    }

    public T StartOfMonth(T date, int month = 0)
    {
        var dateTime = _converter.ConvertTo(date)
            .AddMonths(month, Culture)
            .StartOfMonth(Culture);

        return _converter.ConvertFrom(dateTime);
    }
}
