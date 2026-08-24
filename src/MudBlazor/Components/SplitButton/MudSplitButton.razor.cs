using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        protected string Classname => new CssBuilder("mud-split-button")
            .AddClass(Class)
            .Build();

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
    }
}
