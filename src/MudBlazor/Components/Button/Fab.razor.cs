using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseFab : CommonButtonBase
    {
        protected string Classname =>
        new CssBuilder("mud-fab-root mud-fab")
          .AddClass($"mud-fab-extended", !String.IsNullOrEmpty(Label))
          .AddClass($"mud-fab-{Color.ToDescriptionString()}")
          .AddClass($"mud-fab-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-ripple", !DisableRipple && !Disabled)
          .AddClass($"mud-fab-disable-elevation", DisableElevation)
          .AddClass(Class)
        .Build();

        [Parameter] public Color Color { get; set; } = Color.Default;

        [Parameter] public Size Size { get; set; } = Size.Large;

        [Parameter] public string Icon { get; set; }

        [Parameter] public string Label { get; set; }

        [Parameter] public string Class { get; set; }

        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Allow more content (i.e. an additional text label) instead of just a single icon
        /// </summary>

        [Parameter] public bool DisableElevation { get; set; }

        [Parameter] public bool DisableRipple { get; set; }

    }
}
