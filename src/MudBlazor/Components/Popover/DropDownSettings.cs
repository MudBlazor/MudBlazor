// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    /// <summary>
    /// Configures the behavior of a dropdown popover, specifically the Fixed and OverflowBehavior properties.  
    /// </summary>
    public struct DropdownSettings
    {
        /// <summary>
        /// Displays the dropdown popover in a fixed position, even through scrolling.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>False</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public bool Fixed { get; set; }

        /// <summary>
        /// The behavior applied when there is not enough space for this dropdown to be visible.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="OverflowBehavior.FlipOnOpen"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public OverflowBehavior OverflowBehavior { get; set; } = OverflowBehavior.FlipOnOpen;

        public DropdownSettings() { }
    }
}
