
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace MudBlazor
{
    public partial class MudToggleIconButton
    {
        private bool _toggled;

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

        protected override void OnInitialized()
        {
            _toggled = Toggled;
        }

        public async Task Toggle()
        {
            _toggled = !_toggled;
            await ToggledChanged.InvokeAsync(_toggled);
        }

    }
}
