using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public abstract class MudBaseBindableItemsControl<TChildComponent, TData> : MudBaseItemsControl<TChildComponent>
        where TChildComponent : MudComponentBase
    {

        /// <summary>
        /// Items Collection - For databinding usage
        /// </summary>
        [Parameter]
        public IEnumerable<TData> ItemsSource { get; set; }

        /// <summary>
        /// Template for each Item in ItemsSource collection
        /// </summary>
        [Parameter]
        public RenderFragment<TData> ItemTemplate { get; set; }

        /// <summary>
        /// Gets the Selected Item from ItemsSource, or Selected TChildComponent, when it's null
        /// </summary>
        public object SelectedItem
        {
            get => ItemsSource == null ? Items[SelectedIndex] : ItemsSource.ElementAtOrDefault(SelectedIndex);
        }
    }
}
