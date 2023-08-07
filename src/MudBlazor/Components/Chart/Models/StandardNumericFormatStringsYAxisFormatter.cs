// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

public class StandardNumericFormatStringsYAxisFormatter : IYAxisFormatter
{
    private readonly string _format;

    public StandardNumericFormatStringsYAxisFormatter(string format)
    {
        _format = format;
    }
    
    public string Format(double value)
    {
        return value.ToString(_format);
    }
}
