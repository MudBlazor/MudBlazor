using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudIcon : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("")
          .AddClass($"mud-svg-icon-root", !String.IsNullOrEmpty(Icon))
          .AddClass($"mud-icon-root", !String.IsNullOrEmpty(FontIcon))
          .AddClass($"mud-color-text-{Color.ToDescriptionString()}")
          .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
          .AddClass(FontClass, !String.IsNullOrEmpty(FontClass))
          .AddClass(Class)
        .Build();

        [Parameter] public string Icon { get; set; }
        [Parameter] public string FontIcon { get; set; }
        [Parameter] public string FontClass { get; set; }
        [Parameter] public Size Size { get; set; } = Size.Medium;
        [Parameter] public Color Color { get; set; } = Color.Inherit;

        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
