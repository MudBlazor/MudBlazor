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
        public string MarkupString { get; }
        public RenderFragment RenderFragment { get; }

        internal SnackbarMessage(string markupString)
        {
            MarkupString = markupString;
        }

        internal SnackbarMessage(RenderFragment renderFragment)
        {
            RenderFragment = renderFragment;
        }
    }
}
