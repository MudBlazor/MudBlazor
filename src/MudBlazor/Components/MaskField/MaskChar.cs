// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.MaskField
{
    public class MaskChar
    {
        public char Char { get; set; }
        public string Regex { get; set; }
        public bool AddToValue { get; set; }
        public bool Writable { get; set; }

        public static MaskChar Letter(char c) => new MaskChar { Char = c, Regex = @"^\w$", AddToValue = true };
        public static MaskChar Digit(char c) => new MaskChar { Char = c, Regex = @"^\d$", AddToValue = true };
        public static MaskChar LetterOrDigit(char c) => new MaskChar { Char = c, Regex = @"^\w|\d$", AddToValue = true };
    }
}
