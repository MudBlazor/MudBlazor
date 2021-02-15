using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudDataTable<T> : MudComponentBase
    {
        protected string Classname =>
       new CssBuilder("mud-datatable")
           //.AddClass($"mud-sm-table", Breakpoint == Breakpoint.Sm)
           //.AddClass($"mud-md-table", Breakpoint == Breakpoint.Md)
           //.AddClass($"mud-lg-table", Breakpoint == Breakpoint.Lg)
           //.AddClass($"mud-xl-table", Breakpoint == Breakpoint.Xl)
           .AddClass($"mud-datatable-dense", Dense)
         //.AddClass($"mud-table-hover", Hover)
         //.AddClass($"mud-table-outlined", Outlined)
         //.AddClass($"mud-table-square", Square)
         //.AddClass($"mud-table-sticky-header", FixedHeader)
         //.AddClass($"mud-elevation-{Elevation.ToString()}", !Outlined)
         .AddClass(Class)
       .Build();

        protected T Def
        {
            get
            {
                T t;
                T t1 = default(T);
                if (t1 == null)
                {
                    t = Activator.CreateInstance<T>();
                }
                else
                {
                    t1 = default(T);
                    t = t1;
                }
                return t;
            }
        }

        protected MudTable<T> mudTable;
        //protected ItemType Def => new ItemType();
        /// <summary>
        /// Text to show when there are no items to show
        /// </summary>
        [Parameter] public string EmptyDataText { get; set; }

        /// <summary>
        /// Gets or sets the data source that the DataTable is displaying data for.
        /// </summary>
        [Parameter]
        public IEnumerable<T> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                StateHasChanged();
            }
        }
        protected IEnumerable<T> _items { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether table footer is displayed.
        /// </summary>
        [Parameter]
        public bool ShowFooter { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether the pagination feature is enabled.
        /// </summary>
        [Parameter]
        public bool UsePageing { set; get; } = false;
        /// <summary>
        /// Gets or sets a value indicating whether editing feature is enabled.
        /// </summary>
        [Parameter]
        public bool ReadOnly { set; get; }
        #region Parameters forwarded to internal MudTable

        /// <summary>
        /// Set true for rows with a narrow height
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// Set true to see rows hover on mouse-over.
        /// </summary>
        [Parameter] public bool Hover { get; set; }
        /// <summary>
        /// At what breakpoint the table should switch to mobile layout. Takes Xs, Sm, Md, Lg and Xl the default behavior is breaking on Xs.
        /// </summary>
        [Parameter] public Breakpoint Breakpoint { get; set; } = Breakpoint.Xs;
        /// <summary>
        /// Supply an async function which (re)loads filtered, paginated and sorted data from server.
        /// Table will await this func and update based on the returned TableData.
        /// Used only with ServerData
        /// </summary>
        [Parameter] public Func<TableState, Task<TableData<T>>> ServerData { get; set; }
        /// <summary>
        /// Call this to reload the server-filtered, -sorted and -paginated items
        /// </summary>
        public Task ReloadServerData()
        {
            return mudTable.ReloadServerData();
        }
        #endregion
        #region Templates
        [Parameter] public RenderFragment<T> Columns { get; set; }
        #endregion
    }
}
