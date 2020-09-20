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
          .AddClass($"mud-svg-icon-root", !WebFont)
          .AddClass($"mud-icon-size-{Size.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        [Parameter] public string Icon { get; set; }
        [Parameter] public bool WebFont { get; set; }
        [Parameter] public Size Size { get; set; } = Size.Medium;

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
