using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudText : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-typography")
          .AddClass($"mud-typography-{Typo.ToDescriptionString()}")
          .AddClass($"mud-{Color.ToDescriptionString()}-text")
          .AddClass("mud-typography-gutterbottom", GutterBottom)
          .AddClass($"mud-typography-align-{ConvertAlign(Align).ToDescriptionString()}", Align != Align.Inherit)
          .AddClass("mud-typography-display-inline", Inline)
          .AddClass(Class)
        .Build();

        private Align ConvertAlign(Align align)
        {
            return align switch
            {
                Align.Start => RightToLeft ? Align.Right : Align.Left,
                Align.End => RightToLeft ? Align.Left : Align.Right,
                _ => align
            };
        }

        [CascadingParameter] public bool RightToLeft { get; set; }

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
        [Parameter] public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// If true, Sets display inline
        /// </summary>
        [Parameter] public bool Inline { get; set; }
    }
}
