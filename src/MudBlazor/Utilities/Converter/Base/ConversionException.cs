// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Utilities.Converter.Base;

#nullable enable
public class ConversionException : Exception
{
    public string ErrorMessageKey { get; }

    public object[] ErrorMessageArgs { get; }

    public ConversionException(string key, object[]? arguments = null, Exception? inner = null)
        : base(key, inner)
    {
        ErrorMessageKey = key ?? throw new ArgumentNullException(nameof(key));
        ErrorMessageArgs = arguments ?? [];
    }
}
