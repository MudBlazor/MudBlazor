// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
internal partial class DefaultConverter
{
    internal sealed class CharConverter : IReversibleConverter<char, string?>, IReversibleConverter<char?, string?>
    {
        public string Convert(char input) => input.ToString();

        public string? Convert(char? input) => input?.ToString();

        public char ConvertBack(string? input) => string.IsNullOrEmpty(input) ? '\0' : input[0];

        char? IReversibleConverter<char?, string?>.ConvertBack(string? input)
        {
            if (input is null)
            {
                return null;
            }

            if (input.Length == 0)
            {
                return null;
            }

            return ConvertBack(input);
        }

        public static readonly CharConverter Instance = new();
    }
}
