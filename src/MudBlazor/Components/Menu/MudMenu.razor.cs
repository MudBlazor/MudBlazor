// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
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
        private CancellationTokenSource? _hoverCts;
        private CancellationTokenSource? _leaveCts;

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

        [ExcludeFromCodeCoverage]
        [Obsolete($"Will be removed in future, replaced by {nameof(PositionAttributes)}.")]
        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("top", _openPosition.Top.ToPx(), PositionAtCursor)
                .AddStyle("left", _openPosition.Left.ToPx(), PositionAtCursor)
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
        /// The behavior of the dropdown popover menu
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="DropdownSettings.Fixed" /> false
        /// Defaults to <see cref="DropdownSettings.OverflowBehavior" /> <see cref="OverflowBehavior.FlipOnOpen" />
        /// </remarks>
        [Category(CategoryTypes.Popover.Behavior)]
        [Parameter]
        public DropdownSettings DropdownSettings { get; set; } = new DropdownSettings();

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
        /// Defaults to <c>true</c>.
        /// </remarks>
        [Parameter]
        [Category(CategoryTypes.Menu.PopupBehavior)]
        public bool Modal { get; set; } = MudGlobal.PopoverDefaults.ModalOverlay;

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
        [Parameter]
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
    }
}
