#nullable enable
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.State;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Allows registering a hotkey.
/// </summary>
public partial class MudHotkey : MudComponentBase
{
    private const string RegisterJsMethodName = "mudHotkeyListener.registerHotkey";
    private const string UnregisterJsMethodName = "mudHotkeyListener.unregisterHotkey";

    /// <summary>
    /// The optional content to be displayed when the hotkey is pressed.
    /// </summary>
    [Parameter, Category(CategoryTypes.Hotkey.Appearance)] public RenderFragment? ChildContent { get; set; }
    /// <summary>
    /// The hotkey to register.
    /// </summary>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public JsKey Key { get; set; }
    /// <summary>
    /// The modifiers the user has to press together with <see cref="Key"/> to trigger the hotkey.
    /// </summary>
    /// <remarks>
    /// If left empty the hotkey will be triggered by pressing <see cref="Key"/> alone.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public IEnumerable<JsKeyModifier> KeyModifiers { get; set; } = [];
    /// <summary>
    /// Occurs when <see cref="Key"/> and <see cref="KeyModifiers"/> are pressed.
    /// </summary>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public EventCallback OnHotkeyPressed { get; set; }
    /// <summary>
    /// Whether to hide the child content when the hotkey is pressed again, allowing for a toggle behavior.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public bool HideChildContentOnRepress { get; set; }
    /// <summary>
    /// Whether to prevent the key press event from propagating.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>true</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public bool PreventEventPropagation { get; set; } = true;
    /// <summary>
    /// Ignores the hotkey when set to true.
    /// </summary>
    /// <remarks>
    /// Defaults to <c>false</c>.
    /// </remarks>
    [Parameter, Category(CategoryTypes.Hotkey.Behavior)] public bool Disabled { get; set; }

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    private readonly string _hotkeyId = Guid.NewGuid().ToString();
    private readonly DotNetObjectReference<MudHotkey> _dotNetObjectReference;
    private bool _childContentIsVisible;
    private bool _isRendered;

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
        if (firstRender)
        {
            if (!Disabled) await RegisterHotkeyAsync();
            _isRendered = true;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_isRendered && !Disabled) await RegisterHotkeyAsync();
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

    private async Task OnDisabledChangedAsync(ParameterChangedEventArgs<bool> args)
    {
        if (!args.Value)
        {
            await RegisterHotkeyAsync();
        }
        else
        {
            await UnregisterHotkeyAsync();
        }
    }

    [JSInvokable]
    public async Task MudHotkeyProviderJsCallback()
    {
        if (!_childContentIsVisible)
        {
            _childContentIsVisible = true;
            StateHasChanged();
        }
        else if (HideChildContentOnRepress)
        {
            _childContentIsVisible = false;
            StateHasChanged();
        }

        await OnHotkeyPressed.InvokeAsync();
    }
}
