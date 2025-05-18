// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable

/// <summary>
/// A layer which darkens a window, often as part of showing a <see cref="MudDialog" />.
/// </summary>
public partial class MudOverlay : MudComponentBase, IPointerEventsNoneObserver, IAsyncDisposable
{
    private int _lockCount;
    private bool _previousAbsolute;
    private bool _previousLockScroll;
    private readonly ParameterState<bool> _visibleState;
    private readonly string _elementId = Identifier.Create("overlay");

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
            .AddStyle("pointer-events", "none", !Modal)
            .AddStyle(Style)
            .Build();

    /// <summary>
    /// The manager for scroll events.
    /// </summary>
    [Inject]
    public IScrollManager ScrollManager { get; set; } = null!;

    /// <summary>
    /// Pointer events none service when pointer events are set to none.
    /// </summary>
    [Inject]
    private IPointerEventsNoneService PointerEventsNoneService { get; set; } = null!;

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
    /// Occurs when <see cref="AutoClose"/> changes.
    /// </summary>
    /// <remarks>
    /// This event is triggered when the auto-close behavior of the overlay changes.
    /// </remarks>
    public EventCallback<bool> AutoCloseChanged { get; set; }

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
    /// Prevents interaction with background elements.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter]
    [Category(CategoryTypes.Overlay.Behavior)]
    public bool Modal { get; set; } = true;

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

    /// <summary>
    /// Determines whether the overlay should be rendered outside of the section. If it's false, the overlay will be rendered with the MudPopOverProvider.
    /// If it's true it will be rendered as is where is (v7 and previous behavior)
    /// </summary>
    /// <remarks>
    /// If the user sets Absolute to true, the user intends for it to be part of his markup and not rendered by the MudPopoverProvider
    /// Dialog's need the separation of the overlay for display purposes
    /// If the user provides a child content, the user intends for it to be part of his markup and not rendered by the MudPopoverProvider
    /// </remarks>
    internal bool RenderOutsideOfSection =>
        Absolute ||
        (Class?.Contains("mud-skip-overlay-section") ?? false) ||
        ChildContent != null;

    string IPointerEventsNoneObserver.ElementId => _elementId;

    public MudOverlay()
    {
        using var registerScope = CreateRegisterScope();
        _visibleState = registerScope.RegisterParameter<bool>(nameof(Visible))
            .WithParameter(() => Visible)
            .WithEventCallback(() => VisibleChanged)
            .WithChangeHandler(HandleVisibleChanged);
    }

    protected override async Task OnAfterRenderAsync(bool firstTime)
    {
        // If the overlay is initially visible and modeless auto-close is enabled,
        // then start tracking pointer down events.
        if (firstTime && Visible && !Modal && AutoClose)
        {
            await StartModelessAutoCloseTrackingAsync();
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_previousLockScroll != LockScroll || _previousAbsolute != Absolute)
        {
            // handle lock scroll change when user changes LockScroll parameter
            _previousLockScroll = LockScroll;
            _previousAbsolute = Absolute;
            await HandleLockScrollChange();
        }

        if (Modal || !AutoClose)
        {
            return;
        }

        if (Visible)
        {
            await StartModelessAutoCloseTrackingAsync();
        }
        else
        {
            await StopModelessAutoCloseTrackingAsync();
        }
    }

    internal async Task HandleLockScrollChange()
    {
        if (LockScroll && !Absolute)
        {
            if (_visibleState.Value)
            {
                await BlockScrollAsync();
            }
            else
            {
                await UnblockScrollAsync();
            }
        }
    }

    // change lockscroll value when user toggles visible state
    private Task HandleVisibleChanged(ParameterChangedEventArgs<bool> args) => HandleLockScrollChange();

    protected internal async Task OnClickHandlerAsync(MouseEventArgs ev)
    {
        if (AutoClose)
        {
            await CloseOverlayAsync();
        }

        await OnClick.InvokeAsync(ev);
    }

    internal async Task CloseOverlayAsync()
    {
        await _visibleState.SetValueAsync(false);
        await OnClosed.InvokeAsync();
        await HandleLockScrollChange();
    }

    /// <summary>
    /// Locks the scroll by attaching a CSS class to the specified element, in this case the body.
    /// </summary>
    private ValueTask BlockScrollAsync()
    {
        // we only want to lock scroll once
        if (_lockCount > 0)
        {
            return ValueTask.CompletedTask;
        }

        _lockCount++;
        return ScrollManager.LockScrollAsync("body", LockScrollClass);
    }

    /// <summary>
    /// Removes the CSS class that prevented scrolling.
    /// </summary>
    private ValueTask UnblockScrollAsync()
    {
        _lockCount = Math.Max(0, _lockCount - 1);
        return ScrollManager.UnlockScrollAsync("body", LockScrollClass);
    }

    /// <summary>
    /// Subscribes to pointer down events to close the overlay when the user clicks outside of it.
    /// </summary>
    private async Task StartModelessAutoCloseTrackingAsync()
    {
        if (IsJSRuntimeAvailable)
        {
            await PointerEventsNoneService.SubscribeAsync(this, new() { SubscribeDown = true });
        }
    }

    /// <summary>
    /// Unsubscribes from pointer down events.
    /// </summary>
    private async Task StopModelessAutoCloseTrackingAsync()
    {
        if (IsJSRuntimeAvailable)
        {
            await PointerEventsNoneService.UnsubscribeAsync(this);
        }
    }

    Task IPointerDownObserver.NotifyOnPointerDownAsync(EventArgs args) => CloseOverlayAsync();

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (!IsJSRuntimeAvailable)
        {
            return;
        }

        if (_lockCount > 0)
        {
            await UnblockScrollAsync();
        }

        await StopModelessAutoCloseTrackingAsync();
    }
}
