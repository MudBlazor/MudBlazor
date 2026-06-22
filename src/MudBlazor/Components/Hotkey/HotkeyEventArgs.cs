// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using MudBlazor.Utilities;

namespace MudBlazor;

/// <summary>
/// The information passed when a <see cref="MudHotkey"/> is pressed.
/// </summary>
public class HotkeyEventArgs : EventArgs
{
    /// <summary>
    /// The key which triggered the hotkey.
    /// </summary>
    public JsKey Key { get; }

    /// <summary>
    /// The modifiers which were pressed together with <see cref="Key"/>.
    /// </summary>
    public IReadOnlyCollection<JsKeyModifier> KeyModifiers { get; }

    /// <summary>
    /// The <see cref="MudHotkey"/> which raised the event.
    /// </summary>
    public MudHotkey Sender { get; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="key">The key which triggered the hotkey.</param>
    /// <param name="keyModifiers">The modifiers which were pressed together with <paramref name="key"/>.</param>
    /// <param name="sender">The <see cref="MudHotkey"/> which raised the event.</param>
    public HotkeyEventArgs(JsKey key, IReadOnlyCollection<JsKeyModifier> keyModifiers, MudHotkey sender)
    {
        Key = key;
        KeyModifiers = keyModifiers;
        Sender = sender;
    }
}
