// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// Represents a vertical set of values.
    /// </summary>
    /// <typeparam name="T">The kind of item for this column.</typeparam>
    /// <seealso cref="MudDataGrid{T}"/>
    public abstract partial class Column<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T> : MudComponentBase, IDisposable
    {
        private static readonly RenderFragment<CellContext<T>> EmptyChildContent = _ => builder => { };
        internal ParameterState<bool> HiddenState { get; }
        internal ParameterState<bool> GroupingState { get; }
        internal ParameterState<bool> _groupExpandedState;
        internal ParameterState<int> _groupByOrderState;

        /// <summary>
        /// The data grid which owns this column.
        /// </summary>
        [CascadingParameter]
        public MudDataGrid<T> DataGrid { get; set; } = null!;

        /// <summary>
        /// The value stored in this column.
        /// </summary>
        [Parameter] public T? Value { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Value"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<T> ValueChanged { get; set; }

        /// <summary>
        /// The display text for this column.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        /// <summary>
        /// Hides this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        public bool HideSmall { get; set; }

        /// <summary>
        /// The number of columns spanned by this column in the footer.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.
        /// </remarks>
        [Parameter] public int FooterColSpan { get; set; } = 1;

        /// <summary>
        /// The number of columns spanned by this column in the header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>1</c>.
        /// </remarks>
        [Parameter] public int HeaderColSpan { get; set; } = 1;

        /// <summary>
        /// The template used to display this column's header.
        /// </summary>
        [Parameter]
        public RenderFragment<HeaderContext<T>>? HeaderTemplate { get; set; }

        public virtual RenderFragment<HeaderContext<T>>? GetHeaderTemplate() => HeaderTemplate;

        /// <summary>
        /// The template used to display this column's value cells.
        /// </summary>
        [Parameter]
        public RenderFragment<CellContext<T>>? CellTemplate { get; set; }

        public virtual RenderFragment<CellContext<T>>? GetCellTemplate() => CellTemplate;

        /// <summary>
        /// The template used to display this column's footer.
        /// </summary>
        [Parameter]
        public RenderFragment<FooterContext<T>>? FooterTemplate { get; set; }

        public virtual RenderFragment<FooterContext<T>>? GetFooterTemplate() => FooterTemplate;

        /// <summary>
        /// The template used to display this column's grouping.
        /// </summary>
        [Parameter]
        public RenderFragment<GroupDefinition<T>>? GroupTemplate { get; set; }

        public virtual RenderFragment<GroupDefinition<T>>? GetGroupTemplate() => GroupTemplate;

        /// <summary>
        /// The template used to display this column's aggregate.
        /// </summary>
        [Parameter]
        public RenderFragment<IEnumerable<T>>? AggregateTemplate { get; set; }

        public virtual RenderFragment<IEnumerable<T>>? GetAggregateTemplate() => AggregateTemplate;

        /// <summary>
        /// The function which groups values in this column.
        /// </summary>
        [Parameter]
        public Func<T, object>? GroupBy { get; set; }

        /// <summary>
        /// The order in which values are grouped when there are more than one group
        /// </summary>
        /// <remarks>
        /// Defaults to 0.
        /// </remarks>
        [Parameter, ParameterState]
        public int GroupByOrder { get; set; }

        /// <summary>
        /// Occurs when the <see cref="GroupByOrder"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> GroupByOrderChanged { get; set; }

        /// <summary>
        /// Indents the column <c>48px</c> beyond its parent when grouped.
        /// </summary>
        [Parameter]
        public bool GroupIndented { get; set; } = true;

        /// <summary>
        /// Whether groups created from this column are expanded. Toggling the value will Toggle all grouped rows of this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter, ParameterState]
        public bool GroupExpanded { get; set; }

        /// <summary>
        /// Occurs when the <see cref="GroupExpanded"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> GroupExpandedChanged { get; set; }

        /// <summary>
        /// Requires a value to be set.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Required { get; set; } = true;

        #region HeaderCell Properties

        /// <summary>
        /// The CSS class applied to the header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Separate multiple classes with spaces.
        /// </remarks>
        [Parameter]
        public string? HeaderClass { get; set; }

        /// <summary>
        /// The function which calculates CSS classes for the header.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Separate multiple classes with spaces.
        /// </remarks>
        [Parameter]
        public Func<IEnumerable<T>, string>? HeaderClassFunc { get; set; }

        /// <summary>
        /// The CSS style applied to this column's header.
        /// </summary>
        [Parameter]
        public string? HeaderStyle { get; set; }

        /// <summary>
        /// The function which calculates CSS styles for the header.
        /// </summary>
        [Parameter]
        public Func<IEnumerable<T>, string>? HeaderStyleFunc { get; set; }

        /// <summary>
        /// Sorts values in this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, this overrides the <see cref="MudDataGrid{T}.SortMode"/> property.
        /// </remarks>
        [Parameter]
        public virtual bool? Sortable { get; set; }

        /// <summary>
        /// Allows this column's width to be changed.
        /// </summary>
        [Parameter]
        public virtual bool? Resizable { get; set; }

        /// <summary>
        /// Allows this column to be reordered via drag-and-drop operations.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, this overrides the <see cref="MudDataGrid{T}.DragDropColumnReordering"/> property.
        /// </remarks>
        [Parameter]
        public virtual bool? DragAndDropEnabled { get; set; }

        /// <summary>
        /// Allows filters to be used on this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, this overrides the <see cref="MudDataGrid{T}.Filterable"/> property.
        /// </remarks>
        [Parameter]
        public virtual bool? Filterable { get; set; }

        /// <summary>
        /// Shows the filter icon.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, this overrides the <see cref="MudDataGrid{T}.ShowFilterIcons"/> property.
        /// </remarks>
        [Parameter]
        public bool? ShowFilterIcon { get; set; }

        /// <summary>
        /// Allows this column to be hidden.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, this overrides the <see cref="MudDataGrid{T}.Hideable"/> property.
        /// </remarks>
        [Parameter]
        public bool? Hideable { get; set; }

        /// <summary>
        /// Hides this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter, ParameterState]
        public bool Hidden { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Hidden"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> HiddenChanged { get; set; }

        /// <summary>
        /// Shows options for this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, this overrides the <see cref="MudDataGrid{T}.ShowColumnOptions"/> property.
        /// </remarks>
        [Parameter] public virtual bool? ShowColumnOptions { get; set; }

        /// <summary>
        /// The comparison used for values in this column.
        /// </summary>
        [Parameter]
        public IComparer<object?>? Comparer { get; set; } = null;

        /// <summary>
        /// The function used to sort values in this column.
        /// </summary>
        [Parameter]
        public Func<T, object?> SortBy
        {
            get
            {
                return GetLocalSortFunc();
            }
            set
            {
                _sortBy = value;
            }
        }

        /// <summary>
        /// The sorting direction applied when <see cref="Sortable"/> is <c>true</c>.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="SortDirection.None"/>.
        /// </remarks>
        [Parameter]
        public SortDirection InitialDirection { get; set; } = SortDirection.None;

        /// <summary>
        /// The icon shown when <see cref="Sortable"/> is <c>true</c>.
        /// </summary>
        [Parameter]
        public string SortIcon { get; set; } = Icons.Material.Filled.ArrowUpward;

        /// <summary>
        /// Allows values in this column to be grouped.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When set, this overrides the <see cref="MudDataGrid{T}.Groupable"/> property.
        /// </remarks>
        [Parameter]
        public bool? Groupable { get; set; }

        /// <summary>
        /// Indicates whether this column is currently grouped.
        /// </summary>
        [Parameter, ParameterState]
        public bool Grouping { get; set; }

        /// <summary>
        /// Occurs when the <see cref="Grouping"/> property has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> GroupingChanged { get; set; }

        /// <summary>
        /// Fixes this column to the left side.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, this column will be visible even as the container is scrolled horizontally.
        /// </remarks>
        [Parameter]
        public bool StickyLeft { get; set; }

        /// <summary>
        /// Fixes this column to the right side.
        /// </summary>
        /// <remarks>
        /// When <c>true</c>, this column will be visible even as the container is scrolled horizontally.
        /// </remarks>
        [Parameter]
        public bool StickyRight { get; set; }

        /// <summary>
        /// The template used to display this column's filter.
        /// </summary>
        [Parameter]
        public RenderFragment<FilterContext<T>>? FilterTemplate { get; set; }

        /// <summary>
        /// The operators to use for this column's filter.
        /// </summary>
        [Parameter]
        public HashSet<string> FilterOperators { get; set; } = [];

        /// <summary>
        /// The unique identifier for this column.
        /// </summary>
        public string? Identifier { get; set; }

        private CultureInfo? _culture;

        /// <summary>
        /// The culture used to parse, filter, and display values in this column.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MudDataGrid{T}.Culture"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public CultureInfo? Culture
        {
            get => _culture ?? DataGrid?.Culture;
            set
            {
                _culture = value;
            }
        }

        #endregion

        #region Cell Properties

        /// <summary>
        /// The CSS classes to apply to the cell.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public string? CellClass { get; set; }

        /// <summary>
        /// The function used to determine CSS classes for this cell.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public Func<T, string>? CellClassFunc { get; set; }

        /// <summary>
        /// The CSS styles to apply to this cell.
        /// </summary>
        [Parameter]
        public string? CellStyle { get; set; }

        /// <summary>
        /// The function which calculates CSS styles for this cell.
        /// </summary>
        [Parameter]
        public Func<T, string>? CellStyleFunc { get; set; }

        /// <summary>
        /// Allows editing for this cell.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        public bool Editable { get; set; } = true;

        /// <summary>
        /// The template for editing values in this cell.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When <see cref="MudDataGrid{T}.EditMode"/> is <see cref="DataGridEditMode.Form"/>, the built-in Save button
        /// automatically invokes <see cref="MudDataGrid{T}.CommittedItemChanges"/> — no extra handling is required.
        /// </para>
        /// <para>
        /// When <see cref="MudDataGrid{T}.EditMode"/> is <see cref="DataGridEditMode.Cell"/>, standard columns commit
        /// their value automatically on change. Custom controls inside an <see cref="EditTemplate"/> do not trigger
        /// <see cref="MudDataGrid{T}.CommittedItemChanges"/> automatically. Track changes directly in a value-changed
        /// handler instead:
        /// </para>
        /// <code>
        /// &lt;EditTemplate&gt;
        ///     &lt;MudDatePicker Date="@context.Item.Date"
        ///         DateChanged="@(d => { context.Item.Date = d; TrackChange(context.Item); })" /&gt;
        /// &lt;/EditTemplate&gt;
        /// </code>
        /// </remarks>
        [Parameter]
        public RenderFragment<CellContext<T>>? EditTemplate { get; set; }

        #endregion

        #region FooterCell Properties

        /// <summary>
        /// The CSS classes applied to this column's footer.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public string? FooterClass { get; set; }

        /// <summary>
        /// The function which calculates CSS classes for this column's footer.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        public Func<IEnumerable<T>, string>? FooterClassFunc { get; set; }

        /// <summary>
        /// The CSS styles to apply to this column's footer.
        /// </summary>
        [Parameter]
        public string? FooterStyle { get; set; }

        /// <summary>
        /// The function which calculates CSS styles for this column's footer.
        /// </summary>
        [Parameter]
        public Func<IEnumerable<T>, string>? FooterStyleFunc { get; set; }

        /// <summary>
        /// Allows the footer to be selected.
        /// </summary>
        [Parameter]
        public bool EnableFooterSelection { get; set; }

        /// <summary>
        /// The function which calculates aggregates for this column.
        /// </summary>
        [Parameter]
        public AggregateDefinition<T>? AggregateDefinition { get; set; }

        #endregion

        internal string HeaderClassname =>
            new CssBuilder("mud-table-cell")
                .AddClass("mud-table-cell-hide", HideSmall)
                .AddClass("sticky-left", StickyLeft)
                .AddClass("sticky-right", StickyRight)
                .AddClass(Class)
                .Build();

        internal string FooterClassname =>
            new CssBuilder("mud-table-cell")
                .AddClass("footer-cell")
                .AddClass("mud-table-cell-hide", HideSmall)
                .AddClass(Class)
                .Build();

        #region Computed Properties

        internal Type dataType
        {
            get
            {
                Debug.Assert(PropertyType is not null);
                return PropertyType;
            }
        }

        internal bool isNumber
        {
            get
            {
                return TypeIdentifier.IsNumber(PropertyType);
            }
        }

        internal bool hideable
        {
            get
            {
                return Hideable ?? DataGrid?.Hideable ?? false;
            }
        }

        internal bool sortable
        {
            get
            {
                return Sortable ?? (DataGrid?.SortMode != SortMode.None);
            }
        }

        internal bool groupable
        {
            get
            {
                return Groupable ?? DataGrid?.Groupable ?? false;
            }
        }

        internal bool filterable
        {
            get
            {
                return Filterable ?? DataGrid?.Filterable ?? false;
            }
        }

        #endregion

        internal int SortIndex { get; set; } = -1;
        internal HeaderCell<T> HeaderCell { get; set; } = null!;

        private Func<T, object?>? _sortBy;
        internal Func<T, object?>? groupBy;

        // These are set in OnInitialized() so they can't be null
        internal HeaderContext<T> headerContext = null!;
        private FilterContext<T> filterContext = null!;
        internal FooterContext<T> footerContext = null!;

        // Cached filter definition to avoid repeated lookups during rendering
        private IFilterDefinition<T>? _cachedFilterDefinition;

        /// <summary>
        /// The context used for filtering values in this column.
        /// </summary>
        public FilterContext<T> FilterContext
        {
            get
            {
                Debug.Assert(DataGrid is not null);

                // Check if the cached filter definition is still valid in the grid's FilterDefinitions
                var existingFilterDefinition = DataGrid.FilterDefinitions.FirstOrDefault(fd => fd.Column == this);

                if (existingFilterDefinition != null)
                {
                    // Use the existing filter definition from the grid
                    if (_cachedFilterDefinition != existingFilterDefinition)
                    {
                        _cachedFilterDefinition = existingFilterDefinition;
                        filterContext.FilterDefinition = existingFilterDefinition;
                    }
                }
                else
                {
                    // No filter exists in the grid - check if we have a stale reference or need to create a new one
                    if (_cachedFilterDefinition != null || filterContext.FilterDefinition == null)
                    {
                        // Clear the stale cached reference and create a new filter definition
                        _cachedFilterDefinition = null;
                        var operators = GetFilterOperators(FieldType.Identify(PropertyType));
                        var filterDefinition = DataGrid.CreateFilterDefinitionInstance();
                        filterDefinition.Title = Title;
                        filterDefinition.Operator = operators.FirstOrDefault();
                        filterDefinition.Column = this;
                        filterContext.FilterDefinition = filterDefinition;
                    }
                }

                return filterContext;
            }
        }

        protected Column()
        {
            using var registerScope = CreateRegisterScope();
            HiddenState = registerScope.RegisterParameter<bool>(nameof(Hidden))
                .WithParameter(() => Hidden)
                .WithEventCallback(() => HiddenChanged);
            GroupingState = registerScope.RegisterParameter<bool>(nameof(Grouping))
                .WithParameter(() => Grouping)
                .WithEventCallback(() => GroupingChanged)
                .WithChangeHandler(OnGroupingParameterChangedAsync);
            _groupExpandedState = registerScope.RegisterParameter<bool>(nameof(GroupExpanded))
                .WithParameter(() => GroupExpanded)
                .WithChangeHandler(OnGroupExpandedChangedAsync);
            _groupByOrderState = registerScope.RegisterParameter<int>(nameof(GroupByOrder))
                .WithParameter(() => GroupByOrder)
                .WithChangeHandler(OnGroupByOrderChangedAsync);
        }

        private void OnGroupingParameterChangedAsync() => DataGrid?.GroupItems();

        private void OnGroupByOrderChangedAsync() => DataGrid?.GroupItems();

        private void OnGroupExpandedChangedAsync() => DataGrid?.GroupItems();

        protected override void OnInitialized()
        {
            Debug.Assert(DataGrid is not null);

            if (FilterOperators.Count > 0)
            {
                var defaultOperators = FilterOperator.GetOperatorByDataType(PropertyType);
                var invalidOperators = FilterOperators.Where(@operator => !defaultOperators.Contains(@operator)).ToArray();

                if (invalidOperators.Length > 0)
                {
                    throw new ArgumentException($"Invalid filter operators for {PropertyType?.Name}: {string.Join(", ", invalidOperators)}");
                }
            }

            base.OnInitialized();

            groupBy = GroupBy;

            DataGrid.AddColumn(this);

            // Add the HeaderContext
            headerContext = new HeaderContext<T>(DataGrid);

            // Add the FilterContext
            filterContext = new FilterContext<T>(DataGrid);

            // Add the FooterContext
            footerContext = new FooterContext<T>(DataGrid);
        }

        internal IReadOnlyCollection<string> GetFilterOperators(FieldType fieldType)
        {
            if (FilterOperators.Count == 0)
            {
                return FilterOperator.GetOperatorByDataType(fieldType);
            }
            else
            {
                return FilterOperators;
            }
        }

        internal Func<T, object?> GetLocalSortFunc()
        {
            if (_sortBy == null)
            {
                if (this is TemplateColumn<T>)
                {
                    _sortBy = x => true;
                }
                else
                    _sortBy = PropertyFunc;
            }

            return _sortBy;
        }

        internal void CompileGroupBy()
        {
            if (groupBy == null && !string.IsNullOrWhiteSpace(PropertyName))
            {
                var type = typeof(T);
                // set the default GroupBy
                if (type == typeof(IDictionary<string, object>))
                {
                    groupBy = x => (x as IDictionary<string, object>)?[PropertyName];
                }
                else
                {
                    groupBy = PropertyFunc;
                }
            }
        }

        // Allows child components to change column grouping.
        internal async Task SetGroupingAsync(bool group)
        {
            await GroupingState.SetValueAsync(group);
        }

        /// <summary>
        /// This method's sole purpose is for the DataGrid to remove grouping in mass.
        /// </summary>
        internal async Task RemoveGrouping()
        {
            if (GroupingState.Value)
            {
                await GroupingState.SetValueAsync(false);
            }
        }

        /// <summary>
        /// Hides this column.
        /// </summary>
        public Task HideAsync()
        {
            return HiddenState.SetValueAsync(true);
        }

        /// <summary>
        /// Shows this column.
        /// </summary>
        public Task ShowAsync()
        {
            return HiddenState.SetValueAsync(false);
        }

        /// <summary>
        /// Hides or shows this column.
        /// </summary>
        public async Task ToggleAsync()
        {
            Debug.Assert(DataGrid is not null);
            await HiddenState.SetValueAsync(!HiddenState.Value);
            ((IMudStateHasChanged)DataGrid).StateHasChanged();
        }

        /// <summary>
        /// Releases resources used by this column.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by this column.
        /// </summary>
        /// <param name="disposing">When <c>true</c>, managed resources should be released.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (DataGrid != null)
                DataGrid.RemoveColumn(this);
        }

        #region Abstract Members

        protected internal virtual LambdaExpression? PropertyExpression { get; }

        protected internal virtual Func<T, bool> GetFilterExpression()
        {
            return x => true;
        }

        /// <summary>
        /// The name of the property used for sorting this column's values.
        /// </summary>
        public virtual string? PropertyName { get; }

        protected internal virtual string? ContentFormat { get; }

        protected internal abstract object? CellContent(T item);

        protected internal abstract object? PropertyFunc(T item);

        protected internal virtual Type? PropertyType { get; }

        protected internal abstract void SetProperty(object? item, object? value);

        #endregion
    }
}
