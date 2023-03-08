using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;


namespace MudBlazor
{
    // note: the MudTable code is split. Everything depending on the type parameter T of MudTable<T> is here in MudTable<T>

    public partial class MudTable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T> : MudTableBase
    {
        /// <summary>
        /// Defines how a table row looks like. Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public RenderFragment<T> RowTemplate { get; set; }

        /// <summary>
        /// Row Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public RenderFragment<T> ChildRowContent { get; set; }

        /// <summary>
        /// Defines how a table row looks like in edit mode (for selected row). Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Editing)]
        public RenderFragment<T> RowEditingTemplate { get; set; }

        #region Code for column based approach
        /// <summary>
        /// Defines how a table column looks like. Columns components should inherit from MudBaseColumn
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public RenderFragment<T> Columns { get; set; }
        /// <summary>
        /// Comma separated list of columns to show if there is no templates defined
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public string QuickColumns { get; set; }

        // Workaround because "where T : new()" didn't work with Blazor components
        // T must have a default constructor, otherwise we cannot show headers when Items collection
        // is empty
        protected T Def
        {
            get
            {
                T t1 = default;
                if (t1 == null)
                {
                    return Activator.CreateInstance<T>();
                }
                else
                {
                    return default;
                }
            }
        }
        /// <summary>
        /// Creates a default Column renderfragment if there is no templates defined
        /// </summary>
        protected override void OnInitialized()
        {
            if (Columns == null && RowTemplate == null && RowEditingTemplate == null)
            {
                string[] quickcolumnslist = null;
                if (!QuickColumns.IsEmpty())
                {
                    quickcolumnslist = QuickColumns.Split(",");
                }
                // Create template from T
                Columns = context => builder =>
                {
                    var myType = context.GetType();
                    IList<PropertyInfo> propertylist = new List<PropertyInfo>(myType.GetProperties().Where(p => p.PropertyType.IsPublic));

                    if (quickcolumnslist == null)
                    {
                        foreach (var propinfo in propertylist)
                        {
                            BuildMudColumnTemplateItem(context, builder, propinfo);
                        }
                    }
                    else
                    {
                        foreach (var colname in quickcolumnslist)
                        {
                            var propinfo = propertylist.SingleOrDefault(pl => pl.Name == colname);
                            if (propinfo != null)
                            {
                                BuildMudColumnTemplateItem(context, builder, propinfo);
                            }
                        }
                    }

                };
            }
        }

        private static void BuildMudColumnTemplateItem(T context, RenderTreeBuilder builder, PropertyInfo propinfo)
        {
            if (propinfo.PropertyType.IsPrimitive || propinfo.PropertyType == typeof(string))
            {
                builder.OpenComponent<MudColumn<string>>(0);
                builder.AddAttribute(1, "Value", propinfo.GetValue(context)?.ToString());
                builder.AddAttribute(2, "HeaderText", propinfo.Name);
                builder.CloseComponent();
            }
        }
        #endregion
        /// <summary>
        /// Defines the table body content when there are no matching records found
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public RenderFragment NoRecordsContent { get; set; }

        /// <summary>
        /// Defines the table body content  the table has no rows and is loading
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public RenderFragment LoadingContent { get; set; }

        /// <summary>
        /// Defines if the table has a horizontal scrollbar.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Behavior)]
        public bool HorizontalScrollbar { get; set; }

        internal string GetHorizontalScrollbarStyle() => HorizontalScrollbar ? ";display: block; overflow-x: auto;" : string.Empty;

        /// <summary>
        /// The data to display in the table. MudTable will render one row per item
        /// </summary>
        /// 
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
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
        [Parameter]
        [Category(CategoryTypes.Table.Filtering)]
        public Func<T, bool> Filter { get; set; } = null;

        /// <summary>
        /// Row click event.
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
        [Category(CategoryTypes.Table.Rows)]
        public Func<T, int, string> RowClassFunc { get; set; }

        /// <summary>
        /// Returns the style that will get joined with RowStyle. Takes the current item and row index.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Rows)]
        public Func<T, int, string> RowStyleFunc { get; set; }

