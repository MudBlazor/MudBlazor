using Microsoft.AspNetCore.Components;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudIconButton : MudBaseButton
    {
        protected string Classname =>
        new CssBuilder("mud-button-root mud-icon-button")
          .AddClass($"mud-icon-button-color-{Color.ToDescriptionString()}", Color != Color.Default)
          .AddClass($"mud-ripple mud-ripple-icon", !DisableRipple)
          .AddClass($"mud-icon-button-size-{Size.ToDescriptionString()}", when: () => Size != Size.Medium)
          .AddClass($"mud-icon-button-edge-{Edge.ToDescriptionString()}", when: () => Edge != Edge.False)
          .AddClass(Class)
        .Build();

        /// <summary>
        /// The Icon that will be used in the component.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// Title of the icon used for accessibility.
        /// </summary>
        [Parameter] public string Title { get; set; }

        /// <summary>
        /// The color of the component. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the component.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// If set uses a negative margin.
        /// </summary>
        [Parameter] public Edge Edge { get; set; }

        /// <summary>
        /// Child content of component, only shows if Icon is null or Empty.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}
