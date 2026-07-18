// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MudBlazor.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="double"/> values that format them using the invariant culture.
    /// </summary>
    public static class DoubleExtensions
    {
        public static string ToInvariantString(this double input) => input.ToString(CultureInfo.InvariantCulture);
    }
}
