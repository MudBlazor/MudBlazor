// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MudBlazor;

internal partial class DefaultConverter
{
    internal sealed class BoolConverter : IReversibleConverter<bool?, string?>, IReversibleConverter<bool, string?>
    {
        public string Convert(bool input) => input.ToString(CultureInfo.InvariantCulture);

        public string? Convert(bool? input) => input?.ToString(CultureInfo.InvariantCulture);

        public bool ConvertBack(string? input) =>
            input?.ToLowerInvariant() switch
            {
                "true" or "1" or "on" => true,
                _ => false
            };

        bool? IReversibleConverter<bool?, string?>.ConvertBack(string? input) =>
            input?.ToLowerInvariant() switch
            {
                "true" or "1" or "on" => true,
                "false" or "0" or "off" => false,
                _ => null
            };

        public static readonly BoolConverter Instance = new();
    }
}
