using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudActionArea : MudBaseButton
    {
        protected string Classname =>
        new CssBuilder("mud-action-area")
          .AddClass($"mud-disabled", Disabled)
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass(Class)
        .Build();

        /// <summary>
        /// If true, the area will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}
