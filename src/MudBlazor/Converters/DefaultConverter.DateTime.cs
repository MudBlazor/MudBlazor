// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor;

internal partial class DefaultConverter
{
    public sealed class DateTimeConverter(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<DateTime, string?>, IReversibleConverter<DateTime?, string?>
    {
        public string Convert(DateTime input)
        {
            var currentCulture = culture.Invoke();
            return input.ToString(format.Invoke() ?? currentCulture.DateTimeFormat.ShortDatePattern, currentCulture);
        }

        public string? Convert(DateTime? input)
        {
            var currentCulture = culture.Invoke();
            return input?.ToString(format.Invoke() ?? currentCulture.DateTimeFormat.ShortDatePattern, currentCulture);
        }

        public DateTime ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }

            var currentCulture = culture.Invoke();
            if (DateTime.TryParseExact(input, format.Invoke() ?? currentCulture.DateTimeFormat.ShortDatePattern, currentCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidDateTime);
        }

        DateTime? IReversibleConverter<DateTime?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }
    }
}
