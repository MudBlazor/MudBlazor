// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor;

internal partial class DefaultConverter
{
    internal sealed class ParsableConverter<TParsable>(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<TParsable?, string?>
        where TParsable : IParsable<TParsable>
    {
        public string? Convert(TParsable? input) => input switch
        {
            null => null,
            IFormattable formattable => formattable.ToString(format.Invoke(), culture.Invoke()),
            _ => input.ToString()
        };

        public TParsable? ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }

            if (TParsable.TryParse(input, culture.Invoke(), out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidType, [typeof(TParsable).Name]);
        }
    }

    internal sealed class NullableParsableConverter<TParsable>(Func<CultureInfo> culture, Func<string> format)
        : IReversibleConverter<TParsable?, string?>
        where TParsable : struct, IParsable<TParsable>
    {
        public string? Convert(TParsable? input) => input switch
        {
            null => null,
            IFormattable formattable => formattable.ToString(format.Invoke(), culture.Invoke()),
            _ => input.ToString()
        };

        public TParsable? ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            if (TParsable.TryParse(input, culture.Invoke(), out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidType, [typeof(TParsable).Name]);
        }
    }
}
