using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class IconButton : MudBaseButton
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-icon-button")
          .AddClass($"mud-icon-button-color-{Color.ToDescriptionString()}")
          .AddClass($"mud-ripple mud-ripple-icon", !DisableRipple)
          .AddClass($"mud-icon-button-size-{Size.ToDescriptionString()}", when: () => Size != Size.Medium)
          .AddClass($"mud-icon-button-edge-{Edge.ToDescriptionString()}", when: () => Edge != Edge.False)
          .AddClass(Class)
        .Build();

        [Parameter] public string Icon { get; set; }

        [Parameter] public string ToggleIcon { get; set; }

        [Parameter] public Color Color { get; set; } = Color.Default;

        [Parameter] public Size Size { get; set; } = Size.Medium;

        [Parameter] public Edge Edge { get; set; }

        [Parameter] public bool DisableRipple { get; set; }

        [Parameter] public bool Disabled { get; set; }

    }
}
