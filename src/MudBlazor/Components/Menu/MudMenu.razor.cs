// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Interfaces;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor
{
#nullable enable
    /// <summary>
    /// A list of choices displayed after clicking an element.
    /// </summary>
    /// <seealso cref="MudMenuItem" />
    public partial class MudMenu : MudComponentBase, IActivatable, IDisposable
    {
        private readonly ParameterState<bool> _openState;
        private readonly List<MudMenu> _children = [];
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

        protected string ListClassname =>
            new CssBuilder("mud-menu-list")
                .AddClass(ListClass)
                .Build();

        protected string ActivatorClassname =>
            new CssBuilder("mud-menu-activator")
                .AddClass("mud-disabled", Disabled)
                .Build();

        protected string Stylename =>
            new StyleBuilder()
                .AddStyle("top", _openPosition.Top.ToPx(), PositionAtCursor)
                .AddStyle("left", _openPosition.Left.ToPx(), PositionAtCursor)
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

        public IReadOnlyList<MudMenu> GetChildren() => _children.AsReadOnly();

        protected bool GetActivatorHidden() => ActivatorContent is null && string.IsNullOrWhiteSpace(Label) && string.IsNullOrWhiteSpace(Icon);

        protected void RegisterChild(MudMenu child)
        {
            _children.Add(child);
        }

        protected void UnregisterChild(MudMenu child)
        {
            _children.Remove(child);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ParentMenu?.RegisterChild(this);
        }

        protected Task OnOpenChanged(ParameterChangedEventArgs<bool> args)
        {
            return args.Value ?
                OpenMenuAsync(EventArgs.Empty) :
                CloseMenuAsync();
        }

        /// <summary>
        /// Closes this menu and all descendants.
        /// </summary>
        public async Task CloseMenuAsync()
        {
            foreach (var child in _children)
            {
                await child.CloseMenuAsync();
            }

            await _openState.SetValueAsync(false);
            StateHasChanged();
        }

        /// <summary>
        /// Closes all menus linked to this one, including parents, descendants, and itself.
        /// </summary>
        public async Task CloseAllMenusAsync()
        {
            var top = this;
            while (true)
            {
                if (top.ParentMenu is null)
                {
                    break;
                }
                else
                {
                    top = top.ParentMenu;
                }
            }

            await top.CloseMenuAsync();
        }

        /// <summary>
        /// Opens this menu.
        /// </summary>
        /// <param name="args">The <see cref="MouseEventArgs"/> or <see cref="PointerEventArgs"/> of the location of the click.
        /// When <see cref="PositionAtCursor"/> is <c>true</c>, the menu will be positioned at these coordinates.
        /// </param>
        /// <param name="transient">
        /// Defaults to <c>false</c>.  When <c>true</c>, no overlay will be displayed.
        /// This is used for menus which only stay open while the cursor is over them and is not used for click events.
        /// </param>
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

            if (args is MouseEventArgs mouseEventArgs)
            {
                _openPosition = (mouseEventArgs.PageY, mouseEventArgs.PageX);
            }

            // Don't open if already open. But let the stuff above get updated.
            if (_openState.Value)
            {
                return;
            }

            await _openState.SetValueAsync(true);
            StateHasChanged();
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

            return _openState.Value
                ? CloseMenuAsync()
                : OpenMenuAsync(args);
        }

        private async Task PointerEnterAsync(PointerEventArgs args)
        {
            _isPointerOver = true;

            // Cancel any existing mouse leave delay
            _leaveCts?.Cancel();

            // Start a new hover delay
            _hoverCts?.Cancel();
            _hoverCts = new();

            try
            {
                // Wait a bit to allow the cursor to move over the activator if the user isn't trying to open it.
                await Task.Delay(MudGlobal.MenuDefaults.HoverDelay, _hoverCts.Token);
            }
            catch (TaskCanceledException)
            {
                // Hover action was canceled.
                return;
            }

            if (_openState.Value || ActivationEvent != MouseEvent.MouseOver)
            {
                return;
            }

            // The Click event will interfere with the Enter event on devices that can't hover.
            if (args.PointerType is "touch" or "pen")
            {
                return;
            }

            await OpenMenuAsync(args, true);
        }

        /// <summary>
        /// Handles the pointer leave event.
        /// Closes the menu if the pointer leaves and does not re-enter within a specified delay.
        /// </summary>
        private async Task PointerLeaveAsync()
        {
            _isPointerOver = false;

            // We don't want to close the menu if it's not transient.
            if (!_isTransient)
            {
                return;
            }

            // Cancel any existing mouse hover delay
            _hoverCts?.Cancel();

            // Start a new leave delay
            _leaveCts?.Cancel();
            _leaveCts = new();

            try
            {
                // Wait a bit to allow the cursor to move from the activator to the items popover.
                await Task.Delay(MudGlobal.MenuDefaults.HoverDelay, _leaveCts.Token);
            }
            catch (TaskCanceledException)
            {
                // Leave action was canceled.
                return;
            }

            if (!_children.Any(x => x._isPointerOver))
                await CloseMenuAsync();
        }

        /// <summary>
        /// Implementation of IActivatable.Activate, toggles the menu.
        /// </summary>
        void IActivatable.Activate(object activator, MouseEventArgs args)
        {
            _ = ToggleMenuAsync(args);
        }

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
