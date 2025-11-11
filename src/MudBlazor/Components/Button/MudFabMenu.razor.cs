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
        .AddClass("open", Open)
        .AddClass("dampen", DampenItemColors)
        .AddClass($"align-{AlignItems.ToDescriptionString()}")
        .AddClass(MenuClass)
        .Build();

    private string ClassnameFab => new CssBuilder("mud-fab-menu-button")
        .AddClass("open", Open && string.IsNullOrEmpty(Label))
        .AddClass(ButtonClass)
        .Build();

    private string Stylename => new StyleBuilder()
        .AddStyle("right: 16px;", Fixed)
        .AddStyle("bottom: 16px;", Fixed)
        .AddStyle("z-index: 2000;", Fixed)
        .AddStyle(Style)
        .Build();

    private string StylenameMenu => new StyleBuilder()
        .AddStyle($"padding-bottom: {_menuPaddingBottomCorrection}px;", _menuPaddingBottomCorrection != 0)
        .AddStyle(MenuStyle)
        .Build();

    private string? _startIcon;
    private string? _endIcon;
    private int _menuPaddingBottomCorrection;
    private bool _lastInteractionWasTouch;

    /// <summary>
    /// The CSS classes applied to the menu button.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separated by spaces.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Appearance)] public string? ButtonClass { get; set; }

    /// <summary>
    /// The CSS style applied to the menu button.
    /// </summary>
    [Parameter, Category(CategoryTypes.Button.Appearance)] public string? ButtonStyle { get; set; }

    /// <summary>
    /// The CSS classes applied to the item list.
    /// </summary>
    /// <remarks>
    /// Multiple classes must be separated by spaces.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Appearance)] public string? MenuClass { get; set; }

    /// <summary>
    /// The CSS style applied to the  item list.
    /// </summary>
    [Parameter, Category(CategoryTypes.Button.Appearance)] public string? MenuStyle { get; set; }

    /// <summary>
    /// The <see cref="MudFabMenuItem" /> components within this menu.
    /// </summary>
    /// <remarks>
    /// Note that you can add any component you like as long as it has the <c>mud-fab-menu-item</c> class.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Menu.PopupBehavior)] public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Whether this menu is open and the menu items are visible.
    /// </summary>
    /// <remarks>
    /// When this property changes, <see cref="OpenChanged"/> occurs.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Menu.PopupBehavior)] public bool Open { get; set; }

    /// <summary>
    /// Occurs when <see cref="Open"/> has changed.
    /// </summary>
    [Parameter, Category(CategoryTypes.Menu.PopupBehavior)] public EventCallback<bool> OpenChanged { get; set; }

    /// <summary>
    /// Sets the menu to a fixed position in the bottom right corner of the screen with padding of 16 px towards each screen edge.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Behavior)] public bool Fixed { get; set; }

    /// <summary>
    /// Whether to replace the set icon with a close icon when the menu is open.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Behavior)] public bool UseCloseIcon { get; set; } = true;

    /// <summary>
    /// Dampens the background color of the menu items when set to true.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Behavior)] public bool DampenItemColors { get; set; } = true;

    /// <summary>
    /// The alignment of the menu items in respect to the menu button.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>AlignItems.Center</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Button.Behavior)] public AlignItems AlignItems { get; set; } = AlignItems.Center;

    /// <summary>
    /// Whether to open the menu on mouse hover.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Menu.Behavior)] public bool OpenOnMouseHover { get; set; } = true;

    /// <summary>
    /// Whether to close the menu when a menu item is clicked.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Menu.Behavior)] public bool CloseOnMenuItemClicked { get; set; } = true;

    public MudFabMenu()
    {
        using var registerScope = CreateRegisterScope();
        registerScope.RegisterParameter<bool>(nameof(Open))
            .WithParameter(() => Open)
            .WithEventCallback(() => OpenChanged)
            .WithChangeHandler(OnOpenChanged);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (!Open || !UseCloseIcon)
        {
            _startIcon = StartIcon;
            _endIcon = EndIcon;
        }

        if (!string.IsNullOrEmpty(Label))
        {
            _menuPaddingBottomCorrection = Size switch
            {
                Size.Large => 60,
                Size.Medium => 52,
                Size.Small => 48,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private void OnOpenChanged(ParameterChangedEventArgs<bool> args)
    {
        if (args.Value && UseCloseIcon)
        {
            if (StartIcon != null) _startIcon = Icons.Material.Outlined.Add;
            if (EndIcon != null) _endIcon = Icons.Material.Outlined.Add;
        }

        if (!args.Value)
        {
            _startIcon = StartIcon;
            _endIcon = EndIcon;
        }
    }

    private async Task ToggleOpenAsync(bool? open = null)
    {
        Open = open ?? !Open;
        await OpenChanged.InvokeAsync(Open);

        OnOpenChanged(new ParameterChangedEventArgs<bool>(nameof(Open), !Open, Open));
    }

    private async Task OnMenuButtonClickAsync(MouseEventArgs args)
    {
        await ToggleOpenAsync();
        await OnClickHandler(args);
    }

    private async Task OnMouseEnterLeaveAsync(bool enter)
    {
        if (OpenOnMouseHover && !_lastInteractionWasTouch) await ToggleOpenAsync(enter);
        _lastInteractionWasTouch = false;
    }

    private async Task OnMenuClickAsync()
    {
        if (CloseOnMenuItemClicked) await ToggleOpenAsync(false);
    }

    private void OnTouchStart()
    {
        _lastInteractionWasTouch = true;
    }
}
