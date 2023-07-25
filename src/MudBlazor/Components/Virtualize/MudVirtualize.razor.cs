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


        private bool _refreshVirtualContainer;
        [Parameter]
        public bool RefreshVirtualContainer 
        {   get =>_refreshVirtualContainer; 
            set {
                if (value == _refreshVirtualContainer)
                    return;
                _refreshVirtualContainer = value;
                if(_refreshVirtualContainer)
                {
                    InvokeAsync(RefreshAsync);
                    _refreshVirtualContainer = false;
                }
                if(RefreshVirtualContainerChanged.HasDelegate)
                {
                    RefreshVirtualContainerChanged.InvokeAsync(_refreshVirtualContainer);
                }
            } 
        }

        
        [Parameter]
        public EventCallback<bool> RefreshVirtualContainerChanged { get; set; }


        private Virtualize<T> VirtualizeContainer { get; set; }

        private async Task RefreshAsync()
        {
            await VirtualizeContainer.RefreshDataAsync();
        }
    }
}
