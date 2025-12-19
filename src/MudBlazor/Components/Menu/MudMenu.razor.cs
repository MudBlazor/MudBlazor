// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Interfaces;
using MudBlazor.Services;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// An interactive menu that displays a list of options.
    /// </summary>
    /// <seealso cref="MudMenuItem" />
    public partial class MudMenu : MudComponentBase, IActivatable, IDisposable
    {
        private readonly ParameterState<bool> _openState;
        private readonly List<MudMenu> _subMenus = [];
        private (double Top, double Left) _openPosition;
        private bool _isPointerOver;
        private bool _isTransient;
        private bool _lastInteractionWasKeyboard;
        private CancellationTokenSource? _hoverCts;
        private CancellationTokenSource? _leaveCts;
        private int _focusedIndex = -1;
        private MudButton? _buttonActivator;
        private MudMenuItem? _menuItemActivator;
        private MudIconButton? _iconButtonActivator;
        private ElementReference _menuWrapperRef;
        private readonly List<object> _menuItems = [];
        private readonly string _elementId = Identifier.Create("menu");
        private DateTime _lastKeyboardActivation = DateTime.MinValue;

        [Inject]
        private IKeyInterceptorService KeyInterceptorService { get; set; } = null!;

        [Inject]
        private IPopoverService PopoverService { get; set; } = null!;

        public MudMenu()
        {
            using var registerScope = CreateRegisterScope();
            _openState = registerScope.RegisterParameter<bool>(nameof(Open))
                .WithParameter(() => Open)
                .WithEventCallback(() => OpenChanged)
                .WithChangeHandler(OnOpenChanged);
        }

        /// <summary>
        /// The CSS class for the root menu container.
        /// </summary>
        protected string Classname =>
            new CssBuilder("mud-menu")
                .AddClass("mud-menu-button-hidden", GetActivatorHidden())
                .AddClass(Class)
                .Build();

        /// <summary>
        /// The CSS class for the menu's popover container.
        /// </summary>
        protected string PopoverClassname =>
            new CssBuilder()
                .AddClass(PopoverClass)
                .AddClass("mud-popover-nested", ParentMenu is not null)
                .AddClass("mud-popover-position-override", PositionAtCursor)
                .Build();

        /// <summary>
        /// The CSS class for the list containing menu items.
        /// </summary>
        protected string ListClassname =>
            new CssBuilder("mud-menu-list")
                .AddClass(ListClass)
                .Build();

        /// <summary>
        /// The CSS class for the activator element (button or custom content).
        /// </summary>
        protected string ActivatorClassname =>
            new CssBuilder("mud-menu-activator")
                .AddClass("mud-disabled", Disabled)
                .Build();

        /// <summary>
        /// Inline data attributes for positioning the menu at the cursor's location.
        /// </summary>
        private Dictionary<string, object> PositionAttributes => new()
        {
            { "data-pc-x", _openPosition.Left.ToString(CultureInfo.InvariantCulture) },
            { "data-pc-y", _openPosition.Top.ToString(CultureInfo.InvariantCulture) },
        };

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
        /// The origin point for the menu's anchor. If set, overrides Nested Menus, and PositionatCursor Anchor points.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>null</c>.
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public Origin? AnchorOrigin { get; set; }

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
        /// Displays the dropdown popover in a fixed position, even while scrolling.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>false</c>.
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public bool PopoverFixed { get; set; }



        /// <summary>
        /// Determines the width of the Popover dropdown in relation the parent container.
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <see cref="DropdownWidth.Ignore" />. </para>
        /// <para>When <see cref="DropdownWidth.Relative" />, restricts the max-width of the component to the width of the parent container</para>
        /// <para>When <see cref="DropdownWidth.Adaptive" />, restricts the min-width of the component to the width of the parent container</para>
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Popover.Appearance)]
        public DropdownWidth RelativeWidth { get; set; } = DropdownWidth.Ignore;

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
        /// Prevents interaction with background elements while this menu is open.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="PopoverOptions.ModalOverlay" />.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public bool? Modal { get; set; }

        /// <summary>
        /// Gets the resolved modal overlay value, using the global default from <see cref="PopoverOptions"/> if not explicitly set.
        /// </summary>
        /// <remarks>
        /// TODO: Once .NET 8 support is dropped, consider using constructor injection (available in .NET 9+) to set defaults directly.
        /// </remarks>
        protected bool GetModal() => Modal ?? PopoverService.PopoverOptions.ModalOverlay;

        /// <summary>
        /// Gets the transition duration for the popover, using dense menus to disable animations.
        /// </summary>
        protected double GetTransitionDuration() => GetDense() ? 0 : PopoverService.PopoverOptions.Duration.TotalMilliseconds;

        /// <summary>
        /// The <see cref="MudMenuItem" /> components within this menu.
        /// </summary>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Whether this menu is open and the menu items are visible.
        /// </summary>
        /// <remarks>
        /// When this property changes, <see cref="OpenChanged"/> occurs.
        /// </remarks>
        [Parameter, ParameterState]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public bool Open { get; set; }

        /// <summary>
        /// Occurs when <see cref="Open"/> has changed.
        /// </summary>
        [Parameter]
        public EventCallback<bool> OpenChanged { get; set; }

        [CascadingParameter]
        protected MudMenu? ParentMenu { get; set; }

        protected bool GetActivatorHidden() => ActivatorContent is null && string.IsNullOrWhiteSpace(Label) && string.IsNullOrWhiteSpace(Icon);

        /// <summary>
        /// Walk recursively up the menu hierarchy to determine if any parent menu is dense.
        /// </summary>
        internal bool GetDense() => Dense || ParentMenu?.GetDense() == true;

        /// <summary>
        /// Determines the positioning origin for the menu popover.
        /// </summary>
        /// <remarks>
        /// This establishes where the menu will appear relative to its activator or the cursor.
        /// </remarks>
        protected Origin GetAnchorOrigin()
        {
            if (AnchorOrigin is not null)
            {
                // Use the defined anchor origin if set.
                return AnchorOrigin.Value;
            }

            if (ParentMenu is not null)
            {
                // Sub-menus typically open to the right of their parent.
                return Origin.TopRight;
            }
            else if (PositionAtCursor)
            {
                return Origin.TopLeft;
            }

            // Default behavior for a top-level menu is to open below its activator.
            return Origin.BottomLeft;
        }

        /// <summary>
        /// Registers a child menu with this menu, allowing for hierarchical menu management.
        /// This is crucial for controlling the open/close state of nested menus.
        /// </summary>
        /// <param name="child">The child <see cref="MudMenu"/> to register.</param>
        protected void RegisterChild(MudMenu child)
        {
            _subMenus.Add(child);
        }

        /// <summary>
        /// Unregisters a child menu from this menu.
        /// This is called when a child menu is disposed or removed, maintaining accurate tracking of nested menus.
        /// </summary>
        /// <param name="child">The child <see cref="MudMenu"/> to unregister.</param>
        protected void UnregisterChild(MudMenu child)
        {
            _subMenus.Remove(child);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            // If this menu is a sub-menu, register it with its parent.
            ParentMenu?.RegisterChild(this);

            if (ParentMenu != null)
            {
                ParentMenu.RegisterItem(this); // Pass the MudMenu directly
            }
        }

        protected Task OnOpenChanged(ParameterChangedEventArgs<bool> args)
        {
            return args.Value ?
                OpenMenuAsync(EventArgs.Empty) :
                CloseMenuAsync();
        }

        /// <summary>
        /// Closes this menu and any descendants if it's a nested menu.
        /// </summary>
        /// <remarks>
        /// It ensures that all nested menus are also closed when a parent menu is closed.
        /// </remarks>
        public async Task CloseMenuAsync()
        {
            // Discard any pending pointer actions so the menu doesn't re-open or try to close twice.
            CancelPendingActions();

            // Recursively close all child menus.
            foreach (var child in _subMenus.Where(m => m._openState.Value))
            {
                await child.CloseMenuAsync();
            }

            // Now close this menu itself.
            _focusedIndex = -1;
            _lastInteractionWasKeyboard = false;
            _menuItems.Clear();
            await Task.Yield();

            if (_openState.Value)
            {
                try
                {
                    await KeyInterceptorService.UnsubscribeAsync(_elementId);
                }
                catch (JSException)
                {
                    // Element already gone, safe to ignore.
                }
            }

            await _openState.SetValueAsync(false);
            await InvokeAsync(StateHasChanged);
        }

        /// <summary>
        /// Closes all menus in the hierarchy, starting from the top-most parent.
        /// </summary>
        /// <remarks>
        /// This is useful for dismissing all open menus with a single action, such as clicking outside the menu area.
        /// </remarks>
        public async Task CloseAllMenusAsync()
        {
            // Traverse up the menu hierarchy to find the top-most parent.
            var top = this;
            while (true)
            {
                if (top.ParentMenu is null)
                {
                    break;
                }

                top = top.ParentMenu;
            }

            // Close the top-most menu, which will cascade down to close all its children.
            await top.CloseMenuAsync();

            // Return focus to the menu
            await top.FocusActivatorAsync();
        }

        /// <summary>
        /// Opens the menu or updates its state if it's already open.
        /// </summary>
        /// <param name="args">
        /// <para>The event arguments for the activation event; <see cref="MouseEventArgs"/> or <see cref="TouchEventArgs"/>.</para>
        /// <para>When <see cref="PositionAtCursor"/> is <c>true</c>, the menu will be positioned at these coordinates.</para>
        /// </param>
        /// <param name="transient">If <c>true</c>, the menu will close automatically when the pointer leaves its bounds.</param>
        /// <remarks>
        /// Parents are not automatically opened when a child is opened.
        /// </remarks>
        public async Task OpenMenuAsync(EventArgs args, bool transient = false)
        {
            if (Disabled)
            {
                return;
            }

            _isTransient = transient;

            // Set the menu position to the cursor if the event has coordinates.
            if (args is MouseEventArgs mouseEventArgs)
            {
                _openPosition = (mouseEventArgs.PageY, mouseEventArgs.PageX);
            }

            // Officially open the menu.
            await _openState.SetValueAsync(true);
            await InvokeAsync(StateHasChanged);

            // Wait for rendering to finish so the element with the ID is in the DOM
            await Task.Yield();

            await SubscribeToMenuKeyInterceptorAsync();
        }

        /// <summary>
        /// Closes sibling menus before opening as a "mouse over" menu.
        /// It prevents multiple sub-menus at the same level from being open simultaneously when hovering.
        /// </summary>
        /// <remarks>
        /// This is called in place of <see cref="OpenMenuAsync"/> if the menu activator is implicitly rendered for the submenu.
        /// </remarks>
        protected async Task OpenSubMenuAsync(EventArgs args)
        {
            // Close siblings (and self) first.
            if (ParentMenu is not null)
            {
                foreach (var sibling in ParentMenu._subMenus.Where(m => m._openState.Value))
                {
                    await sibling.CloseMenuAsync();
                }
            }

            // Open transiently so it will close when the pointer leaves its bounds.
            await OpenMenuAsync(args, true);
        }

        /// <summary>
        /// Toggles the menu's open or closed state.
        /// </summary>
        /// <param name="args">
        /// <para>The event arguments for the activation event; <see cref="MouseEventArgs"/> or <see cref="TouchEventArgs"/>.</para>
        /// <para>When <see cref="PositionAtCursor"/> is <c>true</c>, the menu will be positioned at these coordinates.</para>
        /// </param>
        public Task ToggleMenuAsync(EventArgs args)
        {
            if (Disabled)
            {
                return Task.CompletedTask;
            }

            if (args is MouseEventArgs mouseEventArgs)
            {
                // Determine if the click matches the expected activation event.
                // This indicates it's a synthetic click following Enter/Space
                var timeSinceKeyboard = DateTime.UtcNow - _lastKeyboardActivation;
                if (timeSinceKeyboard.TotalMilliseconds < 50)
                {
                    return Task.CompletedTask;
                }

                var leftClick = ActivationEvent == MouseEvent.LeftClick && mouseEventArgs.Button == 0;
                var rightClick = ActivationEvent == MouseEvent.RightClick && (mouseEventArgs.Button is -1 or 2); // oncontextmenu = -1, right click = 2.

                // Ignore invalid click types if we're using a click-based activation event.
                if (!leftClick && !rightClick && ActivationEvent != MouseEvent.MouseOver)
                {
                    return Task.CompletedTask;
                }
            }

            // Toggle the menu's state; close if open, open if closed.
            return _openState.Value
                ? CloseMenuAsync()
                : OpenMenuAsync(args);
        }

        /// <summary>
        /// Determines if the menu should respond to hover events.
        /// </summary>
        /// <remarks>
        /// This prevents hover-related actions on devices that don't support traditional hovering (e.g., touchscreens).
        /// </remarks>
        private bool IsHoverable(PointerEventArgs args)
        {
            // If hover isn't explicitly enabled (or implicitly by being a submenu) there's no work to be done.
            if (ActivationEvent != MouseEvent.MouseOver && ParentMenu is null)
            {
                return false;
            }

            // The click event will conflict with this one on devices that can't hover so we'll return so we only handle one.
            if (args.PointerType is "touch" or "pen")
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the pointer entering either the activator or the menu list.
        /// </summary>
        /// <remarks>
        /// This initiates a hover delay before opening the menu to provide a more forgiving user experience.
        /// </remarks>
        private async Task PointerEnterAsync(PointerEventArgs args)
        {
            _isPointerOver = true;
            _lastInteractionWasKeyboard = false;

            // Prevent conflicting actions.
            CancelPendingActions();

            if (!IsHoverable(args))
            {
                return;
            }

            if (MudGlobal.MenuDefaults.HoverDelay > 0)
            {
                _hoverCts = new();

                try
                {
                    await Task.Delay(MudGlobal.MenuDefaults.HoverDelay, _hoverCts.Token);
                }
                catch (TaskCanceledException)
                {
                    // Hover action was canceled, meaning another action (like moving the pointer away) occurred.
                    return;
                }
            }

            if (!_openState.Value)
            {
                await OpenSubMenuAsync(args);
            }
        }

        /// <summary>
        /// Handles the pointer leaving either the activator or the menu list.
        /// </summary>
        /// <remarks>
        /// This introduces a delay before closing the menu to allow smooth transitions between nested menus.
        /// </remarks>
        private async Task PointerLeaveAsync(PointerEventArgs args)
        {
            _isPointerOver = false;

            // Prevent conflicting actions.
            CancelPendingActions();

            // Only close if the menu is transient (e.g. hover-activated) and is hoverable.
            if (!_isTransient || !IsHoverable(args))
            {
                return;
            }

            // Add a delay if one is configured.
            if (MudGlobal.MenuDefaults.HoverDelay > 0)
            {
                _leaveCts = new();

                try
                {
                    await Task.Delay(MudGlobal.MenuDefaults.HoverDelay, _leaveCts.Token);
                }
                catch (TaskCanceledException)
                {
                    // Leave action was canceled, meaning the pointer re-entered the menu area.
                    return;
                }
            }

            // Close the menu only if the pointer is no longer over this menu or any of its sub-menus.
            if (!HasPointerOver(this))
            {
                await CloseMenuAsync();
            }
        }

        /// <summary>
        /// Recursively checks if the pointer is currently over this menu or any of its sub-menus.
        /// </summary>
        /// <remarks>
        /// This is crucial for determining when to close hover-activated menus.
        /// </remarks>
        protected bool HasPointerOver(MudMenu menu)
        {
            if (menu._isPointerOver)
                return true;

            // Recursively check all child submenus.
            return menu._subMenus.Any(HasPointerOver);
        }

        /// <summary>
        /// Cancels any pending hover or leave actions.
        /// </summary>
        /// <remarks>
        /// This is called when a new menu action is initiated, preventing conflicting or stale operations.
        /// </remarks>
        private void CancelPendingActions()
        {
            // ReSharper disable MethodHasAsyncOverload
            // Cancels any ongoing hover-to-open or leave-to-close delays.
            _leaveCts?.Cancel();
            _hoverCts?.Cancel();
            // ReSharper restore MethodHasAsyncOverload
        }

        /// <summary>
        /// Implementation of IActivatable.Activate, toggles the menu.
        /// </summary>
        /// <remarks>
        /// This method serves as the entry point for activating the menu via an external activator.
        /// </remarks>
        void IActivatable.Activate(object activator, MouseEventArgs args)
        {
            // Prevent activation if the activator button has a specific CSS class that marks it as non-activatable.
            if (activator is MudBaseButton activatorButton &&
                (activatorButton.Class?.Contains("mud-no-activator") ?? false))
            {
                return;
            }

            ToggleMenuAsync(args).CatchAndLog();
        }


        /// <summary>
        /// Disposes managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Indicates if managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hoverCts?.Cancel();
                _hoverCts?.Dispose();

                _leaveCts?.Cancel();
                _leaveCts?.Dispose();

                ParentMenu?.UnregisterChild(this);
            }
        }

        /// <summary>
        /// Releases resources used by the component.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handles keyboard navigation and interaction logic within the menu. including arrow keys,
        /// enter/space to select or open submenus, tab to close and move focus, and escape to close.
        /// </summary>
        private async Task HandleKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key == "ArrowDown" || e.Key == "ArrowUp" || e.Key == "ArrowRight" || e.Key == "ArrowLeft")
            {
                await HandleNavigationKeyAsync(e);
            }
            else if (e.Key == "Enter" || e.Key == " ")
            {
                await HandleActivationKeyAsync(e);
            }
            else if (e.Key == "Tab" || e.Key == "Escape")
            {
                await HandleDismissalKeyAsync(e);
            }
        }

        /// <summary>
        /// Handles keyboard navigation and interaction logic within the menu for arrow keys.
        /// </summary>
        private async Task HandleNavigationKeyAsync(KeyboardEventArgs e)
        {
            var items = _menuItems.Where(x => x is MudMenuItem).ToList();
            if (items.Count == 0)
                return;

            switch (e.Key)
            {
                case "ArrowDown":
                    await MoveFocusAsync(1, items.Count);
                    break;

                case "ArrowUp":
                    await MoveFocusAsync(-1, items.Count);
                    break;

                case "ArrowRight":
                    await HandleArrowRightAsync();
                    break;

                case "ArrowLeft":
                    await HandleArrowLeftAsync();
                    break;
            }
        }

        /// <summary>
        /// Moves focus up or down in the menu.
        /// </summary>
        private Task MoveFocusAsync(int direction, int itemCount)
        {
            _focusedIndex = (_focusedIndex + direction + itemCount) % itemCount;

            return FocusItemAsync(_focusedIndex);
        }

        /// <summary>
        /// Handles the ArrowRight key - opens submenu or invokes click.
        /// </summary>
        private async Task HandleArrowRightAsync()
        {
            if (_focusedIndex >= 0 && _focusedIndex < _menuItems.Count)
            {
                var currentItem = _menuItems[_focusedIndex];

                switch (currentItem)
                {
                    case MudMenuItem menuItem:
                        var submenu = FindSubmenuForItem(menuItem);
                        if (submenu != null)
                        {
                            await submenu.OpenSubMenuAsync(EventArgs.Empty);
                        }
                        else
                        {
                            await menuItem.OnClick.InvokeAsync();
                        }
                        break;

                    case MudMenu menu:
                        await menu.OpenSubMenuAsync(EventArgs.Empty);
                        break;
                }
            }
        }

        /// <summary>
        /// Handles the ArrowLeft key - closes current submenu or all menus.
        /// </summary>
        private async Task HandleArrowLeftAsync()
        {
            // Exit to parent menu if this is a submenu
            if (ParentMenu != null)
            {
                await CloseMenuAsync();

                // Return focus to the parent menu
                if (ParentMenu._focusedIndex >= 0 && ParentMenu._focusedIndex < ParentMenu._menuItems.Count)
                {
                    await ParentMenu.FocusItemAsync(ParentMenu._focusedIndex);
                }
            }
            else
            {
                // Close the menu if there are no further menu items on the arrow left
                await CloseAllMenusAsync();
            }
        }

        /// <summary>
        /// Handles keyboard navigation and interaction logic within the menu and submenu for enter/space
        /// </summary>
        private async Task HandleActivationKeyAsync(KeyboardEventArgs e)
        {
            if (_menuItems.Count == 0)
                return;

            if (!_lastInteractionWasKeyboard)
            {
                await OpenMenuAsync(e);
            }

            if (_focusedIndex >= 0 && _focusedIndex < _menuItems.Count)
            {
                var currentItem = _menuItems[_focusedIndex];

                // Handle different item types
                switch (currentItem)
                {
                    case MudMenuItem menuItem:
                        // If this item has a submenu, open it instead of invoking click
                        var submenu = FindSubmenuForItem(menuItem);
                        if (submenu != null)
                        {
                            submenu._lastKeyboardActivation = DateTime.UtcNow;
                            submenu._lastInteractionWasKeyboard = true;
                            await submenu.OpenSubMenuAsync(EventArgs.Empty);
                        }
                        else
                        {
                            await menuItem.OnClickHandlerAsync(new MouseEventArgs());
                            _lastInteractionWasKeyboard = false;
                        }
                        break;

                    case MudMenu menu:
                        // For MudMenu items, always open the submenu
                        menu._lastKeyboardActivation = DateTime.UtcNow;
                        menu._lastInteractionWasKeyboard = true;
                        await menu.OpenSubMenuAsync(EventArgs.Empty);
                        break;
                }
            }
        }

        /// <summary>
        /// Handles keyboard navigation and interaction logic within the menu for tab to close and move focus, and escape to close.
        /// </summary>
        private async Task HandleDismissalKeyAsync(KeyboardEventArgs e)
        {
            if (_menuItems.Count == 0)
                return;

            if (e.Key == "Tab")
            {
                await CloseAllMenusAsync();
            }
            else if (e.Key == "Escape")
            {
                // Close current menu or all menus if at top level
                if (ParentMenu != null)
                {
                    await CloseMenuAsync();
                    if (ParentMenu._focusedIndex >= 0 && ParentMenu._focusedIndex < ParentMenu._menuItems.Count)
                    {
                        await ParentMenu.FocusItemAsync(ParentMenu._focusedIndex);
                    }
                }
                else
                {
                    await CloseAllMenusAsync();
                }
            }
        }

        /// <summary>
        /// Runs after component rendering. If the menu is open and no item is focused,
        /// it automatically focuses the first enabled item in the list.
        /// </summary>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (_openState.Value && _focusedIndex == -1)
            {
                // Focus the container first. This makes the menu "listen" for keys.
                if (_menuWrapperRef.Context is not null)
                    await _menuWrapperRef.FocusAsync(preventScroll: true);

                // Check if opened with keyboard and focus the first item
                if (_lastInteractionWasKeyboard && _menuItems.Count > 0)
                {
                    _focusedIndex = 0;
                    await FocusItemAsync(_focusedIndex);
                }
            }
        }

        /// <summary>
        /// Registers a new menu item or submenu with the current menu.
        /// Ensures the item is only added once
        /// </summary>
        internal void RegisterItem(object item)
        {
            if (!_menuItems.Contains(item))
            {
                _menuItems.Add(item);
            }
        }

        /// <summary>
        /// Sets focus to the menu item at the specified index, if the index is valid.
        /// </summary>
        internal async Task FocusItemAsync(int index)
        {
            if (index >= 0 && index < _menuItems.Count)
            {
                var item = _menuItems[index];

                // Retrieves the cref ElementRef associated with a menu item or submenu to allow focus control.
                ElementReference elementRef = item switch
                {
                    MudMenuItem menuItem => menuItem.ElementReference,
                    MudMenu menu => menu._menuItemActivator?.ElementReference ?? default,
                    _ => default
                };

                if (elementRef.Context is not null)
                {
                    await elementRef.FocusAsync();
                }
            }
        }

        /// <summary>
        /// Subscribes to keyboard events for this menu using the <see cref="KeyInterceptorService"/>,
        /// preventing default browser scrolling behaviour for certain keys.
        /// </summary>
        private Task SubscribeToMenuKeyInterceptorAsync()
        {
            // Subscribe key interceptor to prevent default scrolling
            var options = new KeyInterceptorOptions(
                "mud-list",
                [
                    // prevent scrolling page
                    new("ArrowDown", preventDown: "key+none"),
                    new("ArrowUp", preventDown: "key+none"),
                ]);
            return KeyInterceptorService.SubscribeAsync(_elementId, options, keyDown: HandleKeyDownAsync);
        }

        /// <summary>
        /// Focuses the activator element that opened this menu. This could be a button,
        /// icon button, or another menu item depending on the context.
        /// </summary>
        private async Task FocusActivatorAsync()
        {
            try
            {
                if (ParentMenu is null)
                {
                    if (_buttonActivator is not null)
                    {
                        await _buttonActivator.FocusAsync();
                    }
                    else if (_iconButtonActivator is not null)
                    {
                        await _iconButtonActivator.FocusAsync();
                    }
                }
                else
                {
                    if (_menuItemActivator is not null)
                    {
                        await _menuItemActivator.ElementReference.FocusAsync();
                    }
                }
            }
            catch (JSException)
            {
                // No focus added - menu closed without focusing, as the element is likely gone from the DOM.
            }
        }

        /// <summary>
        /// Finds the submenu associated with a given menu item by checking the _subMenus collection.
        /// </summary>
        private MudMenu? FindSubmenuForItem(MudMenuItem menuItem)
        {
            return _subMenus.FirstOrDefault(submenu => submenu._menuItemActivator == menuItem);
        }

        /// <summary>
        /// Track whether the menu is selected or not and move to the correct position on Arrow Up/Down
        /// </summary>
        /// <param name="e"></param>
        private void TrackKeyboardInteraction(KeyboardEventArgs e)
        {
            _lastInteractionWasKeyboard = true;

            if (_openState.Value && _focusedIndex == -1 && _menuItems.Count > 0)
            {
                if (e.Key == "ArrowDown")
                {
                    _focusedIndex = 0;
                    _ = InvokeAsync(() => FocusItemAsync(_focusedIndex));
                }
                else if (e.Key == "ArrowUp")
                {
                    _focusedIndex = _menuItems.Count - 1;
                    _ = InvokeAsync(() => FocusItemAsync(_focusedIndex));
                }
            }
        }

        /// <summary>
        /// Handle activator keydown when activator content is added to the menu/submenus. 
        /// </summary>
        /// <param name="e"></param>
        private async Task HandleActivatorKeydown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" || e.Key == " ")
            {
                _lastInteractionWasKeyboard = true;
                _lastKeyboardActivation = DateTime.UtcNow;

                await ToggleMenuAsync(e);
            }
        }
    }
}
