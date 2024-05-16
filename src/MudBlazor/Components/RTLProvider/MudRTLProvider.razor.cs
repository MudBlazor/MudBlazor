// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a language support provider for Right-to-Left (RTL) languages such as Arabic, Hebrew, and Persian.
    /// </summary>
    public partial class MudRTLProvider : MudComponentBase
    {
        private readonly ParameterState<bool> _rtlState;

        public MudRTLProvider()
        {
            var registerScope = CreateRegisterScope();
            _rtlState = registerScope.RegisterParameter<bool>(nameof(RightToLeft))
                .WithParameter(() => RightToLeft)
                .WithChangeHandler(OnRightToLeftParameterChange);
        }

        protected string Classname =>
            new CssBuilder("mud-rtl-provider")
                .AddClass("mud-application-layout-rtl", RightToLeft)
                .AddClass(Class)
                .Build();

        /// <summary>
        /// Displays text Right-to-Left (RTL).
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When <c>true</c>, text will display properly for RTL languages such as Arabic, Hebrew, and Persian.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.RTLProvider.Behavior)]
        public bool RightToLeft { get; set; }

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.RTLProvider.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        private void OnRightToLeftParameterChange()
        {
            UserAttributes["dir"] = RightToLeft ? "rtl" : "ltr";
        }
    }
}
