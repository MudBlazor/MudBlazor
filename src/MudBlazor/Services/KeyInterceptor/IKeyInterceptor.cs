// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// This transient service binds itself to a parent element to observe the keys of one of its children.
/// It can call preventDefault or stopPropagation directly on the JavaScript side for single keystrokes / key combinations as per configuration.
/// Furthermore, you can precisely subscribe single keystrokes or combinations and only the subscribed ones will be forwarded into .NET
/// </summary>
[Obsolete($"Use {nameof(IKeyInterceptorService)} instead. This will be removed in MudBlazor 8.")]
public interface IKeyInterceptor : IDisposable
{
    /// <summary>
    /// Event raised when a key down event is observed.
    /// </summary>
    event KeyboardEvent KeyDown;

    /// <summary>
    /// Event raised when a key up event is observed.
    /// </summary>
    event KeyboardEvent KeyUp;

    /// <summary>
    /// Connects to the ancestor element of the element(s) that should be observed.
    /// </summary>
    /// <param name="elementId">The unique identifier of the ancestor HTML element.</param>
    /// <param name="options">Defines the descendant(s) by setting <see cref="KeyInterceptorOptions.TargetClass"/> and the keystrokes to be monitored or suppressed.</param>
    Task Connect(string elementId, KeyInterceptorOptions options);

    /// <summary>
    /// Disconnects from the previously connected ancestor and its descendants.
    /// </summary>
    Task Disconnect();

    /// <summary>
    /// Updates the behavior of a registered <see cref="KeyOptions"/>.
    /// The keystroke to update must have been monitored previously.
    /// </summary>
    /// <param name="option">The <see cref="KeyOptions"/> to update.</param>
    Task UpdateKey(KeyOptions option);
}
