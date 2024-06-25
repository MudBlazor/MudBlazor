using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a button consisting of an icon that can be toggled between two distinct states.
    /// </summary>
    /// <remarks>
    /// Creates a <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/Button">button</see> element,
    /// or <see href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/a">anchor</see> if <c>Href</c> is set.<br/>
    /// You can directly add attributes like <c>title</c> or <c>aria-label</c>.
    /// </remarks>
    public partial class MudToggleIconButton : MudComponentBase
    {
        /// <summary>
        /// The toggled value.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool Toggled { get; set; }

        /// <summary>
        /// Fires whenever toggled is changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> ToggledChanged { get; set; }

        /// <summary>
        /// The Icon that will be used in the untoggled state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The Icon that will be used in the toggled state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? ToggledIcon { get; set; }

        /// <summary>
        /// The color of the icon in the untoggled state. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The color of the icon in the toggled state. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color ToggledColor { get; set; } = Color.Default;

        /// <summary>
        /// The Size of the component in the untoggled state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The Size of the component in the toggled state.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size ToggledSize { get; set; } = Size.Medium;

        /// <summary>
        /// If set uses a negative margin.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Edge Edge { get; set; }

        /// <summary>
        /// Whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, the button will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// The variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Determines whether the component has a drop-shadow. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// If true, the click event bubbles up to the containing/parent component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public bool ClickPropagation { get; set; }

        public Task Toggle()
        {
            return SetToggledAsync(!Toggled);
        }

        protected internal async Task SetToggledAsync(bool toggled)
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
