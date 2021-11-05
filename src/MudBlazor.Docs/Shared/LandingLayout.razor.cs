// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Extensions;
using MudBlazor.Docs.Models;

namespace MudBlazor.Docs.Shared
{
    public partial class LandingLayout : LayoutComponentBase
    {
        protected override void OnInitialized()
        {
            _currentTheme = Theme.LandingPageLightTheme;
        }
        private void DarkMode()
        {
            if (_currentTheme == Theme.LandingPageLightTheme)
            {
                _currentTheme = Theme.DocsDarkTheme;
                isDarkMode = true;
            }
            else
            {
                _currentTheme = Theme.LandingPageLightTheme;
                isDarkMode = false;
            }
        }

        private bool isDarkMode;
        private MudTheme _currentTheme = new();
    }
}
