using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Allows registering a hotkey.
/// </summary>
public partial class MudHotkey : MudComponentBase, IDisposable
{
    private const string RegisterJsMethodName = "mudHotkeyListener.registerHotkey";
    private const string UnregisterJsMethodName = "mudHotkeyListener.unregisterHotkey";

    private bool _disposed;
    private bool _isRendered;
    private bool _childContentIsVisible;
    private readonly string _hotkeyId = Guid.NewGuid().ToString();
    private readonly DotNetObjectReference<MudHotkey> _dotNetObjectReference;

    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    /// <summary>
    /// The optional content to be displayed when the hotkey is pressed.
    /// </summary>
    [Parameter, Category(CategoryTypes.Hotkey.Appearance)]
    public RenderFragment? ChildContent { get; set; }

    /// <summary>
    /// The hotkey to register.
    /// </summary>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)]
    public JsKey Key { get; set; }

    /// <summary>
    /// The modifiers the user has to press together with <see cref="Key"/> to trigger the hotkey.
    /// </summary>
    /// <remarks>
    /// If left empty the hotkey will be triggered by pressing <see cref="Key"/> alone.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)]
    public IEnumerable<JsKeyModifier> KeyModifiers { get; set; } = [];
    /// <summary>
    /// Occurs when <see cref="Key"/> and <see cref="KeyModifiers"/> are pressed.
    /// </summary>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)]
    public EventCallback OnHotkeyPressed { get; set; }

    /// <summary>
    /// Whether to hide the child content when the hotkey is pressed again, allowing for a toggle behavior.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)]
    public bool HideChildContentOnRepress { get; set; }

    /// <summary>
    /// Whether to prevent the key press event from propagating.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)]
    public bool PreventEventPropagation { get; set; } = true;

    /// <summary>
    /// Ignores the hotkey when set to true.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)]
    public bool Disabled { get; set; }

    [DynamicDependency(nameof(MudHotkeyProviderJsCallback))]
    public MudHotkey()
    {
        _dotNetObjectReference = DotNetObjectReference.Create(this);
        using var registerScope = CreateRegisterScope();
        registerScope.RegisterParameter<bool>(nameof(Disabled))
            .WithParameter(() => Disabled)
            .WithChangeHandler(OnDisabledChangedAsync);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            if (!Disabled)
            {
                await RegisterHotkeyAsync();
            }

            _isRendered = true;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (_isRendered && !Disabled)
        {
            await RegisterHotkeyAsync();
        }
    }

    private async Task RegisterHotkeyAsync()
    {
        await JsRuntime.InvokeVoidAsync(RegisterJsMethodName,
            _dotNetObjectReference,
            nameof(MudHotkeyProviderJsCallback),
            _hotkeyId,
            Key,
            KeyModifiers,
            PreventEventPropagation);
    }

    private async Task UnregisterHotkeyAsync()
    {
        await JsRuntime.InvokeVoidAsync(UnregisterJsMethodName, _hotkeyId);
    }

    private Task OnDisabledChangedAsync(ParameterChangedEventArgs<bool> args)
    {
        return !args.Value
            ? RegisterHotkeyAsync()
            : UnregisterHotkeyAsync();
    }

    [JSInvokable]
    public async Task MudHotkeyProviderJsCallback()
    {
        if (!_childContentIsVisible)
        {
            _childContentIsVisible = true;
            await InvokeAsync(StateHasChanged);
        }
        else if (HideChildContentOnRepress)
        {
            _childContentIsVisible = false;
            await InvokeAsync(StateHasChanged);
        }

        await OnHotkeyPressed.InvokeAsync();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _dotNetObjectReference.Dispose();
            }

            _disposed = true;
        }
    }
}
