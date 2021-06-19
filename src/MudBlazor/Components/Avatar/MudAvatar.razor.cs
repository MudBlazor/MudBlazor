using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    partial class MudAvatar : MudComponentBase
    {
        protected string Classname =>
        new CssBuilder("mud-avatar")
          .AddClass($"mud-avatar-{Size.ToDescriptionString()}")
          .AddClass($"mud-avatar-rounded", Rounded)
          .AddClass($"mud-avatar-square", Square)
          .AddClass($"mud-avatar-{Variant.ToDescriptionString()}")
          .AddClass($"mud-avatar-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
          .AddClass(Class)
        .Build();

        /// <summary>
        /// If true, border-radius is set to 0.
        /// </summary>
        [Parameter] public bool Square { get; set; }

        /// <summary>
        /// If true, border-radius is set to the themes default value.
        /// </summary>
        [Parameter] public bool Rounded { get; set; }

        /// <summary>
        /// Link to image, if set a image will be displayed instead of text.
        /// </summary>
        [Parameter] public string Image { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the MudAvatar.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Filled;

        /// <summary>
        /// Child content of the component.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }
    }
}
