﻿using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudCardActions : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-card-actions")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
