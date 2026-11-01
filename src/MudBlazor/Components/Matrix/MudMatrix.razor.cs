// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudMatrix : MudComponentBase
    {
        protected string Classname =>
            new CssBuilder("mud-gridv2")
                .AddClass(Class)
                .Build();

        protected string Styles =>
            new StyleBuilder()
                .AddStyle("grid-template-columns", TemplateColumns, !string.IsNullOrEmpty(TemplateColumns))
                .AddStyle("grid-template-rows", TemplateRows, !string.IsNullOrEmpty(TemplateRows))
                .AddStyle("grid-template-rows", TemplateAreas, !string.IsNullOrEmpty(TemplateAreas))
                .AddStyle("grid-column-gap", ColumnGap, string.IsNullOrEmpty(Gap) && !string.IsNullOrEmpty(ColumnGap))
                .AddStyle("grid-row-gap", RowGap, string.IsNullOrEmpty(Gap) && !string.IsNullOrEmpty(RowGap))
                .AddStyle("grid-gap", Gap, !string.IsNullOrEmpty(Gap))
                .AddStyle("justify-items", JustifyItems.ToDescriptionString())
                .AddStyle("align-items", AlignItems.ToDescriptionString())
                .AddStyle("justify-content", JustifyContent.ToDescriptionString())
                .AddStyle("align-content", AlignContent.ToDescriptionString())
                .AddStyle(Style)
                .Build();

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-template-columns")]
        public string TemplateColumns { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-template-rows")]
        public string TemplateRows { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-template-rows")]
        public string TemplateAreas { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-column-gap")]
        public string ColumnGap { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-row-gap")]
        public string RowGap { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("grid-gap")]
        public string Gap { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("justify-items")]
        public Justify JustifyItems { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("align-items")]
        public Align AlignItems { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("justify-content")]
        public Justify JustifyContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        [StyleAttribute("align-content")]
        public Align AlignContent { get; set; }

        /// <summary>
        /// The number of columns spanned by this grid.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        public int ColumnCount { get; set; }

        /// <summary>
        /// The number of rows spanned by this grid.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        public int RowCount { get; set; }

        [Parameter]
        [Category(CategoryTypes.Grid.Behavior)]
        public RenderFragment ChildContent { get; set; }
    }
}
