using System;
using System.Globalization;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public static class Converters
    {
        public static CultureInfo DefaultCulture => CultureInfo.CurrentUICulture;

        #region --> Date converters
        public static Converter<DateTime> IsoDate
            => new()
            { SetFunc = SetIsoDate, GetFunc = GetIsoDate };

        private static DateTime GetIsoDate(string value)
        {
            if (DateTime.TryParse(value, out var dateTime))
                return dateTime;
            return DateTime.MinValue;
        }

        private static string SetIsoDate(DateTime value)
        {
            return value.ToIsoDateString();
        }

        public static Converter<DateTime?> NullableIsoDate
            => new()
            { SetFunc = SetNullableIsoDate, GetFunc = GetNullableIsoDate };

        private static DateTime? GetNullableIsoDate(string value)
        {
            if (DateTime.TryParse(value, out var dateTime))
                return dateTime;
            return null;
        }

        private static string SetNullableIsoDate(DateTime? value)
        {
            return value.ToIsoDateString();
        }

        public static DateConverter DateFormat(string format)
        {
            format ??= "yyyy-MM-dd";
            return new DateConverter(format);
        }

        public static DateConverter DateFormat(string format, CultureInfo culture)
        {
            return new DateConverter(format) { Culture = culture };
        }

        #endregion
    }
}
