#nullable enable
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// A menu appearing from a <see cref="MudFab"/> that displays a list of items.
/// </summary>
/// <seealso cref="MudFabMenuItem" />
public partial class MudFabMenu : MudFab
{
    private new string Classname => new CssBuilder("mud-fab-menu-container")
        .AddClass("fixed", Fixed)
        .AddClass($"align-{AlignItems.ToDescriptionString()}")
        .AddClass(Class)
        .Build();

    private string ClassnameMenu => new CssBuilder("mud-fab-menu")
        .AddClass("mud-fab-menu-open", _openState.Value)
        .AddClass("mud-fab-menu-dampen", DampenItemsBackgroundColor)
        .AddClass($"align-{AlignItems.ToDescriptionString()}")
        .AddClass($"mud-fab-menu-{Size.ToString().ToLower()}", !string.IsNullOrEmpty(Label))
        .AddClass(MenuClass)
        .Build();

    private string ClassnameFab => new CssBuilder("mud-fab-menu-button")
        .AddClass("open", _openState.Value && string.IsNullOrEmpty(Label))
        .AddClass(ButtonClass)
        .Build();

    private readonly ParameterState<bool> _openState;
    private string? _startIcon;
    private string? _endIcon;
    private bool _lastInteractionWasTouch;

    /// <summary>
    /// The CSS classes applied to the menu button.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separated by spaces.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Appearance)]
    public string? ButtonClass { get; set; }

    /// <summary>
    /// The CSS style applied to the menu button.
    /// </summary>
    [Parameter, Category(CategoryTypes.Button.Appearance)]
    public string? ButtonStyle { get; set; }

    /// <summary>
    /// The CSS classes applied to the item list.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separated by spaces.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Appearance)]
    public string? MenuClass { get; set; }

    /// <summary>
    /// The CSS style applied to the  item list.
    /// </summary>
    [Parameter, Category(CategoryTypes.Button.Appearance)]
    public string? MenuStyle { get; set; }

    /// <summary>
    /// The <see cref="MudFabMenuItem" /> components within this menu.
    /// </summary>
    /// <remarks>
    /// Note that you can add any component you like as long as it has the <c>mud-fab-menu-item</c> class.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Menu.PopupBehavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether this menu is open and the menu items are visible.
    /// </summary>
    /// <remarks>
    /// When this property changes, <see cref="OpenChanged"/> occurs.
    /// </remarks>
    [Parameter, ParameterState, Category(CategoryTypes.Menu.PopupBehavior)]
    public bool Open { get; set; }

    /// <summary>
    /// Occurs when <see cref="Open"/> has changed.
    /// </summary>
    [Parameter, Category(CategoryTypes.Menu.PopupBehavior)]
    public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// Sets the menu to a fixed position in the bottom right corner of the screen with padding of 16 px towards each screen edge.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Behavior)]
    public bool Fixed { get; set; }

    /// <summary>
    /// Replaces the set icon with a close icon when the menu is open.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Behavior)]
    public bool UseCloseIcon { get; set; } = true;

    /// <summary>
    /// Dampens the background color of the menu items when set to true to increase the contrast between the menu FAB and the menu items
    /// to archive a similar effect as described in the <a href="https://m3.material.io/components/fab-menu/overview">Material Design guidelines</a>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Behavior)]
    public bool DampenItemsBackgroundColor { get; set; } = true;

    /// <summary>
    /// The alignment of the menu items in respect to the menu button.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>AlignItems.Center</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Behavior)]
    public AlignItems AlignItems { get; set; } = AlignItems.Center;

    /// <summary>
    /// Opens the menu on mouse hover.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Menu.Behavior)]
    public bool OpenOnMouseHover { get; set; } = true;

    /// <summary>
    /// Closes the menu when a menu item is clicked.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Menu.Behavior)]
    public bool CloseOnMenuItemClicked { get; set; } = true;

    public MudFabMenu()
    {
        using var registerScope = CreateRegisterScope();
        _openState = registerScope.RegisterParameter<bool>(nameof(Open))
            .WithParameter(() => Open)
            .WithEventCallback(() => OpenChanged)
            .WithChangeHandler(HandleOpenChanged);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (!_openState.Value || !UseCloseIcon)
        {
            _startIcon = StartIcon;
            _endIcon = EndIcon;
        }
    }

    private void HandleOpenChanged(ParameterChangedEventArgs<bool> args) => HandleOpenChanged(args.Value);

    private void HandleOpenChanged(bool open)
    {
        if (open && UseCloseIcon)
        {
            if (StartIcon != null)
            {
                _startIcon = Icons.Material.Outlined.Add;
            }

            if (EndIcon != null)
            {
                _endIcon = Icons.Material.Outlined.Add;
            }
        }

        if (!open)
        {
            _startIcon = StartIcon;
            _endIcon = EndIcon;
        }
    }

    private async Task ToggleMenuAsync(bool? open = null)
    {
        var isOpen = open ?? !_openState.Value;
        await _openState.SetValueAsync(isOpen);
        HandleOpenChanged(isOpen);
    }

    private async Task OnMenuButtonClickAsync(MouseEventArgs args)
    {
        await ToggleMenuAsync();
        await OnClickHandler(args);
    }

    private async Task OnMouseEnterLeaveAsync(bool enter)
    {
        if (OpenOnMouseHover && !_lastInteractionWasTouch)
        {
            await ToggleMenuAsync(enter);
        }

        _lastInteractionWasTouch = false;
    }

    private async Task OnMenuClickAsync()
    {
        if (CloseOnMenuItemClicked)
        {
            await ToggleMenuAsync(false);
        }
    }

    private void OnTouchStart()
    {
        _lastInteractionWasTouch = true;
    }
}
