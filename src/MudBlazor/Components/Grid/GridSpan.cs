// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor.Components.Grid
{
    public class GridSpan
    {
        private string _value = "1fr";

        public static GridSpan Fr(int value = 1) => new() { _value = $"{value}fr" };
        public static GridSpan Px(int value) => new() { _value = $"{value}px" };
        public static GridSpan Rem(double value) => new() { _value = $"{value}rem" };

        public override string ToString() => _value;
    }
}
