// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MudBlazor.Extensions
{
    public static class DoubleExtensions
    {
        public static string ToInvariantString(this double input) => input.ToString(CultureInfo.InvariantCulture);

        // From .NET 11 on, Math.Round(value, digits) rounds the exact binary value, so 0.525 (stored as 0.52500000000000002) becomes 0.53 instead of 0.52.
        // Scaling before rounding is what earlier runtimes did, and keeps colors and chart labels the same on every runtime.
        // TODO(net11): A major version could switch to Math.Round(value, digits) once every target runs on .NET 11 or later, which shifts lightened and darkened colors and tied chart labels slightly.
        internal static double RoundToDigits(this double value, int digits)
        {
            var scale = Math.Pow(10, digits);
            return Math.Round(value * scale) / scale;
        }
    }
}
