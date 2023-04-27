// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable
    public partial class MudVirtualize<T> : ComponentBase
    {
        /// <summary>
        /// Set false to turn off virtualization
        /// </summary>
        [Parameter]
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the item template for the list.
        /// </summary>
        [Parameter]
        public RenderFragment<T>? ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the fixed item source.
        /// </summary>
        [Parameter]
        public ICollection<T>? Items { get; set; }

        /// <summary>
        /// Gets or sets a value that determines how many additional items will be rendered
        /// before and after the visible region. This help to reduce the frequency of rendering
        /// during scrolling. However, higher values mean that more elements will be present
        /// in the page.
        /// </summary>
        [Parameter]
        public int OverscanCount { get; set; } = 3;
    }
}
