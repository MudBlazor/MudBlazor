// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MudBlazor
{
    public class PopoverOptions
    {
        public string ContainerClass { get; set; } = "mudblazor-main-content";
        public int FlipMargin { get; set; } = 0;
        public bool ThrowOnDuplicateProvider { get; set; } = true;
    }
}
