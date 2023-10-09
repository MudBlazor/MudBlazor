// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

namespace MudBlazor
{
    public class MudThemeService
    {
#nullable enable
        public MudThemeProvider? Provider { get; internal set; }

        public void Attach(MudThemeProvider? provider)
        {
            ArgumentNullException.ThrowIfNull(provider);
            Provider = provider;
        }

        public void Detach()
        {
            Provider = null;
        }
    }
}
