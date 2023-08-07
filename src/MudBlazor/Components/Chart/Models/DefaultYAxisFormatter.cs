﻿// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

using System.Globalization;

public class DefaultYAxisFormatter : IYAxisFormatter
{
    public string Format(double value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}
