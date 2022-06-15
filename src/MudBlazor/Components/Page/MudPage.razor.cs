// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudPage : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-page")
            .AddClass($"mud-page-column-{Column.ToString()}")
            .AddClass($"mud-page-row-{Row.ToString()}")
            .AddClass($"mud-page-height-{FullScreen.ToDescriptionString()}")
            .AddClass(Class)
        .Build();

        protected string Stylename =>
        new StyleBuilder()
            .AddStyle("height", $"{Height}", !String.IsNullOrEmpty(Height))
            .AddStyle(Style)
        .Build();

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int Column { get; set; } = 4;

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public int Row { get; set; } = 4;

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public FullScreen FullScreen { get; set; } = FullScreen.None;

        [Parameter]
        [Category(CategoryTypes.Item.Appearance)]
        public string Height { get; set; }

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
