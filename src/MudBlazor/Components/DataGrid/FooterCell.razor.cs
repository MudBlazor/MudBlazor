// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class FooterCell<T> : MudComponentBase
    {
        [CascadingParameter] public MudDataGrid<T> DataGrid { get; set; }

        [Parameter] public Column<T> Column { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public IEnumerable<T> CurrentItems { get; set; }

        private string _classname =>
            new CssBuilder("footer-cell")
                .AddClass(Column?.FooterClass)
                .AddClass(Column?.footerClassname)
                .AddClass(Class)
            .Build();
        private string _style =>
            new StyleBuilder()
                .AddStyle(Column?.FooterStyle)
                .AddStyle(Style)
                .AddStyle("font-weight", "600")
            .Build();

        internal IEnumerable<T> items
        {
            get
            {
                return CurrentItems ?? DataGrid?.CurrentPageItems;
            }
        }
    }
}
