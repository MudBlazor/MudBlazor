// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;

namespace MudBlazor;

public class IPv4Mask : RegexMask
{
    private const string Octet = "(25[0-5]|2[0-4][0-9]|[0-1]?[0-9]?[0-9])";

    /// <summary>
    ///     Create an IPv4 mask to restrict input.
    /// </summary>
    public IPv4Mask()
        : base("000.000.000.000")
    {
        Delimiters = ".";
    }

    protected override void InitRegex()
    {
        _regex =
            new
                Regex($"^({Octet})(\\.|\\.{Octet})?(\\.|\\.{Octet})?(\\.|\\.{Octet})?$");
    }
}
