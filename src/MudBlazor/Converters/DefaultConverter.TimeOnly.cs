// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor;

internal partial class DefaultConverter
{
    internal sealed class TimeOnlyConverter(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<TimeOnly, string?>, IReversibleConverter<TimeOnly?, string?>
    {
        public string Convert(TimeOnly input) => input.ToString(format.Invoke(), culture.Invoke());

        public string? Convert(TimeOnly? input) => input?.ToString(format.Invoke(), culture.Invoke());

        public TimeOnly ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }

            var currentCulture = culture.Invoke();
            var currentFormat = format.Invoke();

            // Without a Format, Convert writes the short time pattern, so read that back exactly before parsing leniently.
            // Lenient parsing alone misreads some patterns, such as fr-CA "12 h 30" as 12:00.
            var parsed = currentFormat is null
                ? TimeOnly.TryParseExact(input, "t", currentCulture, DateTimeStyles.None, out var result)
                    || TimeOnly.TryParse(input, currentCulture, DateTimeStyles.None, out result)
                : TimeOnly.TryParseExact(input, currentFormat, currentCulture, DateTimeStyles.None, out result);

            if (parsed)
            {
                return result;
            }

            // TODO: Differentiate error message for TimeOnly
            throw new ConversionException(LanguageResource.Converter_InvalidDateTime);
        }

        TimeOnly? IReversibleConverter<TimeOnly?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }
    }
}
