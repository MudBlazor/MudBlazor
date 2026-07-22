// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MudBlazor.Utilities
{
#nullable enable
    public static class WebUnitsExtensions
    {
        public static string ToPx(this int val) => $"{val}px";
        public static string ToPx(this int? val) => val != null ? val.Value.ToPx() : string.Empty;
        public static string ToPx(this long val) => $"{val}px";
        public static string ToPx(this long? val) => val != null ? val.Value.ToPx() : string.Empty;
        public static string ToPx(this double val) => $"{val.ToString("0.##", CultureInfo.InvariantCulture)}px";
        public static string ToPx(this double? val) => val != null ? val.Value.ToPx() : string.Empty;
    }
}
