// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace MudBlazor
{
    public partial class MudVirtualize<T> : ComponentBase
    {
        private Virtualize<T> innerComponent;

        /// <summary>
        /// Set false to turn off virtualization
        /// </summary>
        [Parameter] public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the item template for the list.
        /// </summary>
        [Parameter]
        public RenderFragment<T> ChildContent { get; set; }

        [Parameter]
        public RenderFragment<T> ItemContent { get; set; }

        [Parameter]
        public RenderFragment<PlaceholderContext> Placeholder { get; set; }

        /// <summary>
        /// Gets or sets the fixed item source.
        /// </summary>
        [Parameter]
        public ICollection<T> Items { get; set; }


        [Parameter]
        public ItemsProviderDelegate<T> ItemsProvider { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (!IsEnabled && Items == null && ItemsProvider != null)
            {
                var itemResult = await ItemsProvider.Invoke(new ItemsProviderRequest(0, int.MaxValue, default));
                Items = itemResult.Items.ToList();
            }
        }

        public Task RefreshDataAsync()
        {
            return innerComponent?.RefreshDataAsync() ?? Task.CompletedTask;
        }
    }
}
