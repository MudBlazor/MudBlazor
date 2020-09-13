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
          .AddClass($"mud-button-{Variant.ToDescriptionString()}-size-{Size.ToDescriptionString()}")
          .AddClass($"mud-ripple", !DisableRipple)
          .AddClass($"mud-button-disable-elevation", DisableElevation)
          .AddClass(Class)
        .Build();

        protected string IconClassname =>
        new CssBuilder()
          .AddClass($"mud-button-start-icon", !String.IsNullOrEmpty(StartIcon))
          .AddClass($"mud-button-end-icon", !String.IsNullOrEmpty(EndIcon))
          .AddClass($"mud-button-icon-size-{Size.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [Parameter] public string StartIcon { get; set; }

        [Parameter] public string EndIcon { get; set; }

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
