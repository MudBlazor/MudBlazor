// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudSection : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-section")
            .AddClass($"mud-section-col-start-{Column.ToString()}")
            .AddClass($"mud-section-col-end-{(Column + ColSpan).ToString()}")
            .AddClass($"mud-section-row-start-{Row.ToString()}")
            .AddClass($"mud-section-row-end-{(Row + RowSpan).ToString()}")
            .AddClass(Class)
        .Build();

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int Column { get; set; } = 1;

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int ColSpan { get; set; } = 1;

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int Row { get; set; } = 1;

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int RowSpan { get; set; } = 1;

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public bool OnClickStopPropagation { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public EventCallback OnContextMenu { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public bool OnContextMenuPreventDefault { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Behavior)]
        public bool OnContextMenuStopPropagation { get; set; }

        protected async Task OnClickHandler(MouseEventArgs ev)
        {
            await OnClick.InvokeAsync(ev);
        }
    }
}
