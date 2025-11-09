using System.Diagnostics.CodeAnalysis;
using Microsoft.JSInterop;
using MudBlazor.Utilities;

namespace MudBlazor.Hotkey;

/// <summary>
/// Allows registering hotkeys.
/// </summary>
public interface IHotkeyService
{
    /// <summary>
    /// Registers a global hotkey.
    /// </summary>
    /// <remarks>
    /// Global meaning that the hotkey will be active for the whole application and not only for the current page.
    /// </remarks>
    /// <param name="key">The key the user has to press for this hotkey.</param>
    /// <param name="keyModifiers">The modifiers the user has to press in addition to the <c>key</c> for this hotkey.</param>
    /// <returns></returns>
    public Task RegisterHotkeyAsync<TComponent>(JsKey key, params IEnumerable<JsKeyModifier> keyModifiers);

    /// <summary>
    /// Unregisters a global hotkey.
    /// </summary>
    /// <remarks>
    /// Note that you have to pass <b>exactly</b> the same parameters as you passed to <see cref="RegisterHotkeyAsync"/>.
    /// </remarks>
    /// <param name="key">The key the user has to press for this hotkey.</param>
    /// <param name="keyModifiers">The modifiers the user has to press in addition to the <c>key</c> for this hotkey.</param>
    /// <returns></returns>
    public Task UnregisterHotkeyAsync<TComponent>(JsKey key, params IEnumerable<JsKeyModifier> keyModifiers);

    /// <summary>
    /// Unregisters all global hotkeys.
    /// </summary>
    /// <returns></returns>
    public Task UnregisterAllHotkeysAsync();
}
