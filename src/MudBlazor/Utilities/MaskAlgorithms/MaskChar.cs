// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor;

public struct MaskChar
{
    public MaskChar(char c, string regex)
    {
        Char = c;
        Regex = regex;
    }

    public char Char { get; set; }
    public string Regex { get; set; }

    public static MaskChar Letter(char c) => new MaskChar { Char = c, Regex = @"\p{L}" };
    public static MaskChar Digit(char c) => new MaskChar { Char = c, Regex = @"\d" };
    public static MaskChar LetterOrDigit(char c) => new MaskChar { Char = c, Regex = @"\p{L}|\d" };
}
