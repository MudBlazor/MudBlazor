// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.Components.Snackbar
{
    public class SnackbarMessage
    {
        internal Guid SnackbarMessageId { get; private set; } = Guid.NewGuid();
        public RenderFragment RenderFragment { get; }

        internal SnackbarMessage(RenderFragment renderFragment)
        {
            RenderFragment = renderFragment;
        }
    }
}
