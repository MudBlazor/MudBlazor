using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Extensions;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
    public partial class MudMenu : MudBaseButton, IActivatable
    {
        protected string Classname =>
        new CssBuilder("mud-menu")
        .AddClass(Class)
       .Build();

        protected string MenuClassname =>
        new CssBuilder("mud-menu-container")
        .AddClass("mud-menu-fullwidth", FullWidth)
       .Build();

        private bool _isOpen;

        [Parameter] public string Label { get; set; }

        /// <summary>
        /// Icon to use if set will turn the button into a MudIconButton.
        /// </summary>
        [Parameter] public string Icon { get; set; }

        /// <summary>
        /// The color of the icon. It supports the theme colors.
        /// </summary>
        [Parameter] public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// Icon placed before the text if set.
        /// </summary>
        [Parameter] public string StartIcon { get; set; }

        /// <summary>
        /// Icon placed after the text if set.
        /// </summary>
        [Parameter] public string EndIcon { get; set; }

        /// <summary>
        /// The color of the button. It supports the theme colors.
        /// </summary>
        [Parameter] public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The button Size of the component.
        /// </summary>
        [Parameter] public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The button variant to use.
        /// </summary>
        [Parameter] public Variant Variant { get; set; } = Variant.Text;


        /// <summary>
        /// If true, compact vertical padding will be applied to all menu items.
        /// </summary>
        [Parameter] public bool Dense { get; set; }

        /// <summary>
        /// If true, the list menu will be same width as the parent.
        /// </summary>
        [Parameter] public bool FullWidth { get; set; }

        /// <summary>
        /// Sets the maxheight the menu can have when open.
        /// </summary>
        [Parameter] public int? MaxHeight { get; set; }

        /// <summary>
        /// If true, instead of positioning the menu at the left upper corner, position at the exact cursor location.
        /// This makes sense for larger activators
        /// </summary>
        [Parameter] public bool PositionAtCursor { get; set; }

        /// <summary>
        /// If true, instead of positioning the menu at the left upper corner, position at the exact cursor location.
        /// This makes sense for larger activators
        /// </summary>
        [Obsolete("Obsolete.  Replace with `PositionAtCursor`.")]
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
        [Parameter] public RenderFragment ActivatorContent { get; set; }

        /// <summary>
        /// Specify the activation event when ActivatorContent is set
        /// </summary>
        [Parameter] public MouseEvent ActivationEvent { get; set; } = MouseEvent.LeftClick;

        /// <summary>
        /// Sets the direction the select menu will start from relative to its parent.
        /// </summary>
        [Parameter] public Direction Direction { get; set; } = Direction.Bottom;

        /// <summary>
        /// If true, the select menu will open either before or after the input depending on the direction.
        /// </summary>
        [Parameter] public bool OffsetY { get; set; }

        /// <summary>
        /// If true, the select menu will open either above or bellow the input depending on the direction.
        /// </summary>
        [Parameter] public bool OffsetX { get; set; }

        /// <summary>
        /// Set to true if you want to prevent page from scrolling when the menu is open
        /// </summary>
        [Parameter] public bool LockScroll { get; set; }

        /// <summary>
        /// Add menu items here
        /// </summary>
        [Parameter] public RenderFragment ChildContent { get; set; }

        public string PopoverStyle { get; set; }

        public void CloseMenu()
        {
            _isOpen = false;
            PopoverStyle = null;
            StateHasChanged();
        }

        public void OpenMenu(EventArgs args)
        {
            if (Disabled)
                return;
            if (PositionAtCursor) SetPopoverStyle((MouseEventArgs)args);
            _isOpen = true;
            StateHasChanged();
        }

        // Sets the popover style ONLY when there is an activator
        private void SetPopoverStyle(MouseEventArgs args)
        {
            //use the offset with a relative position to the container
            PopoverStyle = $"left:{args?.OffsetX.ToPixels()};top:{args?.OffsetY.ToPixels()};";
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

        /// <summary>
        /// Implementation of IActivatable.Activate, toggles the menu.
        /// </summary>
        /// <param name="activator"></param>
        /// <param name="args"></param>
        public void Activate(object activator, MouseEventArgs args)
        {
            ToggleMenu(args);
        }

    }
}
