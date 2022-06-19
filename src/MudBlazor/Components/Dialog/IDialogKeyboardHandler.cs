// Copyright (c) 2019 Blazored (https://github.com/Blazored)
// Copyright (c) 2020 Jonny Larsson (https://github.com/MudBlazor/MudBlazor)
// Copyright (c) 2021 improvements by Meinrad Recheis
// See https://github.com/Blazored
// License: MIT

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor
{
    /// <summary>
    /// Handler for Dialog Keyboard events
    /// all MudDialogInstances implementing this interface can consume KeyDown events
    /// if they are the top level dialog
    /// </summary>
    internal interface IDialogKeyboardHandler
    {
        void HandleKeyDown(KeyboardEventArgs e);
    }
}
