using MudBlazor.Utilities;

namespace MudBlazor.Hotkey;

/// <summary>
/// Allows registering hotkeys.
/// </summary>
public interface IGlobalHotkeyService
{
    /// <summary>
    /// Registers a global hotkey.
    /// </summary>
    /// <remarks>
    /// Global meaning that the hotkey will be active for the whole application and not only for the current page.
    /// </remarks>
    /// <param name="assemblyName">The name of your assembly (this typically the name of your .csproj file).</param>
    /// <param name="jsInvokableIdentifier">The js identifier for the function to call on hotkey pressed (see the example below for implementation details).</param>
    /// <param name="key">The key the user has to press for this hotkey.</param>
    /// <param name="keyModifiers">The modifiers the user has to press in addition to the <c>key</c> for this hotkey.</param>
    /// <example>
    /// <code>
    /// public static class JsInvokables
    /// {
    ///     [JSInvokable("HotkeyFunction")]
    ///     public static HotkeyFunction() { }
    /// }
    /// </code>
    /// </example>
    /// <returns></returns>
    public Task RegisterHotkeyAsync(string assemblyName, string jsInvokableIdentifier, JsKey key, params IEnumerable<JsKeyModifier> keyModifiers);

    /// <summary>
    /// Unregisters a global hotkey.
    /// </summary>
    /// <remarks>
    /// Note that you have to pass <b>exactly</b> the same parameters as you passed to <see cref="RegisterHotkeyAsync"/>.
    /// </remarks>
    /// <param name="assemblyName">The name of your assembly (this typically the name of your .csproj file).</param>
    /// <param name="jsInvokableIdentifier">The js identifier for the function to call on hotkey pressed.</param>
    /// <param name="key">The key the user has to press for this hotkey.</param>
    /// <param name="keyModifiers">The modifiers the user has to press in addition to the <c>key</c> for this hotkey.</param>
    /// <returns></returns>
    public Task UnregisterHotkeyAsync(string assemblyName, string jsInvokableIdentifier, JsKey key, params IEnumerable<JsKeyModifier> keyModifiers);

    /// <summary>
    /// Unregisters all global hotkeys.
    /// </summary>
    /// <returns></returns>
    public Task UnregisterAllHotkeysAsync();
}
