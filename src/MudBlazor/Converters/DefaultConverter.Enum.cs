// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor;

internal partial class DefaultConverter
{
    internal sealed class EnumConverter<TEnum> : IReversibleConverter<TEnum, string?>
        where TEnum : struct, Enum
    {
        public string Convert(TEnum input) => input.ToString();

        public TEnum ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return default;
            }

            if (Enum.TryParse<TEnum>(input, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_NotValueOf, [typeof(TEnum).Name]);
        }
    }

    internal sealed class NullableEnumConverter<TEnum> : IReversibleConverter<TEnum?, string?>
        where TEnum : struct, Enum
    {
        public string? Convert(TEnum? input) => input?.ToString();

        public TEnum? ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            if (Enum.TryParse<TEnum>(input, out var result))
            {
                return result;
            }

            throw new ConversionException(LanguageResource.Converter_NotValueOf, [typeof(TEnum).Name]);
        }
    }
}
