using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
    public partial class MudToggleIconButton : MudComponentBase
    {
        /// <summary>
        /// The toggled value.
        /// </summary>
        [Parameter] public bool Toggled { get; set; }

        /// <summary>
        /// Fires whenever toggled is changed. 
        /// </summary>
        [Parameter] public EventCallback<bool> ToggledChanged { get; set; }

        /// <summary>
        /// The Icon that will be used in the untoggled state.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// The Icon that will be used in the toggled state.
        /// </summary>
        [Parameter] public string ToggledIcon { get; set; }

        /// <summary>
        /// Title of the icon used for accessibility.
        /// </summary>
        [Parameter] public string Title { get; set; }

        /// <summary>
        /// Title used in toggled state, if different.
        /// </summary>
        [Parameter] public string ToggledTitle { get; set; }

        /// <summary>
        /// The color of the icon in the untoggled state. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The color of the icon in the toggled state. It supports the theme colors.
        /// </summary>
        [Parameter] public Color ToggledColor { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the component in the untoggled state.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The Size of the component in the toggled state.
        /// </summary>
        [Parameter] public Size ToggledSize { get; set; } = Size.Medium;

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
        /// The variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;

        public Task Toggle()
        {
            return SetToggledAsync(!Toggled);
        }

        protected async Task SetToggledAsync(bool toggled)
        {
            if (Disabled)
                return;
            if (Toggled != toggled)
            {
                Toggled = toggled;
                await ToggledChanged.InvokeAsync(Toggled);
            }
        }
    }
}
