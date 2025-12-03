// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
internal partial class BoolConverter
{
    internal sealed class StringConverter : IReversibleConverter<string?, bool?>
    {
        public bool? Convert(string? input)
        {
            if (input is null) return null;
            if (bool.TryParse(input, out var b)) return b;

            return input.ToLowerInvariant() switch
            {
                "on" => true,
                "off" => false,
                _ => null
            };
        }

        public string? ConvertBack(bool? value) =>
            value switch
            {
                true => "on",
                false => "off",
                _ => null
            };

        public static readonly StringConverter Instance = new();
    }
}
