// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Docs.Shared
{
    public partial class LandingLayout : LayoutComponentBase
    {
        protected override void OnInitialized()
        {
            _currentTheme = _test.LandingPageTheme(false);
        }
        private bool _isDarkMode;
        private void DarkMode()
        {
            _isDarkMode = !_isDarkMode;
            _currentTheme = _test.LandingPageTheme(_isDarkMode);
        }
        Theme _test = new();
        MudTheme _currentTheme;
    }
}
