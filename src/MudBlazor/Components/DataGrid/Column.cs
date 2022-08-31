// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    [RequiresUnreferencedCode(CodeMessage.SerializationUnreferencedCodeMessage)]
    public partial class Column<T> : MudComponentBase
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }

        //[CascadingParameter(Name = "HeaderCell")] public HeaderCell<T> HeaderCell { get; set; }

        [Parameter] public T Value { get; set; }
        [Parameter] public EventCallback<T> ValueChanged { get; set; }

        //[Parameter] public bool Visible { get; set; } = true;

        /// <summary>
        /// Specifies the name of the object's property bound to the column
        /// </summary>
        [Parameter] public string Field { get; set; }

        [Parameter] public Type FieldType { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public bool HideSmall { get; set; }
        [Parameter] public int FooterColSpan { get; set; } = 1;
        [Parameter] public int HeaderColSpan { get; set; } = 1;
        [Parameter] public RenderFragment ChildContent { get; set; }
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
                if (FieldType != null)
                    return FieldType;

                if (Field == null)
                    return typeof(object);

                if (typeof(T) == typeof(IDictionary<string, object>) && FieldType == null)
                    throw new ArgumentNullException(nameof(FieldType));

                var t = typeof(T).GetProperty(Field).PropertyType;
                return Nullable.GetUnderlyingType(t) ?? t;
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
                    var first = DataGrid.Items.FirstOrDefault(x => ((IDictionary<string, object>)x)[Field] != null);

                    if (first != null)
                    {
                        return ((IDictionary<string, object>)first)[Field].GetType();
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
                return FilterOperator.NumericTypes.Contains(dataType);
            }
        }

        internal string computedTitle
        {
            get
            {
                return Title ?? Field;
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

        private Func<T, object> _sortBy;
        internal Func<T, object> groupBy;
        internal HeaderContext<T> headerContext;
        internal FilterContext<T> filterContext;
        internal FooterContext<T> footerContext;

        [UnconditionalSuppressMessage("Trimming", "IL2046: 'RequiresUnreferencedCodeAttribute' annotations must match across all interface implementations or overrides.", Justification = "Suppressing because we annotating the whole component with RequiresUnreferencedCodeAttribute for information that generic type must be preserved.")]
        protected override void OnInitialized()
        {
            if (!Hideable.HasValue)
                Hideable = DataGrid?.Hideable;

            groupBy = GroupBy;
            CompileGroupBy();

            if (groupable && Grouping)
                grouping = Grouping;

            if (null != DataGrid)
                DataGrid.AddColumn(this);

            // Add the HeaderContext
            headerContext = new HeaderContext<T>(DataGrid);

            // Add the FilterContext
            if (filterable)
            {
                filterContext = new FilterContext<T>(DataGrid);
                var operators = FilterOperator.GetOperatorByDataType(dataType);
                filterContext.FilterDefinition = new FilterDefinition<T>()
                {
                    Field = Field,
                    FieldType = FieldType,
                    Title = Title,
                    Operator = operators.FirstOrDefault()
                };
            }

            // Add the FooterContext
            footerContext = new FooterContext<T>(DataGrid);
        }

        internal Func<T, object> GetLocalSortFunc()
        {
            if (null == _sortBy)
            {
                var type = typeof(T);

                // set the default SortBy
                if (type == typeof(IDictionary<string, object>))
                {
                    if (FieldType == null)
                        throw new ArgumentNullException(nameof(FieldType));

                    var innerType = innerDataType;

                    if (innerType == typeof(JsonElement))
                    {
                        _sortBy = x =>
                        {
                            var json = (JsonElement)(x as IDictionary<string, object>)[Field];

                            if (FieldType == typeof(string))
                                return json.GetString();
                            else if (isNumber)
                                return json.GetDouble();
                            else
                                return json.GetRawText();
                        };
                    }
                    else
                    {
                        _sortBy = x => Convert.ChangeType((x as IDictionary<string, object>)[Field], FieldType);
                    }
                }
                else
                {
                    var parameter = Expression.Parameter(type, "x");
                    var field = Expression.Convert(Expression.Property(parameter, type.GetProperty(Field)), typeof(object));
                    _sortBy = Expression.Lambda<Func<T, object>>(field, parameter).Compile();
                }
            }

            return _sortBy;
        }

        internal void CompileGroupBy()
        {
            if (groupBy == null && !string.IsNullOrWhiteSpace(Field))
            {
                var type = typeof(T);

                // set the default GroupBy
                if (type == typeof(IDictionary<string, object>))
                {
                    groupBy = x => (x as IDictionary<string, object>)[Field];
                }
                else
                {
                    var parameter = Expression.Parameter(type, "x");
                    var field = Expression.Convert(Expression.Property(parameter, type.GetProperty(Field)), typeof(object));
                    groupBy = Expression.Lambda<Func<T, object>>(field, parameter).Compile();
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
            DataGrid.ExternalStateHasChanged();
        }
    }
}
