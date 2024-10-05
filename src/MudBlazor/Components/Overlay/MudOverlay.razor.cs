using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
public partial class MudOverlay : MudComponentBase, IAsyncDisposable
{
    private readonly ParameterState<bool> _visibleState;

    protected string Classname =>
        new CssBuilder("mud-overlay")
            .AddClass("mud-overlay-absolute", Absolute)
            .AddClass(Class)
            .Build();

    protected string ScrimClassname =>
        new CssBuilder("mud-overlay-scrim")
            .AddClass("mud-overlay-dark", DarkBackground)
            .AddClass("mud-overlay-light", LightBackground)
            .Build();

    protected string Styles =>
        new StyleBuilder()
            .AddStyle("z-index", $"{ZIndex}", ZIndex != 5)
            .AddStyle(Style)
            .Build();

    [Inject]
    public IScrollManager ScrollManager { get; set; } = null!;

    /// <summary>
    /// Child content of the component.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>null</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Behavior)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// Makes the overlay visible.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Behavior)]
    public bool Visible { get; set; }

    /// <summary>
    /// Occurs when <see cref="Visible"/> changes.
    /// </summary>
    /// <remarks>
    /// This event is triggered when the visibility of the overlay changes.
    /// </remarks>
    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    /// <summary>
    /// Sets <see cref="Visible"/> to <c>false</c> when the overlay is clicked and calls <see cref="OnClosed"/>.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// This is preferred over the previously used <c>OnClick</c> event.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.ClickAction)]
    public bool AutoClose { get; set; }

    /// <summary>
    /// Prevents the <c>Document.body</c> element from scrolling.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Behavior)]
    public bool LockScroll { get; set; } = true;

    /// <summary>
    /// The css class that will be added to body if lockscroll is used.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>"scroll-locked"</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Behavior)]
    public string LockScrollClass { get; set; } = "scroll-locked";

    /// <summary>
    /// Applies the theme's dark overlay color.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Appearance)]
    public bool DarkBackground { get; set; }

    /// <summary>
    /// Applies the theme's light overlay color.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Appearance)]
    public bool LightBackground { get; set; }

    /// <summary>
    /// Uses absolute positioning for the overlay.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Behavior)]
    public bool Absolute { get; set; }

    /// <summary>
    /// Sets the z-index of the overlay.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>5</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Behavior)]
    public int ZIndex { get; set; } = 5;

    /// <summary>
    /// Occurs when the overlay is clicked.
    /// </summary>
    /// <remarks>
    /// If you need to close the overlay automatically, you can use <see cref="AutoClose"/> and <see cref="OnClosed"/> instead. 
    /// </remarks>
    [Parameter]
    public EventCallback<MouseEventArgs> OnClick { get; set; }

    /// <summary>
    /// Occurs when the overlay is closed due to <see cref="AutoClose"/>.
    /// </summary>
    [Parameter]
    public EventCallback OnClosed { get; set; }

    public MudOverlay()
    {
        using var registerScope = CreateRegisterScope();
        _visibleState = registerScope.RegisterParameter<bool>(nameof(Visible))
            .WithParameter(() => Visible)
            .WithEventCallback(() => VisibleChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstTime)
    {
        if (!LockScroll || Absolute)
        {
            return;
        }

        if (Visible)
        {
            await BlockScrollAsync();
        }
        else
        {
            await UnblockScrollAsync();
        }
    }

    protected internal async Task OnClickHandlerAsync(MouseEventArgs ev)
    {
        if (AutoClose)
        {
            await _visibleState.SetValueAsync(false);
            await OnClosed.InvokeAsync();
        }

        await OnClick.InvokeAsync(ev);
    }

    /// <summary>
    /// Locks the scroll by attaching a CSS class to the specified element, in this case the body.
    /// </summary>
    private ValueTask BlockScrollAsync()
    {
        return ScrollManager.LockScrollAsync("body", LockScrollClass);
    }

    /// <summary>
    /// Removes the CSS class that prevented scrolling.
    /// </summary>
    private ValueTask UnblockScrollAsync()
    {
        return ScrollManager.UnlockScrollAsync("body", LockScrollClass);
    }

    public ValueTask DisposeAsync()
    {
        if (IsJSRuntimeAvailable)
        {
            return UnblockScrollAsync();
        }

        return ValueTask.CompletedTask;
    }
}
