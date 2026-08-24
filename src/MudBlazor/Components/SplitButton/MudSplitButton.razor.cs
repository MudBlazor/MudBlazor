using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
    /// <summary>
    /// A button with a primary action and an adjacent toggle which opens a menu of related actions.
    /// </summary>
    /// <seealso cref="MudButton" />
    /// <seealso cref="MudButtonGroup" />
    /// <seealso cref="MudMenu" />
    public partial class MudSplitButton : MudComponentBase
    {
        private readonly ParameterState<bool> _openState;

        public MudSplitButton()
        {
            using var registerScope = CreateRegisterScope();
            _openState = registerScope.RegisterParameter<bool>(nameof(Open))
                .WithParameter(() => Open)
                .WithEventCallback(() => OpenChanged);
        }

        protected string Classname => new CssBuilder("mud-split-button")
            .AddClass(Class)
            .Build();

        /// <summary>
        /// Relays the menu's own open state back through <see cref="Open" />.
        /// </summary>
        /// <remarks>
        /// The menu owns closing itself (an item was chosen, the overlay was clicked), so its state
        /// has to travel back up rather than being pushed down only.
        /// </remarks>
        private Task OnMenuOpenChangedAsync(bool open) => _openState.SetValueAsync(open);

        /// <summary>
        /// The text of the primary action.
        /// </summary>
        /// <remarks>
        /// Ignored when <see cref="ButtonContent" /> is set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// The custom content of the primary action.
        /// </summary>
        /// <remarks>
        /// Takes precedence over <see cref="Label" /> when set.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public RenderFragment? ButtonContent { get; set; }

        /// <summary>
        /// The icon displayed before the primary action's text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public string? StartIcon { get; set; }

        /// <summary>
        /// The icon displayed after the primary action's text.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// Occurs when the primary action is clicked.
        /// </summary>
        /// <remarks>
        /// Clicking the toggle opens the menu instead, and never raises this event.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        /// <summary>
        /// The URL navigated to when the primary action is clicked.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public string? Href { get; set; }

        /// <summary>
        /// The browsing context for the primary action's <see cref="Href" />.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public string? Target { get; set; }

        /// <summary>
        /// The relationship between the current document and the primary action's <see cref="Href" />.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public string? Rel { get; set; }

        /// <summary>
        /// The type of the primary action button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="ButtonType.Button" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public ButtonType ButtonType { get; set; }

        /// <summary>
        /// The menu content shown when the toggle is clicked.
        /// </summary>
        /// <remarks>
        /// Typically a set of <see cref="MudMenuItem" /> components.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// The color of both segments.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default" />.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of both segments.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The display variant of both segments.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Prevents interaction with both segments.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Takes up 100% of the available width.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// Displays a shadow.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// Shows a ripple effect when a segment is clicked.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// The icon displayed on the menu toggle.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Icons.Material.Filled.ArrowDropDown" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public string ToggleIcon { get; set; } = Icons.Material.Filled.ArrowDropDown;

        /// <summary>
        /// Prevents interaction with the menu toggle only.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Use <see cref="Disabled" /> to disable both segments.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public bool ToggleDisabled { get; set; }

        /// <summary>
        /// Whether the menu is open and its items are visible.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  When this property changes, <see cref="OpenChanged" /> occurs.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.SplitButton.Behavior)]
        public bool Open { get; set; }

        /// <summary>
        /// Occurs when <see cref="Open" /> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>
        /// Uses compact vertical padding for the menu items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// The origin point on the split button where the menu opens from.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>, which lets <see cref="MudMenu" /> choose an origin.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public Origin? AnchorOrigin { get; set; }

        /// <summary>
        /// The direction the menu expands in from its anchor.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// The CSS classes applied to the menu's popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the menu's item list.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.SplitButton.Appearance)]
        public string? ListClass { get; set; }
    }
}
