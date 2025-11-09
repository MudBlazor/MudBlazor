#nullable enable
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// Allows registering a hotkey.
/// </summary>
public partial class MudHotkey : MudComponentBase
{
    private const string RegisterJsMethodName = "mudHotkeyListener.registerHotkey";

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

    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    private bool _childContentIsVisible;
    private bool _isRendered;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await RegisterHotkeyAsync();
            _isRendered = true;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (_isRendered) await RegisterHotkeyAsync();
    }

    private async Task RegisterHotkeyAsync()
    {
        await JsRuntime.InvokeVoidAsync(RegisterJsMethodName, DotNetObjectReference.Create(this), nameof(MudHotkeyProviderJsCallback), Key, KeyModifiers);
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
