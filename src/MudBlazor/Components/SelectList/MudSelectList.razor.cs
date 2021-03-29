using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using static MudBlazor.Colors;

namespace MudBlazor
{
    public partial class MudSelectList<T> : MudComponentBase, IDisposable
    {
        #region private fields
        private HashSet<MudSelectListItem<T>> _items = new();
        private HashSet<MudSelectList<T>> _childLists = new();
        private HashSet<T> _selection = new();
        #endregion

        #region properties
        protected string Classname =>
        new CssBuilder("mud-list")
           .AddClass("mud-list-padding", !DisablePadding)
          .AddClass(Class)
        .Build();

        [CascadingParameter] protected MudSelectList<T> ParentList { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If true, vertical padding will be removed from the list.
        /// </summary>
        [Parameter] public bool DisablePadding { get; set; }

        /// <summary>
        /// If true, compact vertical padding will be applied to all list items.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, the left and right padding is removed on all list items.
        /// </summary>
        [Parameter] public bool DisableGutters { get; set; }

        /// <summary>
        /// If true, will disable the list item if it has onclick.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Set to true to enable selection of multiple items. 
        /// </summary>
        [Parameter] public bool MultiSelection { get; set; }

        /// <summary>
        /// Set to true, the user won't be able to deselect the last item, behaving like a radio button if <see cref="MultiSelection"/> is false.
        /// </summary>
        [Parameter] public bool RadioBehaviour { get; set; }

        /// <summary>
        /// Callback is called when an item has been clicked and returns the latest selected item.
        /// </summary>
        [Parameter] public EventCallback<T> SelectionChanged { get; set; }

        /// <summary>
        /// This returns the currently selected items. You can bind this property and the initial content of the HashSet you bind it to will cause these rows to be selected initially.
        /// </summary>
        [Parameter]
        public HashSet<T> SelectedItems
        {
            get
            {
                return _selection;
            }
            set
            {
                if (value == _selection)
                    return;
                if (value == null)
                {
                    if (_selection.Count == 0)
                        return;
                    _selection = new HashSet<T>();
                }
                else
                    _selection = value;

                foreach (var item in _items)
                    item.VerifySelection(_selection);
                foreach (var child in _childLists)
                    child.SelectedItems = _selection;

                SelectedItemsChanged.InvokeAsync(_selection);

                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Callback is called whenever items are selected or deselected in multi selection mode.
        /// </summary>
        [Parameter] public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        IEqualityComparer<T> _equalityComparer = EqualityComparer<T>.Default;
        /// <summary>
        /// Specify a different equality comparer then the default one.
        /// </summary>
        [Parameter]
        public IEqualityComparer<T> EqualityComparer
        {
            get => _equalityComparer;
            set
            {
                _equalityComparer = value ?? EqualityComparer<T>.Default;
            }
        }

        /// <summary>
        /// Defines how values are displayed in the list
        /// </summary>
        [Parameter]
        public Func<T, string> ToStringFunc { get; set; }
        #endregion

        protected override void OnInitialized()
        {
            ParentList?.Register(this);
        }

        internal event Action ParametersChanged;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ParametersChanged?.Invoke();
        }

        internal void Register(MudSelectListItem<T> listItem)
        {
            _items.Add(listItem);
            foreach (var item in _selection)
                if(_equalityComparer.Equals(listItem.Item, item))
                    listItem.SetSelected(true);
        }

        internal void Unregister(MudSelectListItem<T> listItem)
        {
            _items.Remove(listItem);
            if (_selection.Remove(listItem.Item))
                ParentList?.SetSelection(listItem.Item, false, true);
        }

        internal void Register(MudSelectList<T> child)
        {
            _childLists.Add(child);
            foreach (var item in _selection)
                child.SetSelection(item, true, true);
        }

        internal void Unregister(MudSelectList<T> child)
        {
            _childLists.Remove(child);
            foreach (var item in child.SelectedItems)
                SetSelection(item, false, true);
        }

        /// <summary>
        /// Determines if the element can be removed or not, for example if it's the last element when the parent list has <see cref="RadioBehaviour"/> enabled
        /// </summary>
        private bool BlockRemove()
        {
            if (ParentList != null)
                return ParentList.BlockRemove();

            return RadioBehaviour && _selection.Count == 1;
        }

        internal void SetSelection(T item, bool selected, bool force = false)
        {
            if (selected)
            {

                if (_selection.Contains(item))
                    return;

                if (MultiSelection)
                    _selection.Add(item);
                else
                {
                    if (_selection.Count > 0 && !_equalityComparer.Equals(_selection.First(), item))
                        SetSelection(_selection.First(), false, true);//remove the previous one

                    _selection = new HashSet<T> { item };
                }

                SelectedItemsChanged.InvokeAsync(_selection);
                SelectionChanged.InvokeAsync(item);
            }
            else
            {
                if (!force && BlockRemove())
                    return;
                //remove item and check if it has been removed, if not exit because it wasn't in the list to begin with
                if (!_selection.Remove(item))
                    return;

                SelectedItemsChanged.InvokeAsync(_selection);
            }

            foreach (var listItem in _items.ToArray())
            {
                if (_equalityComparer.Equals(item, listItem.Item))
                    listItem.SetSelected(selected);
            }
            foreach (var childList in _childLists.ToArray())
                childList.SetSelection(item, selected);
            ParentList?.SetSelection(item, selected);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ParametersChanged = null;
                ParentList?.Unregister(this);
            }
        }
    }
}
