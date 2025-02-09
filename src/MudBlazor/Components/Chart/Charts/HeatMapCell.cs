// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor.Charts
{
#nullable enable
    public class HeatMapCell
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public double? Value { get; set; }

        public MudColor? MudColor { get; set; }

        public int? Width { get; set; }

        public int? Height { get; set; }

        public RenderFragment? CustomFragment { get; set; }
    }
}
