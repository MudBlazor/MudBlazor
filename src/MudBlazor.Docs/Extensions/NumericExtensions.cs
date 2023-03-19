// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace MudBlazor.Docs.Extensions
{
    public static class NumericExtensions
    {
        public static string RoundedToString(this decimal num)
        {
            if (num > 999999999 || num < -999999999)
            {
                return num.ToString("0,,,.#B", CultureInfo.InvariantCulture);
            }
            else if (num > 999999 || num < -999999)
            {
                return num.ToString("0,,.#M", CultureInfo.InvariantCulture);
            }
            else if (num > 9999 || num < -9999)
            {
                return num.ToString("0,k", CultureInfo.InvariantCulture);
            }
            else if (num > 999 || num < -999)
            {
                return num.ToString("0,.#k", CultureInfo.InvariantCulture);
            }
            else
            {
                return num.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
