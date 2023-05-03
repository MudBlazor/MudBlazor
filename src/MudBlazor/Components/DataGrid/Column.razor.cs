// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public abstract partial class Column<T> : MudComponentBase
    {
        private static readonly RenderFragment<CellContext<T>> EmptyChildContent = _ => builder => { };

        internal readonly Guid uid = Guid.NewGuid();

        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }

        //[CascadingParameter(Name = "HeaderCell")] public HeaderCell<T> HeaderCell { get; set; }

        [Parameter] public T Value { get; set; }
        [Parameter] public EventCallback<T> ValueChanged { get; set; }

        //[Parameter] public bool Visible { get; set; } = true;

        /// <summary>
        /// Specifies the name of the object's property bound to the column
        /// </summary>
        //[Parameter] public string Field { get; set; }

        //[Parameter] public Type FieldType { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public bool HideSmall { get; set; }
        [Parameter] public int FooterColSpan { get; set; } = 1;
        [Parameter] public int HeaderColSpan { get; set; } = 1;
        [Parameter] public RenderFragment<HeaderContext<T>> HeaderTemplate { get; set; }
        [Parameter] public RenderFragment<CellContext<T>> CellTemplate { get; set; }
        [Parameter] public RenderFragment<FooterContext<T>> FooterTemplate { get; set; }
        [Parameter] public RenderFragment<GroupDefinition<T>> GroupTemplate { get; set; }
        [Parameter] public Func<T, object> GroupBy { get; set; }

        #region HeaderCell Properties

        [Parameter] public string HeaderClass { get; set; }
        [Parameter] public Func<T, string> HeaderClassFunc { get; set; }
        [Parameter] public string HeaderStyle { get; set; }
        [Parameter] public Func<T, string> HeaderStyleFunc { get; set; }

        /// <summary>
        /// Determines whether this columns data can be sorted. This overrides the Sortable parameter on the DataGrid.
        /// </summary>
        [Parameter] public bool? Sortable { get; set; }

        [Parameter] public bool? Resizable { get; set; }

        /// <summary>
        /// If set this will override the DragDropColumnReordering parameter of MudDataGrid which applies to all columns.
        /// Set true to enable reordering for this column. Set false to disable it. 
        /// </summary>
        [Parameter] public bool? DragAndDropEnabled { get; set; }
        /// <summary>
        /// Determines whether this columns data can be filtered. This overrides the Filterable parameter on the DataGrid.
        /// </summary>
        [Parameter] public bool? Filterable { get; set; }

        [Parameter] public bool? ShowFilterIcon { get; set; }

        /// <summary>
        /// Determines whether this column can be hidden. This overrides the Hideable parameter on the DataGrid.
        /// </summary>
        [Parameter] public bool? Hideable { get; set; }

        [Parameter] public bool Hidden { get; set; }
        [Parameter] public EventCallback<bool> HiddenChanged { get; set; }

        /// <summary>
        /// Determines whether to show or hide column options. This overrides the ShowColumnOptions parameter on the DataGrid.
        /// </summary>
        [Parameter] public bool? ShowColumnOptions { get; set; }

        [Parameter]
        public IComparer<object> Comparer
        {
            get => _comparer;
            set => _comparer = value;
        }

        [Parameter]
        public Func<T, object> SortBy
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
        [Parameter] public SortDirection InitialDirection { get; set; } = SortDirection.None;
        [Parameter] public string SortIcon { get; set; } = Icons.Material.Filled.ArrowUpward;

        /// <summary>
        /// Specifies whether the column can be grouped.
        /// </summary>
        [Parameter] public bool? Groupable { get; set; }

        /// <summary>
        /// Specifies whether the column is grouped.
        /// </summary>
        [Parameter] public bool Grouping { get; set; }

        /// <summary>
        /// Specifies whether the column is sticky.
        /// </summary>
        [Parameter] public bool StickyLeft { get; set; }

        [Parameter] public bool StickyRight { get; set; }

        [Parameter] public RenderFragment<FilterContext<T>> FilterTemplate { get; set; }

        public string Identifier { get; set; }
        

        private CultureInfo _culture;
        /// <summary>
        /// The culture used to represent this column and by the filtering input field.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Table.Appearance)]
        public CultureInfo Culture
        {
            get => _culture ?? DataGrid?.Culture;
            set
            {
                _culture = value;
            }
        }
        #endregion

        #region Cell Properties

        [Parameter] public string CellClass { get; set; }
        [Parameter] public Func<T, string> CellClassFunc { get; set; }
        [Parameter] public string CellStyle { get; set; }
        [Parameter] public Func<T, string> CellStyleFunc { get; set; }
        [Parameter] public bool IsEditable { get; set; } = true;
        [Parameter] public RenderFragment<CellContext<T>> EditTemplate { get; set; }

        #endregion

        #region FooterCell Properties

        [Parameter] public string FooterClass { get; set; }
        [Parameter] public Func<T, string> FooterClassFunc { get; set; }
        [Parameter] public string FooterStyle { get; set; }
        [Parameter] public Func<T, string> FooterStyleFunc { get; set; }
        [Parameter] public bool EnableFooterSelection { get; set; }
        [Parameter] public AggregateDefinition<T> AggregateDefinition { get; set; }

        #endregion

        public Action ColumnStateHasChanged { get; set; }

        internal string headerClassname =>
            new CssBuilder("mud-table-cell")
                .AddClass("mud-table-cell-hide", HideSmall)
                .AddClass("sticky-left", StickyLeft)
                .AddClass("sticky-right", StickyRight)
                .AddClass(Class)
            .Build();

        internal string cellClassname;
        //internal string cellClassname =>
        //    new CssBuilder("mud-table-cell")
        //        .AddClass("mud-table-cell-hide", HideSmall)
        //        .AddClass("sticky-right", StickyRight)
        //        .AddClass(Class)
        //    .Build();

        internal string footerClassname =>
            new CssBuilder("mud-table-cell")
                .AddClass("mud-table-cell-hide", HideSmall)
                .AddClass(Class)
            .Build();

        internal bool grouping;

        #region Computed Properties

        internal Type dataType
        {
            get
            {
                return PropertyType;
            }
        }

        // This returns the data type for an object when T is an IDictionary<string, object>.
        internal Type innerDataType
        {
            get
            {
                // Handle case where T is IDictionary.
                if (typeof(T) == typeof(IDictionary<string, object>))
                {
                    // We need to get the actual type here so we need to look at actual data.
                    // get the first item where we have a non-null value in the field to be filtered.
                    var first = DataGrid.Items.FirstOrDefault(x => ((IDictionary<string, object>)x)[PropertyName] != null);

                    if (first != null)
                    {
                        return ((IDictionary<string, object>)first)[PropertyName].GetType();
                    }
                    else
                    {
                        return typeof(object);
                    }
                }

                return dataType;
            }
        }

        internal bool isNumber
        {
            get
            {
                return TypeIdentifier.IsNumber(PropertyType);
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
        internal HeaderCell<T> HeaderCell { get; set; }

        private IComparer<object> _comparer = null;
        private Func<T, object> _sortBy;
        internal Func<T, object> groupBy;
        internal HeaderContext<T> headerContext;
        private FilterContext<T> filterContext;
        internal FooterContext<T> footerContext;

        public FilterContext<T> FilterContext
        {
            get
            {
                // Make sure that when we access filterContext properties, they have been defined...
                if (filterContext.FilterDefinition == null)
                {
                    var operators = FilterOperator.GetOperatorByDataType(PropertyType);
                    filterContext.FilterDefinition = new FilterDefinition<T>()
                    {
                        DataGrid = DataGrid,
                        //Field = PropertyName,
                        //FieldType = PropertyType,
                        Title = Title,
                        Operator = operators.FirstOrDefault(),
                        PropertyExpression = PropertyExpression,
                        Column = this,
                    };
                }

                return filterContext;
            }
        }

        protected override void OnInitialized()
        {
            if (!Hideable.HasValue)
                Hideable = DataGrid?.Hideable;

            groupBy = GroupBy;

            if (groupable && Grouping)
                grouping = Grouping;

            if (null != DataGrid)
                DataGrid.AddColumn(this);

            // Add the HeaderContext
            headerContext = new HeaderContext<T>(DataGrid);

            // Add the FilterContext
            //if (filterable)
            //{
            //    filterContext = new FilterContext<T>(DataGrid);
            //    var operators = FilterOperator.GetOperatorByDataType(dataType);
            //    filterContext.FilterDefinition = new FilterDefinition<T>()
            //    {
            //        DataGrid = this.DataGrid,
            //        Field = PropertyName,
            //        FieldType = dataType,
            //        Title = Title,
            //        Operator = operators.FirstOrDefault()
            //    };
            //}

            // Add the FilterContext
            filterContext = new FilterContext<T>(DataGrid);

            // Add the FooterContext
            footerContext = new FooterContext<T>(DataGrid);
        }

        internal Func<T, object> GetLocalSortFunc()
        {
            if (null == _sortBy)
            {
                if (this is TemplateColumn<T>)
                {
                    _sortBy = x => true;
                }
                else
                    _sortBy = x => PropertyFunc(x);
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
                    groupBy = x => (x as IDictionary<string, object>)[PropertyName];
                }
                else
                {
                    groupBy = x => PropertyFunc(x);
                }
            }
        }

        // Allows child components to change column grouping.
        internal void SetGrouping(bool g)
        {
            if (groupable)
            {
                grouping = g;
                DataGrid?.ChangedGrouping(this);
            }
        }

        /// <summary>
        /// This method's sole purpose is for the DataGrid to remove grouping in mass.
        /// </summary>
        internal void RemoveGrouping()
        {
            grouping = false;
        }

        public async Task HideAsync()
        {
            Hidden = true;
            await HiddenChanged.InvokeAsync(Hidden);
        }

        public async Task ShowAsync()
        {
            Hidden = false;
            await HiddenChanged.InvokeAsync(Hidden);
        }

        public async Task ToggleAsync()
        {
            Hidden = !Hidden;
            await HiddenChanged.InvokeAsync(Hidden);
            ((IMudStateHasChanged)DataGrid).StateHasChanged();
        }


        #region Abstract Members

#nullable enable
        protected internal virtual LambdaExpression? PropertyExpression { get; }
#nullable disable

        protected internal virtual Func<T, bool> GetFilterExpression()
        {
            return x => true;
        }

        public virtual string PropertyName { get; }

#nullable enable
        protected internal virtual string? ContentFormat { get; }
#nullable disable

        protected internal abstract object CellContent(T item);

        protected internal abstract object PropertyFunc(T item);

        protected internal virtual Type PropertyType { get; }

        protected internal virtual string FullPropertyName { get; }

        protected internal abstract void SetProperty(object item, object value);

        #endregion
    }
}
