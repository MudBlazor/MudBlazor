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
    public class GridTemplate
    {
        private string _value = "repeat(auto-fit, 1fr)";

        public static GridTemplate Fixed(params GridSpan[] spans) => new() { _value = string.Join(" ", spans.Select(t => t.ToString())) };
        public static GridTemplate Repeat(GridSpan track) => new() { _value = $"repeat(auto-fill, {track})" };
        public static GridTemplate Auto() => new() { _value = "repeat(auto-fit, 1fr)" };

        public override string ToString() => _value;
    }
}
