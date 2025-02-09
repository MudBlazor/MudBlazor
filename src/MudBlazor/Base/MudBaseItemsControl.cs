using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a base class for designing components which contain items.
    /// </summary>
    /// <typeparam name="TChildComponent">The type of <see cref="MudComponentBase"/> managed by this component.</typeparam>
    public abstract class MudBaseItemsControl<TChildComponent> : MudComponentBase
            where TChildComponent : MudComponentBase
    {
        private int _selectedIndexField = -1;

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.General.Data)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The index of the currently selected item.
        /// </summary>
        /// <remarks>
        /// When this property changes, the <see cref="SelectedIndexChanged"/> event occurs.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.General.Behavior)]
        public int SelectedIndex
        {
            get => _selectedIndexField;
            set
            {
                if (SelectedIndex == value)
                    return;

                _moveNext = value >= _selectedIndexField;
                LastContainer = _selectedIndexField >= 0 ? SelectedContainer : null;
                _selectedIndexField = value;
                SelectionChanged();
                StateHasChanged();
                SelectedIndexChanged.InvokeAsync(value);
            }
        }

        /// <summary>
        /// Occurs when the <see cref="SelectedIndex"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> SelectedIndexChanged { get; set; }

        /// <summary>
        /// The previously selected item.
        /// </summary>
        public TChildComponent? LastContainer { get; private set; } = null;

        /// <summary>
        /// The list of items.
        /// </summary>
        /// <remarks>
        /// This property is ignored when <c>ItemsSource</c> is not null.
        /// </remarks>
        public List<TChildComponent> Items { get; } = new List<TChildComponent>();

        /// <summary>
        /// The currently selected item.
        /// </summary>
        /// <remarks>
        /// This property returns the item in the <see cref="Items"/> property at the <see cref="SelectedIndex"/>.
        /// </remarks>
        public TChildComponent? SelectedContainer
        {
            get => SelectedIndex >= 0 && Items.Count > SelectedIndex ? Items[SelectedIndex] : null;
        }

        /// <inheritdoc />
        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Items.Count > 0 && SelectedIndex < 0)
                {
                    MoveTo(0);
                }
            }

            return base.OnAfterRenderAsync(firstRender);
        }

        internal bool _moveNext = true;

        /// <summary>
        /// Selects the previous item.
        /// </summary>
        /// <remarks>
        /// If the current <see cref="SelectedIndex" /> is <c>0</c>, the selection is changed to the index of the last item.
        /// </remarks>
        public void Previous()
        {
            _moveNext = false;

            if (SelectedIndex > 0)
            {
                SelectedIndex--;
            }
            else
            {
                SelectedIndex = Items.Count - 1;
            }
        }

        /// <summary>
        /// Selects the next item.
        /// </summary>
        /// <remarks>
        /// If the current <see cref="SelectedIndex" /> is the last item, the selection is changed to <c>0</c>.
        /// </remarks>
        public void Next()
        {
            _moveNext = true;

            if (SelectedIndex < (Items.Count - 1))
            {
                SelectedIndex++;
            }
            else
            {
                SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Changes the <see cref="SelectedIndex"/> to the specified value.
        /// </summary>
        public void MoveTo(int index)
        {
            if (SelectedIndex != index)
            {
                _moveNext = index >= SelectedIndex;
                SelectedIndex = index;
            }
        }

        /// <summary>
        /// Occurs when the <see cref="SelectedIndex"/> has changed.
        /// </summary>
        protected virtual void SelectionChanged() { }

        /// <summary>
        /// When overridden, adds an item to the list.
        /// </summary>
        /// <param name="item">
        /// The item to add.
        /// </param>
        public virtual void AddItem(TChildComponent item) { }
    }
}
