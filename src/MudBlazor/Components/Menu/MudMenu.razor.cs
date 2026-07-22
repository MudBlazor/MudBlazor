using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    public partial class MudMenu : MudComponentBase, IActivatable
    {
        private bool _open;
        private string? _popoverStyle;
        private bool _isPointerOver;

        protected string Classname =>
            new CssBuilder("mud-menu")
                .AddClass(Class)
                .Build();

        protected string ActivatorClassname =>
            new CssBuilder("mud-menu-activator")
                .AddClass("mud-disabled", Disabled)
                .Build();

        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? Label { get; set; }

        /// <summary>
        /// The <c>aria-label</c> for the menu button when <see cref="ActivatorContent"/> is not set.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? AriaLabel { get; set; }

        /// <summary>
        /// User class names for the list, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public string? ListClass { get; set; }

        /// <summary>
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// Icon to use if set will turn the button into a MudIconButton.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// Icon placed before the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? StartIcon { get; set; }

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// The color of the button. It supports the theme colors.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The button Size of the component.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The button variant to use.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// If true, compact vertical padding will be applied to all menu items.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// If true, the list menu will be same width as the parent.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// Sets the max height the menu can have when open.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// If true, instead of positioning the menu at the left upper corner, position at the exact cursor location.
        /// This makes sense for larger activators
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public bool PositionAtCursor { get; set; }

        /// <summary>
        /// Place a MudButton, a MudIconButton or any other component capable of acting as an activator. This will
        /// override the standard button and all parameters which concern it.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public RenderFragment? ActivatorContent { get; set; }

        /// <summary>
        /// Specify the activation event when ActivatorContent is set
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public MouseEvent ActivationEvent { get; set; } = MouseEvent.LeftClick;

        /// <summary>
        /// Set the anchor origin point to determine where the popover will open from.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public Origin AnchorOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Sets the transform origin point for the popover.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Set to true if you want to prevent page from scrolling when the menu is open
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public bool LockScroll { get; set; }

        /// <summary>
        /// If true, menu will be disabled.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets whether to show a ripple effect when the user clicks the button. Default is true.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Determines whether the component has a drop-shadow. Default is true
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// Add menu items here
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Fired when the menu <see cref="Open"/> property changes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>
        /// Gets a value indicating whether the menu is currently open or not.
        /// </summary>
        public bool Open
        {
            get { return _open; }
        }

        /// <summary>
        /// Closes the menu.
        /// </summary>
        public Task CloseMenuAsync()
        {
            _open = false;
            _isPointerOver = false;
            _popoverStyle = null;
            StateHasChanged();

            return OpenChanged.InvokeAsync(_open);
        }

        /// <summary>
        /// Opens the menu.
        /// </summary>
        /// <param name="args">
        /// The arguments of the calling mouse/pointer event.
        /// If <see cref="PositionAtCursor"/> is true, the menu will be positioned using the coordinates in this parameter.
        /// </param>
        public Task OpenMenuAsync(EventArgs args)
        {
            if (Disabled)
            {
                return Task.CompletedTask;
            }

            if (PositionAtCursor)
            {
                if (args is MouseEventArgs mouseEventArgs)
                {
                    SetPopoverStyle(mouseEventArgs);
                }
            }

            _open = true;
            StateHasChanged();

            return OpenChanged.InvokeAsync(_open);
        }

        /// <summary>
        /// Sets the popover style ONLY when there is an activator.
        /// </summary>
        private void SetPopoverStyle(MouseEventArgs args)
        {
            AnchorOrigin = Origin.TopLeft;
            _popoverStyle = $"margin-top: {args?.OffsetY.ToPx()}; margin-left: {args?.OffsetX.ToPx()};";
        }

        /// <summary>
        /// Toggle the visibility of the menu.
        /// </summary>
        public async Task ToggleMenuAsync(EventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            // oncontextmenu turns a touch event into MouseEventArgs but with a button of -1.
            if (args is MouseEventArgs mouseEventArgs && mouseEventArgs.Button != -1)
            {
                if (ActivationEvent == MouseEvent.LeftClick && mouseEventArgs.Button != 0 && !_open)
                {
                    return;
                }

                if (ActivationEvent == MouseEvent.RightClick && mouseEventArgs.Button != 2 && !_open)
                {
                    return;
                }
            }

            if (_open)
            {
                await CloseMenuAsync();
            }
            else
            {
                await OpenMenuAsync(args);
            }
        }

        private async Task PointerEnterAsync(PointerEventArgs args)
        {
            _isPointerOver = true;

            if (ActivationEvent == MouseEvent.MouseOver)
            {
                await OpenMenuAsync(args);
            }
        }

        private async Task PointerLeaveAsync()
        {
            _isPointerOver = false;

            await Task.Delay(100);

            if (ActivationEvent == MouseEvent.MouseOver && !_isPointerOver)
            {
                await CloseMenuAsync();
            }
        }

        /// <summary>
        /// Implementation of IActivatable.Activate, toggles the menu.
        /// </summary>
        void IActivatable.Activate(object activator, MouseEventArgs args)
        {
            _ = ToggleMenuAsync(args);
        }
    }
}
