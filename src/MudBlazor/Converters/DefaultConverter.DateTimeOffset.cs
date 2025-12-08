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
    internal sealed class DateTimeOffsetConverter(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<DateTimeOffset, string?>, IReversibleConverter<DateTimeOffset?, string?>
    {
        public string Convert(DateTimeOffset input) => input.ToString(format.Invoke(), culture.Invoke());

        public string? Convert(DateTimeOffset? input) => input?.ToString(format.Invoke(), culture.Invoke());

        public DateTimeOffset ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }

            var currentCulture = culture.Invoke();
            if (DateTimeOffset.TryParseExact(input, format.Invoke() ?? currentCulture.DateTimeFormat.ShortDatePattern, currentCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidDateTime);
        }

        DateTimeOffset? IReversibleConverter<DateTimeOffset?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }
    }
}
