using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;


namespace MudBlazor
{
    // note: the MudTable code is split. Everything depending on the type parameter T of MudTable<T> is here in MudTable<T>

    public partial class MudTable<T> : MudTableBase
    {
        /// <summary>
        /// Defines how a table row looks like. Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter] public RenderFragment<T> RowTemplate { get; set; }

        /// <summary>
        /// Row Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment<T> ChildRowContent { get; set; }

        /// <summary>
        /// Defines how a table row looks like in edit mode (for selected row). Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter] public RenderFragment<T> RowEditingTemplate { get; set; }

        /// <summary>
        /// The data to display in the table. MudTable will render one row per item
        /// </summary>
        [Parameter]
        public IEnumerable<T> Items
        {
            get => _items;
            set
            {
                if (_items == value)
                    return;
                _items = value;
                if (Context?.PagerStateHasChanged != null)
                    InvokeAsync(Context.PagerStateHasChanged);
            }
        }

        /// <summary>
        /// A function that returns whether or not an item should be displayed in the table. You can use this to implement your own search function.
        /// </summary>
        [Parameter] public Func<T, bool> Filter { get; set; } = null;

        /// <summary>
        /// Button click event.
        /// </summary>
        [Parameter] public EventCallback<TableRowClickEventArgs<T>> OnRowClick { get; set; }

        internal override void FireRowClickEvent(MouseEventArgs args, MudTr row, object o)
        {
            var item = default(T);
            try
            {
                item = (T)o;
            }
            catch (Exception) { /*ignore*/}
            OnRowClick.InvokeAsync(new TableRowClickEventArgs<T>()
            {
                MouseEventArgs = args,
                Row = row,
                Item = item,
            });
        }

        /// <summary>
        /// Returns the class that will get joined with RowClass. Takes the current item and row index.
        /// </summary>
        [Parameter]
        public Func<T, int, string> RowClassFunc { get; set; }

        /// <summary>
        /// Returns the style that will get joined with RowStyle. Takes the current item and row index.
        /// </summary>
        [Parameter]
        public Func<T, int, string> RowStyleFunc { get; set; }

        /// <summary>
        /// Returns the item which was last clicked on in single selection mode (that is, if MultiSelection is false)
        /// </summary>
        [Parameter]
        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (EqualityComparer<T>.Default.Equals(SelectedItem, value))
                    return;
                _selectedItem = value;
                SelectedItemChanged.InvokeAsync(value);
            }
        }
        private T _selectedItem;

        /// <summary>
        /// Callback is called when a row has been clicked and returns the selected item.
        /// </summary>
        [Parameter] public EventCallback<T> SelectedItemChanged { get; set; }

        /// <summary>
        /// If MultiSelection is true, this returns the currently selected items. You can bind this property and the initial content of the HashSet you bind it to will cause these rows to be selected initially.
        /// </summary>
        [Parameter]
        public HashSet<T> SelectedItems
        {
            get
            {
                if (!MultiSelection)
                    if (_selectedItem is null)
                        return new HashSet<T>(Array.Empty<T>());
                    else
                        return new HashSet<T>(new T[] { _selectedItem });
                else
                    return Context.Selection;
            }
            set
            {
                if (value == Context.Selection)
                    return;
                if (value == null)
                {
                    if (Context.Selection.Count == 0)
                        return;
                    Context.Selection = new HashSet<T>();
                }
                else
                    Context.Selection = value;
                SelectedItemsChanged.InvokeAsync(Context.Selection);
                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Callback is called whenever items are selected or deselected in multi selection mode.
        /// </summary>
        [Parameter] public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        public IEnumerable<T> FilteredItems
        {
            get
            {
                if (ServerData != null)
                    return _server_data.Items;

                if (Filter == null)
                    return Context.Sort(Items);
                return Context.Sort(Items.Where(Filter));
            }
        }

        protected IEnumerable<T> CurrentPageItems
        {
            get
            {
                if (@PagerContent == null)
                    return FilteredItems; // we have no pagination
                if (ServerData == null)
                {
                    var filteredItemCount = GetFilteredItemsCount();
                    int lastPageNo;
                    if (filteredItemCount == 0)
                        lastPageNo = 0;
                    else
                        lastPageNo = (filteredItemCount / RowsPerPage) - (filteredItemCount % RowsPerPage == 0 ? 1 : 0);
                    CurrentPage = lastPageNo < CurrentPage ? lastPageNo : CurrentPage;
                }

                return GetItemsOfPage(CurrentPage, RowsPerPage);
            }
        }

        protected IEnumerable<T> GetItemsOfPage(int n, int pageSize)
        {
            if (n < 0 || pageSize <= 0)
                return Array.Empty<T>();

            if (ServerData != null)
                return _server_data.Items;

            return FilteredItems.Skip(n * pageSize).Take(pageSize);
        }

        protected override int NumPages
        {
            get
            {
                if (ServerData != null)
                    return (int)Math.Ceiling(_server_data.TotalItems / (double)RowsPerPage);

                return (int)Math.Ceiling(FilteredItems.Count() / (double)RowsPerPage);
            }
        }

        public override int GetFilteredItemsCount()
        {
            if (ServerData != null)
                return _server_data.TotalItems;
            return FilteredItems.Count();
        }

        public override void SetSelectedItem(object item)
        {
            SelectedItem = item.As<T>();
        }

        public override void SetEditingItem(object item)
        {
            if (!Object.ReferenceEquals(_editingItem, item))
                _editingItem = item;
        }

        public override TableContext TableContext
        {
            get
            {
                Context.Table = this;
                Context.TableStateHasChanged = this.StateHasChanged;
                return Context;
            }
        }

        // TableContext provides shared functionality between all table sub-components
        public TableContext<T> Context { get; } = new TableContext<T>();

        private void OnRowCheckboxChanged(bool value, T item)
        {
            if (value)
                Context.Selection.Add(item);
            else
                Context.Selection.Remove(item);
            SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        internal override void OnHeaderCheckboxClicked(bool value)
        {
            if (!value)
                Context.Selection.Clear();
            else
            {
                foreach (var item in FilteredItems)
                    Context.Selection.Add(item);
            }
            Context.UpdateRowCheckBoxes(false);
            SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
                await InvokeServerLoadFunc();

            TableContext.UpdateRowCheckBoxes();
            await base.OnAfterRenderAsync(firstRender);
        }


        /// <summary>
        /// Supply an async function which (re)loads filtered, paginated and sorted data from server.
        /// Table will await this func and update based on the returned TableData.
        /// Used only with ServerData
        /// </summary>
        [Parameter] public Func<TableState, Task<TableData<T>>> ServerData { get; set; }

        internal override bool HasServerData => ServerData != null;


        TableData<T> _server_data = new TableData<T>() { TotalItems = 0, Items = Array.Empty<T>() };
        private IEnumerable<T> _items;

        internal override async Task InvokeServerLoadFunc()
        {
            if (ServerData == null)
                return;

            var label = Context.CurrentSortLabel;

            var state = new TableState
            {
                Page = CurrentPage,
                PageSize = RowsPerPage,
                SortDirection = Context.SortDirection,
                SortLabel = label?.SortLabel
            };

            _server_data = await ServerData(state);
            StateHasChanged();
            Context?.PagerStateHasChanged?.Invoke();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender)
                Context?.PagerStateHasChanged?.Invoke();
        }

        /// <summary>
        /// Call this to reload the server-filtered, -sorted and -paginated items
        /// </summary>
        public Task ReloadServerData()
        {
            return InvokeServerLoadFunc();
        }

        internal override bool IsEditable { get => RowEditingTemplate != null; }
    }
}
