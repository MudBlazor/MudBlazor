using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor.Hotkey;

/// <inheritdoc cref="IGlobalHotkeyService"/>
internal class GlobalHotkeyService(IJSRuntime jsRuntime) : IGlobalHotkeyService
{
    private const string RegisterJsMethodName = "mudHotkeyListener.registerGlobalHotkey";
    private const string UnregisterJsMethodName = "mudHotkeyListener.unregisterGlobalHotkey";
    private const string UnregisterAllJsMethodName = "mudHotkeyListener.unregisterAllGlobalHotkeys";

    private IJSRuntime JsRuntime { get; } = jsRuntime;

    /// <inheritdoc cref="IGlobalHotkeyService.RegisterHotkeyAsync"/>
    public async Task RegisterHotkeyAsync(string assemblyName, string jsInvokableIdentifier, JsKey key, params IEnumerable<JsKeyModifier> keyModifiers)
    {
        await JsRuntime.InvokeVoidAsync(RegisterJsMethodName, key, keyModifiers ?? [], assemblyName, jsInvokableIdentifier);
    }

    /// <inheritdoc cref="IGlobalHotkeyService.UnregisterHotkeyAsync"/>
    public async Task UnregisterHotkeyAsync(string assemblyName, string jsInvokableIdentifier, JsKey key, params IEnumerable<JsKeyModifier> keyModifiers)
    {
        await JsRuntime.InvokeVoidAsync(UnregisterJsMethodName, key, keyModifiers ?? [], assemblyName, jsInvokableIdentifier);
    }

    /// <inheritdoc cref="IGlobalHotkeyService.UnregisterAllHotkeysAsync"/>
    public async Task UnregisterAllHotkeysAsync()
    {
        await JsRuntime.InvokeVoidAsync(UnregisterAllJsMethodName);
    }
}
