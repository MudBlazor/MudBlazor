// Copyright (c) 2020 Jonny Larsson
// License: MIT
// See https://github.com/Garderoben/MudBlazor
// Modified version of Blazored Modal
// Copyright (c) 2019 Blazored
// License: MIT
// See https://github.com/Blazored


using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudDialog
    {
        protected string ContentClass =>
        new CssBuilder("mud-dialog-content")
          .AddClass($"mud-dialog-no-side-padding", DisableSidePadding)
          .AddClass(ClassContent)
        .Build();

        protected string ActionClass =>
        new CssBuilder("mud-dialog-actions")
          .AddClass(ClassActions)
        .Build();

        [Parameter] public RenderFragment DialogContent { get; set; }
        [Parameter] public RenderFragment DialogActions { get; set; }

        [Parameter] public bool DisableSidePadding { get; set; }
        [Parameter] public string ClassContent { get; set; }
        [Parameter] public string ClassActions { get; set; }
    }
}
