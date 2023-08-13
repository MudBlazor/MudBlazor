// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudRTLProvider : MudComponentBase
    {
        private bool _rtl;

        protected string Classname =>
            new CssBuilder("mud-rtl-provider")
                .AddClass("mud-application-layout-rtl", RightToLeft)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// If true, changes the layout to RightToLeft.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.RTLProvider.Behavior)]
        public bool RightToLeft
        {
            get => _rtl;
            set
            {
                _rtl = value;
                UserAttributes["dir"] = RightToLeft ? "rtl" : "ltr";
            }
        }

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.RTLProvider.Behavior)]
        public RenderFragment? ChildContent { get; set; }
    }
}
