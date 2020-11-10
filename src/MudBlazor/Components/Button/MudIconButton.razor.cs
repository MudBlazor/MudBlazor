using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;
using MudBlazor.Extensions;

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
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter] public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter] public bool Disabled { get; set; }

        /// <summary>
        /// Child content of component, only shows if Icon is null or Empty.
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

    }
}
