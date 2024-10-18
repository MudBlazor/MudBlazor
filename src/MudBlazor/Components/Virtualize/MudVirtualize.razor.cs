// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace MudBlazor
{
#nullable enable
    public partial class MudVirtualize<T> : ComponentBase
    {
        /// <summary>
        /// Represents a virtualized container for rendering a large list of items efficiently.
        /// </summary>
        private Virtualize<T>? _virtualizeContainerReference;

        /// <summary>
        /// Set false to turn off virtualization
        /// </summary>
        [Parameter]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the item template for the list.
        /// </summary>
        [Parameter]
        public RenderFragment<T>? ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the template for the items that have not yet been loaded in memory.
        /// </summary>
        [Parameter]
        public RenderFragment? Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the fixed item source.
        /// </summary>
        [Parameter]
        public ICollection<T>? Items { get; set; }

        /// <summary>
        /// Gets or sets the function providing items to the list.
        /// </summary>
        [Parameter]
        public ItemsProviderDelegate<T>? ItemsProvider { get; set; }

        /// <summary>
        /// Gets or sets a value that determines how many additional items will be rendered
        /// before and after the visible region. This help to reduce the frequency of rendering
        /// during scrolling. However, higher values mean that more elements will be present
        /// in the page.
        /// </summary>
        [Parameter]
        public int OverscanCount { get; set; } = 3;

        /// <summary>
        /// Gets the size of each item in pixels. Defaults to 50px.
        /// </summary>
        [Parameter]
        public float ItemSize { get; set; } = 50f;

        /// <summary>
        /// Gets or sets tag name of the HTML element that will be used as virtualization spacer. Default is div.
        /// </summary>
        [Parameter]
        public string SpacerElement { get; set; } = "div";

        /// <summary>
        /// Refreshes the data in the Virtualize component asynchronously.
        /// </summary>
        public async Task RefreshDataAsync()
        {
            if (_virtualizeContainerReference != null)
            {
                await _virtualizeContainerReference.RefreshDataAsync();
            }
        }
    }
}
