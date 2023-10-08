// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor
{
    public class MudThemeService
    {
#nullable enable
        public MudThemeProvider? Provider { get; set; }

        public void Attach(MudThemeProvider provider)
        {
            if (provider == null)
            {
                return;
            }
            Provider = provider;
        }

        public void Detach()
        {
            Provider = null;
        }
    }
}
