// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

internal partial class DefaultConverter
{
    internal sealed class ToStringFallbackConverter<T> : IReversibleConverter<T?, string?>
    {
        public string? Convert(T? input) => input?.ToString();

        public T? ConvertBack(string? input) => default;
    }
}
