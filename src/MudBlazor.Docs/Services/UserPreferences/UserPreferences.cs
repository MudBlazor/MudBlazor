// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Docs.Services.UserPreferences
{
    public class UserPreferences
    {
        /// <summary>
        /// Set the direction layout of the docs to RTL or LTR. If true RTL is used
        /// </summary>
        public bool RightToLeft { get; set; }
        
        /// <summary>
        /// If true DarkTheme is used. LightTheme otherwise
        /// </summary>
        public bool DarkTheme { get; set; }
    }
}
