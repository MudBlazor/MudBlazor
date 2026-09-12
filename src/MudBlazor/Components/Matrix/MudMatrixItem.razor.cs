// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudMatrixItem : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-gridv2-item")
                .AddClass(Class)
                .Build();

        protected string Styles =>
            new StyleBuilder().AddStyle(Style)
                .AddStyle("grid-column-start", ColumnStart,
                    string.IsNullOrEmpty(Column) && !string.IsNullOrEmpty(ColumnStart))
                .AddStyle("grid-column-end", ColumnEnd,
                    string.IsNullOrEmpty(Column) && !string.IsNullOrEmpty(ColumnEnd))
                .AddStyle("grid-column", Column, !string.IsNullOrEmpty(Column))
                .AddStyle("grid-row-start", RowStart, string.IsNullOrEmpty(Row) && !string.IsNullOrEmpty(RowStart))
                .AddStyle("grid-row-end", RowEnd, string.IsNullOrEmpty(Row) && !string.IsNullOrEmpty(RowEnd))
                .AddStyle("grid-row", Row, !string.IsNullOrEmpty(Row))
                .AddStyle("grid-area", Area, !string.IsNullOrEmpty(Area))
                .AddStyle("justify-self", JustifySelf.ToDescriptionString())
                .AddStyle("align-self", AlignSelf.ToDescriptionString())
                .AddStyle(Style)
                .Build();

        [CascadingParameter] private MudMatrix Parent { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-column-start")]
        public string ColumnStart { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-column-end")]
        public string ColumnEnd { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-column")]
        public string Column { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-row-start")]
        public string RowStart { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grow-row-end")]
        public string RowEnd { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-row")]
        public string Row { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("grid-area")]
        public string Area { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("justify-self")]
        public Justify JustifySelf { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        [StyleAttribute("align-self")]
        public Align AlignSelf { get; set; }
        
        /// <summary>
        /// The number of columns spanned by this grid item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public int ColumnSpan { get; set; }
        
        /// <summary>
        /// The number of rows spanned by this grid item.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public int RowSpan { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public RenderFragment ChildContent { get; set; }
    }
}
