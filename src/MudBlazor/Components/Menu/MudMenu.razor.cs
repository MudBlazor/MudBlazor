// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A list of choices displayed after clicking an element.
    /// </summary>
    /// <seealso cref="MudMenuItem" />
    public partial class MudMenu : MudComponentBase, IActivatable
    {
        private (double Top, double Left) _lastCursorPosition;
        private bool _isTemporary;
        private bool _isPointerOver;
        private bool _isClosing;
        private bool _isClosingPending;
        private event EventHandler? ChildClosing;
        private CancellationTokenSource _parentCts = new();
        private readonly Stopwatch _pointerEnterStopwatch = new();

        protected string Classname =>
            new CssBuilder("mud-menu")
                .AddClass("mud-menu-button-hidden", GetActivatorHidden())
                .AddClass(Class)
                .Build();

        protected string PopoverClassname =>
            new CssBuilder()
                .AddClass(PopoverClass)
                .AddClass("mud-popover-position-override", PositionAtCursor)
                .Build();

        protected string ActivatorClassname =>
            new CssBuilder("mud-menu-activator")
                .AddClass("mud-disabled", Disabled)
                .Build();

        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("top", _lastCursorPosition.Top.ToPx(), PositionAtCursor)
                .AddStyle("left", _lastCursorPosition.Left.ToPx(), PositionAtCursor)
                .Build();

        /// <summary>
        /// The text shown for this menu.
        /// </summary>
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
        /// The CSS classes applied to items in this menu.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public string? ListClass { get; set; }

        /// <summary>
        /// The CSS classes applied to the popover for this menu.
        /// </summary>
        /// <remarks>
        /// Multiple classes must be separated by spaces.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public string? PopoverClass { get; set; }

        /// <summary>
        /// The icon displayed for this menu.
        /// </summary>
        /// <remarks>
        /// When set, this menu will display a <see cref="MudIconButton" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? Icon { get; set; }

        /// <summary>
        /// The color of the icon when <see cref="Icon"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Inherit"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Color IconColor { get; set; } = Color.Inherit;

        /// <summary>
        /// The icon displayed before the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? StartIcon { get; set; }

        /// <summary>
        /// The icon displayed after the text.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public string? EndIcon { get; set; }

        /// <summary>
        /// The color of this menu's button when <see cref="Icon"/> is not set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Color.Default"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The size of this menu's button when <see cref="Icon" /> is not set.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Size Size { get; set; } = Size.Medium;

        /// <summary>
        /// The display variant to use.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Variant.Text"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public Variant Variant { get; set; } = Variant.Text;

        /// <summary>
        /// Applies compact vertical padding to all menu items.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public bool Dense { get; set; }

        /// <summary>
        /// Expands this menu to the same width as its parent.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public bool FullWidth { get; set; }

        /// <summary>
        /// Sets the maximum allowed height for this menu, when open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public int? MaxHeight { get; set; }

        /// <summary>
        /// Opens this menu at the cursor's location instead of the button's location.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.  Typically used for larger-sized activators.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public bool PositionAtCursor { get; set; }

        /// <summary>
        /// Overrides the default button with a custom component.
        /// </summary>
        /// <remarks>
        /// Can be a <see cref="MudButton"/>, <see cref="MudIconButton"/>, or any other component.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public RenderFragment? ActivatorContent { get; set; }

        /// <summary>
        /// The action which opens the menu, when <see cref="ActivatorContent"/> is set.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MouseEvent.LeftClick"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public MouseEvent ActivationEvent { get; set; } = MouseEvent.LeftClick;

        /// <summary>
        /// The point where the menu will open from.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.BottomLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public Origin AnchorOrigin { get; set; } = Origin.BottomLeft;

        /// <summary>
        /// Sets the direction the menu will open from the anchor.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="Origin.TopLeft"/>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public Origin TransformOrigin { get; set; } = Origin.TopLeft;

        /// <summary>
        /// Prevents the page from scrolling while this menu is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupAppearance)]
        public bool LockScroll { get; set; }

        /// <summary>
        /// Prevents the user from interacting with this menu.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Behavior)]
        public bool Disabled { get; set; }

        /// <summary>
        /// Shows a ripple animation when the user clicks the activator button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// Displays a drop shadow under the activator button.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.Appearance)]
        public bool DropShadow { get; set; } = true;

        /// <summary>
        /// The <see cref="MudMenuItem" /> components within this menu.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Occurs when <see cref="Open"/> has changed.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public EventCallback<bool> OpenChanged { get; set; }

        /// <summary>
        /// Whether this menu is open.
        /// </summary>
        /// <remarks>
        /// When this property changes, <see cref="OpenChanged"/> occurs.
        /// </remarks>
        public bool Open { get; private set; }

        [CascadingParameter]
        protected MudMenu? ParentMenu { get; set; }

        protected bool GetActivatorHidden() => ActivatorContent is null && string.IsNullOrWhiteSpace(Label) && string.IsNullOrWhiteSpace(Icon);

        /// <summary>
        /// Closes this menu.
        /// </summary>
        public async Task CloseMenuAsync()
        {
            Open = false;

            // Disable pointer move event
            // Avoid the issue where the menu can't close when the pointer moves, even though a menu item was clicked
            _isClosing = true;
            await PointerLeaveAsync();
            _isClosing = false;
            StateHasChanged();

            await OpenChanged.InvokeAsync(Open);
        }

        /// <summary>
        /// Opens this menu.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> or <see cref="PointerEventArgs"/> of the location of the click.
        /// When <see cref="PositionAtCursor"/> is <c>true</c>, the menu will be positioned at these coordinates.
        /// </param>
        /// <param name="temporary">
        /// Defaults to <c>false</c>.  When <c>true</c>, no overlay will be displayed.
        /// This is typically used for menus which only open while the cursor is over them.
        /// </param>
        public Task OpenMenuAsync(EventArgs args, bool temporary = false)
        {
            if (Disabled)
            {
                return Task.CompletedTask;
            }

            _isTemporary = temporary;

            if (args is MouseEventArgs mouseEventArgs)
            {
                _lastCursorPosition = (mouseEventArgs.PageY, mouseEventArgs.PageX);
            }

            // Don't open if already open, but let the stuff above get updated.
            if (Open)
            {
                return Task.CompletedTask;
            }

            Open = true;
            StateHasChanged();

            return OpenChanged.InvokeAsync(Open);
        }

        /// <summary>
        /// Shows or hides this menu.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> or <see cref="TouchEventArgs"/> with the location of the click.</param>
        public Task ToggleMenuAsync(EventArgs args)
        {
            if (Disabled)
            {
                return Task.CompletedTask;
            }

            if (args is MouseEventArgs mouseEventArgs)
            {
                var leftClick = ActivationEvent == MouseEvent.LeftClick && mouseEventArgs.Button == 0;
                var rightClick = ActivationEvent == MouseEvent.RightClick && (mouseEventArgs.Button is -1 or 2); // oncontextmenu is -1, right click is 2.

                // Only allow valid left or right conditions, except MouseOver activation which should always be allowed to toggle.
                if (!leftClick && !rightClick && ActivationEvent != MouseEvent.MouseOver)
                {
                    return Task.CompletedTask;
                }
            }

            return Open
                ? CloseMenuAsync()
                : OpenMenuAsync(args);
        }

        private async Task PointerEnterAsync(PointerEventArgs args)
        {
            // The Click event will interfere with the Enter event on devices that can't hover.
            if (args.PointerType is "touch" or "pen")
            {
                return;
            }

            _isPointerOver = true;
            _pointerEnterStopwatch.Restart();
            if (ParentMenu != null)
            {
                ParentMenu.ChildClosing?.Invoke(this, EventArgs.Empty);
                ParentMenu._isPointerOver = true;
                ParentMenu._pointerEnterStopwatch.Restart();
            }

            if (Open || ActivationEvent != MouseEvent.MouseOver)
            {
                return;
            }

            await OpenMenuAsync(args, true);
        }

        private void PointerMove(PointerEventArgs args)
        {
            if (!_isClosing)
            {
                _isPointerOver = true;
                if (ParentMenu != null)
                    ParentMenu._isPointerOver = true;
            }
        }

        /// <summary>
        /// Handles the pointer leave event.
        /// Closes the menu if the pointer leaves and does not re-enter within a specified delay.
        /// </summary>
        private async Task PointerLeaveAsync()
        {
            // There's no reason to handle the leave event if the pointer never entered the menu.
            if (!_isPointerOver)
            {
                return;
            }

            // Mark all parent menus as not hovered
            var menu = this;
            do
            {
                menu._isPointerOver = false;
                menu = menu.ParentMenu;
            } while (menu is not null);

            // If the menu is temporary and activated by mouse over, handle closing
            if (_isTemporary && ActivationEvent == MouseEvent.MouseOver)
            {
                if (_isClosingPending)
                    return;

                _isClosingPending = true;
                _parentCts = new CancellationTokenSource();

                if (ParentMenu != null)
                    ParentMenu.ChildClosing += OnParentCloseNotify;

                // Wait a bit to allow the cursor to move from the activator to the items popover.
                try
                {
                    await Task.Delay(MudGlobal.MenuDefaults.HoverDelay, _parentCts.Token);
                }
                catch (TaskCanceledException)
                {
                    // Ignore if close is requested by parent menu
                }

                if (ParentMenu != null)
                    ParentMenu.ChildClosing -= OnParentCloseNotify;

                await CloseMenuIfPointerNotReentered();

                _isClosingPending = false;
                _parentCts.Dispose();
            }
        }

        /// <summary>
        /// Close the menu if the pointer hasn't re-entered the menu after a delay.
        /// </summary>
        private async Task CloseMenuIfPointerNotReentered()
        {
            var menu = this;
            while (menu is { ActivationEvent: MouseEvent.MouseOver, _isPointerOver: false, _isTemporary: true })
            {
                // If the parent menu is open again, then wait a bit to allow move event
                // But do not wait if the parent menu is open by another child menu
                var waitTime = MudGlobal.MenuDefaults.HoverDelay + MudGlobal.MenuDefaults.PreventCloseWaitingTime;
                var isWaitingNeeded = menu._pointerEnterStopwatch.ElapsedMilliseconds <= waitTime && (menu != this || !_parentCts.Token.IsCancellationRequested);
                if (isWaitingNeeded)
                {
                    await Task.Delay(MudGlobal.MenuDefaults.PreventCloseWaitingTime, CancellationToken.None);
                    continue;
                }

                await menu.CloseMenuAsync();
                menu = menu.ParentMenu;
            }
        }

        /// <summary>
        /// Parent menu sends a close request to this menu.
        /// At this time, this menu should close even if the leave event is waiting.
        /// </summary>
        private void OnParentCloseNotify(object? sender, EventArgs args)
        {
            _parentCts.Cancel();
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
