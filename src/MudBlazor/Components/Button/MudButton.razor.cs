using Microsoft.AspNetCore.Components;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// Represents a button for actions, links, and commands.
    /// </summary>
    /// <remarks>
    /// Creates a <see href="https://developer.mozilla.org/docs/Web/HTML/Element/Button">button</see> element,
    /// or <see href="https://developer.mozilla.org/docs/Web/HTML/Element/a">anchor</see> if <c>Href</c> is set.<br/>
    /// You can directly add attributes like <c>title</c> or <c>aria-label</c>.
    /// </remarks>
    /// <seealso cref="MudButtonGroup" />
    /// <seealso cref="MudFab" />
    /// <seealso cref="MudIconButton" />
    /// <seealso cref="MudToggleIconButton" />
    public partial class MudButton : MudBaseButton, IDisposable
    {
        protected string Classname => new CssBuilder("mud-button-root mud-button")
            .AddClass($"mud-button-{Variant.ToDescriptionString()}")
            .AddClass($"mud-button-{Variant.ToDescriptionString()}-{Color.ToDescriptionString()}")
            .AddClass($"mud-button-{Variant.ToDescriptionString()}-size-{Size.ToDescriptionString()}")
            .AddClass($"mud-width-full", GetRealFullWith())
            .AddClass($"mud-ripple", Ripple)
            .AddClass($"mud-button-disable-elevation", !DropShadow)
            .AddClass(Class)
            .Build();

        protected string StartIconClass => new CssBuilder("mud-button-icon-start")
            .AddClass($"mud-button-icon-size-{(IconSize ?? Size).ToDescriptionString()}")
            .AddClass(IconClass)
            .Build();

        protected string EndIconClass => new CssBuilder("mud-button-icon-end")
            .AddClass($"mud-button-icon-size-{(IconSize ?? Size).ToDescriptionString()}")
            .AddClass(IconClass)
            .Build();

        /// <summary>
        /// The buton group which owns this button.
        /// </summary>
        [CascadingParameter]
        private MudButtonGroup? ButtonGroup { get; set; }

        /// <summary>
        /// The icon displayed before the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use <see cref="EndIcon"/> to display an icon after the text.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? StartIcon { get; set; }

        /// <summary>
        /// The icon displayed after the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  Use <see cref="StartIcon"/> to display an icon before the text.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// The color of icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The size of icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  When <c>null</c>, the value of <see cref="Size"/> is used.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size? IconSize { get; set; }

        /// <summary>
        /// The CSS classes applied to icons.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.  You can use spaces to separate multiple classes.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public string? IconClass { get; set; }

        /// <summary>
        /// The color of the button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.  Theme colors are supported.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Color Color { get; set; } = MudGlobal.ButtonDefaults.Color;

        /// <summary>
        /// The size of the button.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Size.Medium"/>.   Use the <see cref="IconSize"/> property to set the size of icons.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Size Size { get; set; } = MudGlobal.ButtonDefaults.Size;

        /// <summary>
        /// The display variation to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public Variant Variant { get; set; } = MudGlobal.ButtonDefaults.Variant;

        /// <summary>
        /// Expands the button to 100% of the container width.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Button.Appearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// The content within this component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Button.Behavior)]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ButtonGroup?.AddButton(this);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases resources used by this button.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ButtonGroup?.RemoveButton(this);
            }
        }

        internal bool GetRealFullWith()
        {
            if (FullWidth)
            {
                return true;
            }
            // If the button is in a group, the group is stretched and none button is explicitly stretched,
            // then the button need to be streched
            // See https://github.com/MudBlazor/MudBlazor/issues/9710
            return ButtonGroup != null && ButtonGroup.FullWidth && ButtonGroup.NoneButtonIsStreched();
        }
    }
}
