﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public abstract class MudBaseItemsControl<TChildComponent> : MudComponentBase
            where TChildComponent : MudComponentBase

    {
        /// <summary>
        /// Collection of T
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// Items - will be ignored when ItemsSource is not null
        /// </summary>
        public List<TChildComponent> Items { get; } = new List<TChildComponent>();

        private int _selectedIndexField = -1;
        /// <summary>
        /// Selected Item's index
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public int SelectedIndex
        {
            get => _selectedIndexField;
            set
            {
                if (SelectedIndex == value)
                    return;

                LastContainer = _selectedIndexField >= 0 ? SelectedContainer : null;
                _selectedIndexField = value;
                SelectionChanged();
                StateHasChanged();
                SelectedIndexChanged.InvokeAsync(value);
            }
        }

        [Parameter]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        /// <summary>
        /// Gets the Selected TChildComponent
        /// </summary>
        public TChildComponent LastContainer { get; private set; } = null;

        /// <summary>
        /// Gets the Selected TChildComponent
        /// </summary>
        public TChildComponent SelectedContainer
        {
            get => SelectedIndex >= 0 && Items.Count > SelectedIndex ? Items[SelectedIndex] : null;
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Items.Count > 0 && SelectedIndex < 0)
                    MoveTo(0);

            }
            return base.OnAfterRenderAsync(firstRender);
        }

        internal bool _moveNext = true;

        /// <summary>
        /// Move to Previous Item
        /// </summary>
        public void Previous()
        {
            _moveNext = false;

            if (SelectedIndex > 0)
                SelectedIndex--;
            else
                SelectedIndex = Items.Count - 1;
        }

        /// <summary>
        /// Move to Next Item
        /// </summary>
        public void Next()
        {
            _moveNext = true;

            if (SelectedIndex < (Items.Count - 1))
                SelectedIndex++;
            else
                SelectedIndex = 0;
        }

        /// <summary>
        /// Move to Item at desired index
        /// </summary>
        public void MoveTo(int index)
        {
            if (SelectedIndex != index)
            {
                _moveNext = index >= SelectedIndex;
                SelectedIndex = index;
            }
        }

        protected virtual void SelectionChanged() { }

        public virtual void AddItem(TChildComponent item) { }
    }
}
