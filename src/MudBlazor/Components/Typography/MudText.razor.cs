using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

using MudBlazor.Utilities;
using MudBlazor.Extensions;

namespace MudBlazor
{
    public partial class MudText : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-typography")
          .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
          .AddClass($"mud-{Color.ToDescriptionString()}-text")
          .AddClass("mud-typography-gutterbottom", GutterBottom)
          .AddClass($"mud-typography-align-{Align.ToDescriptionString()}", Align != Align.Inherit)
          .AddClass("mud-typography-display-inline", Inline)
          .AddClass(Class)
        .Build();

        /// <summary>
        /// Applies the theme typography styles.
        /// </summary>
        [Parameter] public Typo Typo { get; set; } = Typo.body1;

        /// <summary>
        /// Set the text-align on the component.
        /// </summary>
        [Parameter] public Align Align { get; set; } = Align.Inherit;

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Inherit;

        /// <summary>
        /// If true, the text will have a bottom margin.
        /// </summary>
        [Parameter] public bool GutterBottom { get; set; } = false;

        /// <summary>
        /// Child content of component.
        /// </summary>
        [Parameter]  public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If true, Sets display inine
        /// </summary>
        [Parameter] public bool Inline { get; set; }
    }
}
