using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudMenu : MudComponentBase, IActivatable
    {
        protected string Classname =>
        new CssBuilder("mud-menu")
        .AddClass(Class)
       .Build();

        protected string ActivatorClassname =>
        new CssBuilder("mud-menu-activator")
        .AddClass("mud-disabled", Disabled)
       .Build();

        private bool _isOpen;
        private bool _isMouseOver = false;

        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string Label { get; set; }

        /// <summary>
        /// User class names for the list, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public string ListClass { get; set; }

        /// <summary>
        /// User class names for the popover, separated by space
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public string PopoverClass { get; set; }

        /// <summary>
        /// Icon to use if set will turn the button into a MudIconButton.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string Icon { get; set; }

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
        public string StartIcon { get; set; }

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string EndIcon { get; set; }

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
        /// Sets the maxheight the menu can have when open.
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
        /// If true, instead of positioning the menu at the left upper corner, position at the exact cursor location.
        /// This makes sense for larger activators
        /// </summary>
        [Obsolete("Use PositionAtCursor instead.", true)]
        [Parameter]
        public bool PositionAtCurser
        {
            get => PositionAtCursor;
            set => PositionAtCursor = value;
        }

        /// <summary>
        /// Place a MudButton, a MudIconButton or any other component capable of acting as an activator. This will
        /// override the standard button and all parameters which concern it.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public RenderFragment ActivatorContent { get; set; }

        /// <summary>
        /// Specify the activation event when ActivatorContent is set
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public MouseEvent ActivationEvent { get; set; } = MouseEvent.LeftClick;

        /// <summary>
        /// Set the anchor origin point to determen where the popover will open from.
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
        /// Sets the direction the select menu will start from relative to its parent.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use AnchorOrigin or TransformOrigin instead.", true)]
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the select menu will open either before or after the input depending on the direction.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use AnchorOrigin or TransformOrigin instead.", true)]
        [Parameter] public bool OffsetY { get; set; }

        /// <summary>
        /// If true, the select menu will open either above or bellow the input depending on the direction.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [Obsolete("Use AnchorOrigin or TransformOrigin instead.", true)]
        [Parameter] public bool OffsetX { get; set; }

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
        /// If true, disables ripple effect.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public bool DisableRipple { get; set; }

        /// <summary>
        /// If true, no drop-shadow will be used.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public bool DisableElevation { get; set; }

        #region Obsolete members from previous MudButtonBase inherited structure

        [ExcludeFromCodeCoverage]
        [Obsolete("Linking is not supported. MudMenu is not a MudBaseButton anymore.", true)]
        [Parameter] public string Link { get; set; }

        [ExcludeFromCodeCoverage]
        [Obsolete("Linking is not supported. MudMenu is not a MudBaseButton anymore.", true)]
        [Parameter] public string Target { get; set; }

        [ExcludeFromCodeCoverage]
        [Obsolete("MudMenu is not a MudBaseButton anymore.", true)]
        [Parameter] public string HtmlTag { get; set; } = "button";

        [ExcludeFromCodeCoverage]
        [Obsolete("MudMenu is not a MudBaseButton anymore.", true)]
        [Parameter] public ButtonType ButtonType { get; set; }

        [ExcludeFromCodeCoverage]
        [Obsolete("MudMenu is not a MudBaseButton anymore.", true)]
        [Parameter] public ICommand Command { get; set; }

        [ExcludeFromCodeCoverage]
        [Obsolete("MudMenu is not a MudBaseButton anymore.", true)]
        [Parameter] public object CommandParameter { get; set; }

        #endregion

        /// <summary>
        /// Add menu items here
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public RenderFragment ChildContent { get; set; }
        
        /// <summary>
        /// Fired when the menu IsOpen property changes.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public EventCallback<bool> IsOpenChanged { get; set; }

        public string PopoverStyle { get; set; }

        /// <summary>
        /// Gets a value indicating whether the menu is currently open or not.
        /// </summary>
        public bool IsOpen
        {
            get { return _isOpen; }
        }

        /// <summary>
        /// Closes the menu.
        /// </summary>
        public void CloseMenu()
        {
            _isOpen = false;
            _isMouseOver = false;
            PopoverStyle = null;
            StateHasChanged();
            IsOpenChanged.InvokeAsync(_isOpen);
        }

        /// <summary>
        /// Opens the menu.
        /// </summary>
        /// <param name="args">The arguments of the calling mouse event. If
        /// <see cref="PositionAtCursor"/> is true, the menu will be positioned using the
        /// coordinates in this parameter.</param>
        public void OpenMenu(EventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            if (PositionAtCursor)
            {
                if (args is MouseEventArgs mouseEventArgs)
                {
                    SetPopoverStyle(mouseEventArgs);
                }
            }

            _isOpen = true;
            StateHasChanged();
            IsOpenChanged.InvokeAsync(_isOpen);
        }

        // Sets the popover style ONLY when there is an activator
        private void SetPopoverStyle(MouseEventArgs args)
        {
            AnchorOrigin = Origin.TopLeft;
            PopoverStyle = $"margin-top: {args?.OffsetY.ToPx()}; margin-left: {args?.OffsetX.ToPx()};";
        }

        public void ToggleMenu(MouseEventArgs args)
        {
            if (Disabled)
                return;
            if (ActivationEvent == MouseEvent.LeftClick && args.Button != 0 && !_isOpen)
                return;
            if (ActivationEvent == MouseEvent.RightClick && args.Button != 2 && !_isOpen)
                return;
            if (_isOpen)
                CloseMenu();
            else
                OpenMenu(args);
        }

        public void ToggleMenuTouch(TouchEventArgs args)
        {
            if (Disabled)
            {
                return;
            }

            if (_isOpen)
            {
                CloseMenu();
            }
            else
            {
                OpenMenu(args);
            }
        }

        /// <summary>
        /// Implementation of IActivatable.Activate, toggles the menu.
        /// </summary>
        /// <param name="activator"></param>
        /// <param name="args"></param>
        public void Activate(object activator, MouseEventArgs args)
        {
            ToggleMenu(args);
        }

        public void MouseEnter(MouseEventArgs args)
        {
            _isMouseOver = true;

            if (ActivationEvent == MouseEvent.MouseOver)
            {
                OpenMenu(args);
            }
        }

        public async void MouseLeave()
        {
            _isMouseOver = false;

            await Task.Delay(100);

            if (ActivationEvent == MouseEvent.MouseOver && _isMouseOver == false)
            {
                CloseMenu();
            }
        }
    }
}
