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

        protected T Def => Items.First();
        //protected ItemType Def => new ItemType();
        /// <summary>
        /// Text to show when there are no items to show
        /// </summary>
        [Parameter] public string EmptyDataText { get; set; }

        /// <summary>
        /// Gets or sets the data source that the DataTable is displaying data for.
        /// </summary>
        [Parameter]
        public List<T> DataSource
        {
            get { return Items; }
            set
            {
                Items = value;
                StateHasChanged();
            }
        }

        protected List<T> Items { set; get; }
        /// <summary>
        /// Gets or sets a value indicating whether table footer is displayed.
        /// </summary>
        [Parameter]
        public bool ShowFooter{ set; get; }
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
        #endregion
        #region Templates
        [Parameter] public RenderFragment<T> Columns { get; set; }
        #endregion
    }
}
