using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public class ComponentBaseButton : CommonButtonBase
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-button")
          .AddClass($"mud-button-{Variant.ToDescriptionString()}")
          .AddClass($"mud-button-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
          .AddClass($"mud-button-{Variant.ToDescriptionString()}size-{Size.ToDescriptionString()}")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass($"mud-button-disable-elevation", DisableElevation)
          .AddClass(Class)
        .Build();

        [Parameter] public string Icon { get; set; }

        [Parameter] public Color Color { get; set; } = Color.Default;

        [Parameter] public Size Size { get; set; } = Size.Medium;

        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        [Parameter] public string Class { get; set; }

        [Parameter] public bool Disabled { get; set; }

        [Parameter] public bool DisableElevation { get; set; }

        [Parameter] public bool DisableRipple { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}
