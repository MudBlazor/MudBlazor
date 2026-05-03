// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor;

internal partial class DefaultConverter
{
    internal sealed class GuidConverter(Func<CultureInfo> culture, Func<string?> format) : IReversibleConverter<Guid, string?>, IReversibleConverter<Guid?, string?>
    {
        private const string DefaultGuidFormat = "D";

        public Guid ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Guid.Empty;
            }

            if (Guid.TryParseExact(input, format.Invoke() ?? DefaultGuidFormat, out var guid))
            {
                return guid;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidGUID);
        }

        public string Convert(Guid value) => value.ToString(format.Invoke(), culture.Invoke());

        public string? Convert(Guid? value) => value?.ToString(format.Invoke(), culture.Invoke());

        Guid? IReversibleConverter<Guid?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }
    }
}
