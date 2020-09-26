using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudLink : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-typography mud-link")
           .AddClass($"mud-color-text-{Color.ToDescriptionString()}")
          .AddClass($"mud-link-underline-{Underline.ToDescriptionString()}")
          .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [Parameter] public Color Color { get; set; } = Color.Primary;

        [Parameter] public Typo Typo { get; set; } = Typo.body1;

        [Parameter] public Underline Underline { get; set; } = Underline.Hover;

        [Parameter] public string Href { get; set; }

        [Parameter] public string Target { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
