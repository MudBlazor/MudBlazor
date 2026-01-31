// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Resources;
using MudBlazor.Utilities.Exceptions;

namespace MudBlazor;

#nullable enable
internal partial class DefaultConverter
{
    internal sealed class GuidConverter : IReversibleConverter<Guid, string?>, IReversibleConverter<Guid?, string?>
    {
        public Guid ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return Guid.Empty;
            }

            if (Guid.TryParse(input, out var guid))
            {
                return guid;
            }

            throw new ConversionException(LanguageResource.Converter_InvalidGUID);
        }

        public string Convert(Guid value) => value.ToString();

        public string? Convert(Guid? value) => value is null ? null : value.ToString();

        Guid? IReversibleConverter<Guid?, string?>.ConvertBack(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            return ConvertBack(input);
        }

        public static GuidConverter Instance { get; } = new();
    }
}
