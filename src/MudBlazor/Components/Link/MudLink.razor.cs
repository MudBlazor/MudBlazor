using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudLink : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-typography mud-link")
           .AddClass($"mud-{Color.ToDescriptionString()}-text")
          .AddClass($"mud-link-underline-{Underline.ToDescriptionString()}")
          .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Typography variant to use.
        /// </summary>
        [Parameter] public Typo Typo { get; set; } = Typo.body1;

        /// <summary>
        /// Controls when the link should have an underline.
        /// </summary>
        [Parameter] public Underline Underline { get; set; } = Underline.Hover;

        /// <summary>
        /// The URL, which is the actual link.
        /// </summary>
        [Parameter] public string Href { get; set; }

        /// <summary>
        /// The target attribute specifies where to open the link, if Link is specified. Possible values: _blank | _self | _parent | _top | <i>framename</i>
        /// </summary>
        [Parameter] public string Target { get; set; }

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
