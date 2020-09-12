using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public class ComponentBaseIcon : ComponentBase
    {
        protected string Classname =>
        new CssBuilder("")
          .AddClass($"mud-svg-icon-root", !WebFont)
          .AddClass($"mud-icon-size-{IconSize.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [Parameter] public string Icon { get; set; }
        [Parameter] public bool WebFont { get; set; }
        [Parameter] public IconSize IconSize { get; set; } = IconSize.Default;
        [Parameter] public string Class { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            if (WebFont)
            {
                throw new ArgumentNullException("Not implemented yet");
            }
        }
    }
}
