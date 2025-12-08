// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor;

#nullable enable
internal partial class DefaultConverter
{
    internal sealed class DateOnlyConverter(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<DateOnly, string?>, IReversibleConverter<DateOnly?, string?>
    {
        public string Convert(DateOnly input) => input.ToString(format.Invoke(), culture.Invoke());

        public string? Convert(DateOnly? input) => input?.ToString(format.Invoke(), culture.Invoke());

        public DateOnly ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }

            var currentCulture = culture.Invoke();
            if (DateOnly.TryParseExact(input, format.Invoke() ?? currentCulture.DateTimeFormat.ShortDatePattern, currentCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            // TODO: Differentiate error message for DateOnly
            throw new ConversionException(LanguageResource.Converter_InvalidDateTime);
        }

        DateOnly? IReversibleConverter<DateOnly?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }
    }
}
