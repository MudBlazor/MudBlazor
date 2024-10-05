// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor.Interop;

namespace MudBlazor.Services;

#nullable enable
/// <summary>
/// This transient service binds itself to a parent element to observe the keys of one of its children.
/// It can call preventDefault or stopPropagation directly on the JavaScript side for single keystrokes / key combinations as per configuration.
/// Furthermore, you can precisely subscribe single keystrokes or combinations and only the subscribed ones will be forwarded into .NET
/// </summary>
[ExcludeFromCodeCoverage]
[Obsolete($"Use {nameof(IKeyInterceptorService)} instead. This will be removed in MudBlazor 8.")]
public class KeyInterceptor : IKeyInterceptor
{
    private bool _isDisposed;
    private bool _isObserving;
    private string? _elementId;
    private readonly KeyInterceptorInterop _keyInterceptorInterop;
    private readonly DotNetObjectReference<KeyInterceptor> _dotNetRef;

    /// <inheritdoc />
    public event KeyboardEvent? KeyDown;

    /// <inheritdoc />
    public event KeyboardEvent? KeyUp;

    [DynamicDependency(nameof(OnKeyDown))]
    [DynamicDependency(nameof(OnKeyUp))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KeyboardEvent))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KeyboardEventArgs))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KeyOptions))]
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(KeyInterceptorOptions))]
    public KeyInterceptor(IJSRuntime jsRuntime)
    {
        _dotNetRef = DotNetObjectReference.Create(this);
        _keyInterceptorInterop = new KeyInterceptorInterop(jsRuntime);
    }

    /// <inheritdoc />
    public async Task Connect(string elementId, KeyInterceptorOptions options)
    {
        if (_isObserving || _isDisposed)
        {
            return;
        }

        _elementId = elementId;
        _isObserving = await _keyInterceptorInterop.Connect(_dotNetRef, elementId, options);
    }

    /// <inheritdoc />
    public async Task UpdateKey(KeyOptions option)
    {
        if (_elementId is not null)
        {
            await _keyInterceptorInterop.UpdateKey(_elementId, option);
        }
    }

    /// <inheritdoc />
    public async Task Disconnect()
    {
        if (_elementId is not null)
        {
            await _keyInterceptorInterop.Disconnect(_elementId);
        }

        _isObserving = false;
    }

    /// <summary>
    /// Invoked when a key down event is received from JavaScript.
    /// </summary>
    /// <param name="args">The <see cref="KeyboardEventArgs"/> representing the keyboard event arguments.</param>
    [JSInvokable]
    public void OnKeyDown(KeyboardEventArgs args)
    {
        KeyDown?.Invoke(args);
    }

    /// <summary>
    /// Invoked when a key up event is received from JavaScript.
    /// </summary>
    /// <param name="args">The <see cref="KeyboardEventArgs"/> representing the keyboard event arguments.</param>
    [JSInvokable]
    public void OnKeyUp(KeyboardEventArgs args)
    {
        KeyUp?.Invoke(args);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing || _isDisposed)
        {
            return;
        }

        _isDisposed = true;
        KeyDown = null;
        KeyUp = null;
        Disconnect().CatchAndLog();
        _dotNetRef.Dispose();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
