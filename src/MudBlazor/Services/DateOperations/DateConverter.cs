// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor.Services.DateOperations
{
    public class DateValueConverter
    {
        public static DateTimeOffset? ConvertFrom<TValue>(TValue date)
        {
            if (date == null)
            {
                return null;
            }

            return date switch
            {
                DateOnly dateOnly => new DateTimeOffset(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0, TimeSpan.Zero),
                DateTime dateTime => dateTime.Kind is DateTimeKind.Unspecified or DateTimeKind.Utc
                    ? new DateTimeOffset(dateTime, TimeSpan.Zero)
                    : new DateTimeOffset(dateTime),
                DateTimeOffset dateTimeOffset => dateTimeOffset,
                _ => throw new ArgumentException("Invalid date type")
            };
        }

        public static TValue ConvertTo<TValue>(DateTimeOffset? date)
        {
            if (date == null)
            {
                return default;
            }

            if (typeof(TValue) == typeof(DateOnly) || typeof(TValue) == typeof(DateOnly?))
            {
                return (TValue)(object)new DateOnly(date.Value.Year, date.Value.Month, date.Value.Day);
            }
            else if (typeof(TValue) == typeof(DateTime) || typeof(TValue) == typeof(DateTime))
            {
                return (TValue)(object)date.Value.DateTime;
            }
            else if (typeof(TValue) == typeof(DateTimeOffset) || typeof(TValue) == typeof(DateTimeOffset))
            {
                return (TValue)(object)date.Value;
            }
            else
            {
                throw new ArgumentException("Invalid date type");
            }
        }
    }
}
