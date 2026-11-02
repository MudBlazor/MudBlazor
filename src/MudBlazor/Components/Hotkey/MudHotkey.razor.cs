using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Allows registering a hotkey.
/// </summary>
public partial class MudHotkey : MudComponentBase, IAsyncDisposable
{
    private readonly string _hotkeyId = Identifier.Create("hotkey");
    private bool _childContentIsVisible;
    private DotNetObjectReference<MudHotkey>? _dotNetObjectReference;

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
    /// <remarks>
    /// If you would like to use a modifier key here, you also have to add it to <see cref="KeyModifiers"/>.
    /// </remarks>
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
    public EventCallback<HotkeyEventArgs> OnHotkeyPressed { get; set; }

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
        registerScope.RegisterParameter<JsKey>(nameof(Key))
            .WithParameter(() => Key)
            .WithChangeHandler(RegisterOrUpdateHotkeyAsync);
        registerScope.RegisterParameter<IEnumerable<JsKeyModifier>>(nameof(KeyModifiers))
            .WithParameter(() => KeyModifiers)
            .WithChangeHandler(RegisterOrUpdateHotkeyAsync);
        registerScope.RegisterParameter<bool>(nameof(PreventEventPropagation))
            .WithParameter(() => PreventEventPropagation)
            .WithChangeHandler(RegisterOrUpdateHotkeyAsync);
        registerScope.RegisterParameter<bool>(nameof(Disabled))
            .WithParameter(() => Disabled)
            .WithChangeHandler(OnDisabledChangedAsync);
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender && !Disabled)
        {
            await RegisterOrUpdateHotkeyAsync();
        }
    }

    private async Task RegisterOrUpdateHotkeyAsync()
    {
        if (!IsJSRuntimeAvailable)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudHotkeyListener.registerOrUpdateHotkey",
            _dotNetObjectReference,
            nameof(MudHotkeyProviderJsCallback),
            _hotkeyId,
            Key.ToString(),
            KeyModifiers.Select(m => m.ToString()).ToArray(),
            PreventEventPropagation);
    }

    private async Task UnregisterHotkeyAsync()
    {
        if (!IsJSRuntimeAvailable)
        {
            return;
        }

        await JsRuntime.InvokeVoidAsyncWithErrorHandling("mudHotkeyListener.unregisterHotkey", _hotkeyId);
    }

    private Task OnDisabledChangedAsync(ParameterChangedEventArgs<bool> args)
    {
        return !args.Value
            ? RegisterOrUpdateHotkeyAsync()
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

        await OnHotkeyPressed.InvokeAsync(new HotkeyEventArgs(Key, KeyModifiers.ToArray(), this));
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore();
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_dotNetObjectReference != null)
        {
            _dotNetObjectReference.Dispose();
            _dotNetObjectReference = null;

            await UnregisterHotkeyAsync();
        }
    }
}
