using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor.Hotkey;

/// <inheritdoc cref="IHotkeyService"/>
internal class HotkeyService(IJSRuntime jsRuntime) : IHotkeyService
{
    private const string RegisterJsMethodName = "mudHotkeyListener.registerGlobalHotkey";
    private const string UnregisterJsMethodName = "mudHotkeyListener.unregisterGlobalHotkey";
    private const string UnregisterAllJsMethodName = "mudHotkeyListener.unregisterAllGlobalHotkeys";

    private IJSRuntime JsRuntime { get; } = jsRuntime;

    /// <inheritdoc cref="IHotkeyService.RegisterHotkeyAsync"/>
    public async Task RegisterHotkeyAsync<TComponent>(JsKey key, params IEnumerable<JsKeyModifier> keyModifiers)
    {
        await JsRuntime.InvokeVoidAsync(RegisterJsMethodName, key, keyModifiers ?? [], typeof(TComponent).FullName);
    }

    /// <inheritdoc cref="IHotkeyService.UnregisterHotkeyAsync"/>
    public async Task UnregisterHotkeyAsync<TComponent>(JsKey key, params IEnumerable<JsKeyModifier> keyModifiers)
    {
        await JsRuntime.InvokeVoidAsync(UnregisterJsMethodName, key, keyModifiers ?? [], typeof(TComponent).FullName);
    }

    /// <inheritdoc cref="IHotkeyService.UnregisterAllHotkeysAsync"/>
    public async Task UnregisterAllHotkeysAsync()
    {
        await JsRuntime.InvokeVoidAsync(UnregisterAllJsMethodName);
    }
}
