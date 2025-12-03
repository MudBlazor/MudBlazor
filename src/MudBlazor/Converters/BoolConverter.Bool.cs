// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
internal partial class BoolConverter
{
    internal sealed class BoolIdentity : IReversibleConverter<bool, bool?>, IReversibleConverter<bool?, bool?>
    {
        public bool? Convert(bool value) => value;

        public bool? Convert(bool? value) => value;

        bool IReversibleConverter<bool, bool?>.ConvertBack(bool? value) => value == true;

        public bool? ConvertBack(bool? value) => value;

        public static readonly BoolIdentity Instance = new();
    }
}
