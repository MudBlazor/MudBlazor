﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a base class for designing components with bindable items.
    /// </summary>
    /// <typeparam name="TChildComponent">The <see cref="MudComponentBase"/> managed within this component..</typeparam>
    /// <typeparam name="TData">The type of item managed by this component.</typeparam>
    [DebuggerDisplay("SelectedIndex={SelectedIndex}, SelectedItem={SelectedItem}, Items={Items}, ItemsSource={ItemsSource}")]
    public abstract class MudBaseBindableItemsControl<TChildComponent, TData> : MudBaseItemsControl<TChildComponent>
        where TChildComponent : MudComponentBase
    {
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public IEnumerable<TData>? ItemsSource { get; set; }

        /// <summary>
        /// Gets or sets the template used to display each item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Appearance)]
        public RenderFragment<TData>? ItemTemplate { get; set; }

        /// <summary>
        /// Gets the currently selected item.
        /// </summary>
        /// <remarks>
        /// This property will return either an item from the <c>Items</c> property, or an item from <see cref="ItemsSource"/> if <c>Items</c> is <c>null</c>.
        /// </remarks>
        public object? SelectedItem
        {
            get => ItemsSource == null ? Items[SelectedIndex] : ItemsSource.ElementAtOrDefault(SelectedIndex);
        }
    }
}