        /// <summary>
        /// Returns the item which was last clicked on in single selection mode (that is, if MultiSelection is false)
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Selecting)]
        public T SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_comparer != null && _comparer.Equals(SelectedItem, value))
                    return;
                else if (EqualityComparer<T>.Default.Equals(SelectedItem, value))
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
        [Category(CategoryTypes.Table.Selecting)]
        public HashSet<T> SelectedItems
        {
            get
            {
                if (!MultiSelection)
                    if (_selectedItem is null)
                        return new HashSet<T>(Array.Empty<T>(), _comparer);
                    else
                        return new HashSet<T>(new T[] { _selectedItem }, _comparer);
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
                    Context.Selection = new HashSet<T>(_comparer);
                }
                else
                    Context.Selection = value;
                SelectedItemsChanged.InvokeAsync(Context.Selection);
                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// The Comparer to use for comparing selected items internally.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.FormComponent.Behavior)]
        public IEqualityComparer<T> Comparer
        {
            get => _comparer;
            set
            {
                if (value == _comparer) return;
                _comparer = value;
                // Apply comparer and (selected values are refreshed in the Context.Comparer setter)
                Context.Comparer = _comparer;
            }
        }

        private IEqualityComparer<T> _comparer;

        /// <summary>
        /// Callback is called whenever items are selected or deselected in multi selection mode.
        /// </summary>
        [Parameter] public EventCallback<HashSet<T>> SelectedItemsChanged { get; set; }

        private TableGroupDefinition<T> _groupBy;
        /// <summary>
        /// Defines data grouping parameters. It can has N hierarchical levels
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public TableGroupDefinition<T> GroupBy
        {
            get => _groupBy;
            set
            {
                _groupBy = value;
                if (_groupBy != null)
                    _groupBy.Context = Context;
            }
        }

        /// <summary>
        /// Defines how a table grouping row header looks like. It works only when GroupBy is not null. Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public RenderFragment<TableGroupData<object, T>> GroupHeaderTemplate { get; set; }

        /// <summary>
        /// Defines custom CSS classes for using on Group Header's MudTr.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string GroupHeaderClass { get; set; }

        /// <summary>
        /// Defines custom styles for using on Group Header's MudTr.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string GroupHeaderStyle { get; set; }

        /// <summary>
        /// Defines custom CSS classes for using on Group Footer's MudTr.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string GroupFooterClass { get; set; }

        /// <summary>
        /// Defines custom styles for using on Group Footer's MudTr.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public string GroupFooterStyle { get; set; }

        /// <summary>
        /// Defines how a table grouping row footer looks like. It works only when GroupBy is not null. Use MudTd to define the table cells and their content.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Grouping)]
        public RenderFragment<TableGroupData<object, T>> GroupFooterTemplate { get; set; }

        private IEnumerable<T> _preEditSort { get; set; } = null;
        private bool _hasPreEditSort => _preEditSort != null;

        public IEnumerable<T> FilteredItems
        {
            get
            {
                if (IsEditing && _hasPreEditSort)
                    return _preEditSort;
                if (HasServerData)
                {
                    _preEditSort = _server_data.Items?.ToList();
                    return _preEditSort;
                }

                if (Filter == null)
                {
                    _preEditSort = Context.Sort(Items).ToList();
                    return _preEditSort;
                }
                _preEditSort = Context.Sort(Items.Where(Filter)).ToList();
                return _preEditSort;
            }
        }

        protected IEnumerable<T> CurrentPageItems
        {
            get
            {
                if (@PagerContent == null)
                    return FilteredItems; // we have no pagination
                if (!HasServerData)
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

            if (HasServerData)
                return _server_data.Items;

            return FilteredItems.Skip(n * pageSize).Take(pageSize);
        }

        protected override int NumPages
        {
            get
            {
                if (HasServerData)
                    return (int)Math.Ceiling(_server_data.TotalItems / (double)RowsPerPage);

                return (int)Math.Ceiling(FilteredItems.Count() / (double)RowsPerPage);
            }
        }

        public override int GetFilteredItemsCount()
        {
            if (HasServerData)
                return _server_data.TotalItems;
            return FilteredItems.Count();
        }

        public override void SetSelectedItem(object item)
        {
            SelectedItem = item.As<T>();
        }

        public override void SetEditingItem(object item)
        {
            if (!ReferenceEquals(_editingItem, item))
                _editingItem = item;
        }

        public override bool ContainsItem(object item)
        {
            var t = item.As<T>();
            if (t is null)
                return false;
            return FilteredItems?.Contains(t) ?? false;
        }

        public override void UpdateSelection() => SelectedItemsChanged.InvokeAsync(SelectedItems);

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

        private void OnRowCheckboxChanged(bool checkedState, T item)
        {
            if (checkedState)
                Context.Selection.Add(item);
            else
                Context.Selection.Remove(item);

            if (SelectedItemsChanged.HasDelegate)
                SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        internal override void OnHeaderCheckboxClicked(bool checkedState)
        {
            if (checkedState)
            {
                foreach (var item in FilteredItems)
                    Context.Selection.Add(item);
            }
            else
                Context.Selection.Clear();

            Context.UpdateRowCheckBoxes();

            if (SelectedItemsChanged.HasDelegate)
                SelectedItemsChanged.InvokeAsync(SelectedItems);
        }

        /// <summary>
        /// Supply an async function which (re)loads filtered, paginated and sorted data from server.
        /// Table will await this func and update based on the returned TableData.
        /// Used only with ServerData
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Data)]
        public Func<TableState, Task<TableData<T>>> ServerData { get; set; }

        internal override bool HasServerData => ServerData != null;


        TableData<T> _server_data = new() { TotalItems = 0, Items = Array.Empty<T>() };
        private IEnumerable<T> _items;

        internal override async Task InvokeServerLoadFunc()
        {
            if (!HasServerData)
                return;

            Loading = true;
            await InvokeAsync(StateHasChanged);
            var label = Context.CurrentSortLabel;

            var state = new TableState
            {
                Page = CurrentPage,
                PageSize = RowsPerPage,
                SortDirection = Context.SortDirection,
                SortLabel = label?.SortLabel
            };

            _server_data = await ServerData(state);

            if (CurrentPage * RowsPerPage > _server_data.TotalItems)
                CurrentPage = 0;

            Loading = false;
            StateHasChanged();
            Context?.PagerStateHasChanged?.Invoke();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (!firstRender)
                Context?.PagerStateHasChanged?.Invoke();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
                await InvokeServerLoadFunc();
            TableContext.UpdateRowCheckBoxes(updateGroups: false);
        }

        /// <summary>
        /// Call this to reload the server-filtered, -sorted and -paginated items
        /// </summary>
        public Task ReloadServerData()
        {
            return InvokeServerLoadFunc();
        }

        internal override bool IsEditable { get => (RowEditingTemplate != null) || (Columns != null); }

        //GROUPING:
        private IEnumerable<IGrouping<object, T>> GroupItemsPage
        {
            get
            {
                return GetItemsOfGroup(GroupBy, CurrentPageItems);
            }
        }

        internal IEnumerable<IGrouping<object, T>> GetItemsOfGroup(TableGroupDefinition<T> parent, IEnumerable<T> sourceList)
        {
            if (parent == null || sourceList == null)
                return new List<IGrouping<object, T>>();

            return sourceList.GroupBy(parent.Selector).ToList();
        }

        internal void OnGroupHeaderCheckboxClicked(bool checkedState, IEnumerable<T> items)
        {
            if (checkedState)
            {
                foreach (var item in items)
                    Context.Selection.Add(item);
            }
            else
            {
                foreach (var item in items)
                    Context.Selection.Remove(item);
            }

            Context.UpdateRowCheckBoxes();

            if (SelectedItemsChanged.HasDelegate)
                SelectedItemsChanged.InvokeAsync(SelectedItems);
        }
    }
}
