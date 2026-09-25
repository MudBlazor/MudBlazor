// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Numerics;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor;

internal partial class DefaultConverter
{
    /// <summary>
    /// Parses a numeric string in the given culture, additionally accepting percent-formatted text
    /// such as the output of the "P" format, which numeric parsing cannot read on its own.
    /// </summary>
    internal static bool TryParseNumber<TNumber>(string input, CultureInfo? culture, [MaybeNullWhen(false)] out TNumber result)
        where TNumber : INumber<TNumber>
    {
        if (TNumber.TryParse(input, NumberStyles.Any, culture, out result))
        {
            return true;
        }

        // Formats such as "P" render a value multiplied by 100 and followed by the culture's
        // percent symbol. There is no NumberStyles flag for percent symbols, so the symbol is
        // removed and the multiplication undone to read the text back as the value it came from.
        var percentSymbol = (culture ?? CultureInfo.CurrentCulture).NumberFormat.PercentSymbol;
        if (!string.IsNullOrEmpty(percentSymbol) && input.Contains(percentSymbol))
        {
            var withoutPercentSymbol = input.Replace(percentSymbol, null).Trim();
            if (TNumber.TryParse(withoutPercentSymbol, NumberStyles.Any, culture, out result))
            {
                result = result / TNumber.CreateChecked(100);
                return true;
            }
        }

        return false;
    }

    internal sealed class NumberConverter<TNumber>(Func<CultureInfo> culture, Func<string?> format)
        : IReversibleConverter<TNumber, string?>
        where TNumber : INumber<TNumber>
    {
        public string Convert(TNumber input) => input.ToString(format.Invoke(), culture.Invoke());

        public TNumber ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return TNumber.Zero;
            }

            if (TryParseNumber<TNumber>(input, culture.Invoke(), out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidNumber);
        }
    }
}
