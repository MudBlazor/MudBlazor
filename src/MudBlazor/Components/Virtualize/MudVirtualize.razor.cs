// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace MudBlazor
{
    public partial class MudVirtualize<T> : ComponentBase
    {
        /// <summary>
        /// Represents a virtualized container for rendering a large list of items efficiently.
        /// </summary>
        private Virtualize<T>? _virtualizeContainerReference;

        // TODO: Remove splatting workaround when .NET 8 support is dropped and pass MaxItemCount directly as an attribute instead (#12701).
        private readonly Dictionary<string, object?> _virtualizeAttributes = new();

        /// <summary>
        /// Set false to turn off virtualization
        /// </summary>
        [Parameter]
        public bool Enabled { get; set; }

        /// <summary>
        /// Item template for the list.
        /// </summary>
        [Parameter]
        public RenderFragment<T>? ChildContent { get; set; }

        /// <summary>
        /// Template for the items that have not yet been loaded in memory.
        /// </summary>
        [Parameter]
        public RenderFragment? Placeholder { get; set; }

        /// <summary>
        /// The content shown when there are no rows to display.
        /// </summary>
        [Parameter]
        public RenderFragment? NoRecordsContent { get; set; }

        /// <summary>
        /// Fixed item source.
        /// </summary>
        [Parameter]
        public ICollection<T>? Items { get; set; }

        /// <summary>
        /// Function providing items to the list.
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

#if NET9_0_OR_GREATER
        /// <summary>
        /// Gets or sets the maximum number of items that will be rendered, even if the client reports
        /// that its viewport is large enough to show more. The default value is 100.
        ///
        /// This should only be used as a safeguard against excessive memory usage or large data loads.
        /// Do not set this to a smaller number than you expect to fit on a realistic-sized window, because
        /// that will leave a blank gap below and the user may not be able to see the rest of the content.
        /// </summary>
        [Parameter]
        public int MaxItemCount { get; set; } = 100;
#endif

        /// <summary>
        /// Gets or sets tag name of the HTML element that will be used as virtualization spacer. Default is div.
        /// </summary>
        [Parameter]
        public string SpacerElement { get; set; } = "div";

        /// <inheritdoc/>
        protected override void OnParametersSet()
        {
            _virtualizeAttributes.Clear();
#if NET9_0_OR_GREATER
            _virtualizeAttributes[nameof(MaxItemCount)] = MaxItemCount;
#endif
        }

        /// <summary>
        /// Refreshes the data in the Virtualize component asynchronously.
        /// </summary>
        public Task RefreshDataAsync()
        {
            return _virtualizeContainerReference is null
                ? Task.CompletedTask
                : _virtualizeContainerReference.RefreshDataAsync();
        }
    }
}
