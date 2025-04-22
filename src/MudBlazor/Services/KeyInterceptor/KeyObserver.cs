// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

#nullable enable
/// <summary>
/// Represents a key observer that handles key down and key up events for a specific HTML element.
/// </summary>
public class KeyObserver : IKeyInterceptorObserver, IEquatable<KeyObserver>
{
    private readonly string _elementId;
    private readonly IKeyDownObserver _keyDownObserver;
    private readonly IKeyUpObserver _keyUpObserver;
    private static readonly KeyObserverIgnore _keyObserverIgnore = new();

    /// <inheritdoc />
    string IKeyInterceptorObserver.ElementId => _elementId;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyObserver"/> class.
    /// </summary>
    /// <param name="elementId">The unique identifier of the HTML element to observe.</param>
    /// <param name="keyDownObserver">The observer for key down events.</param>
    /// <param name="keyUpObserver">The observer for key up events.</param>
    internal KeyObserver(string elementId, IKeyDownObserver? keyDownObserver, IKeyUpObserver? keyUpObserver)
    {
        _elementId = elementId;
        _keyDownObserver = keyDownObserver ?? _keyObserverIgnore;
        _keyUpObserver = keyUpObserver ?? _keyObserverIgnore;
    }

    /// <inheritdoc />
    Task IKeyDownObserver.NotifyOnKeyDownAsync(KeyboardEventArgs args) => _keyDownObserver.NotifyOnKeyDownAsync(args);

    /// <inheritdoc />
    Task IKeyUpObserver.NotifyOnKeyUpAsync(KeyboardEventArgs args) => _keyUpObserver.NotifyOnKeyUpAsync(args);

    /// <summary>
    /// Creates an <see cref="IKeyUpObserver"/> that invokes the specified asynchronous lambda function on key up events.
    /// </summary>
    /// <param name="lambda">The lambda function to invoke on key up events.</param>
    /// <returns>An instance of <see cref="IKeyUpObserver"/>.</returns>
    public static IKeyUpObserver KeyUp(Func<KeyboardEventArgs, Task>? lambda) => new KeyUpLambdaTaskObserver(lambda);

    /// <summary>
    /// Creates an <see cref="IKeyUpObserver"/> that invokes the specified lambda action on key up events.
    /// </summary>
    /// <param name="lambda">The lambda action to invoke on key up events.</param>
    /// <returns>An instance of <see cref="IKeyUpObserver"/>.</returns>
    public static IKeyUpObserver KeyUp(Action<KeyboardEventArgs>? lambda) => new KeyUpLambdaObserver(lambda);

    /// <summary>
    /// Creates an <see cref="IKeyDownObserver"/> that invokes the specified asynchronous lambda function on key down events.
    /// </summary>
    /// <param name="lambda">The lambda function to invoke on key down events.</param>
    /// <returns>An instance of <see cref="IKeyDownObserver"/>.</returns>
    public static IKeyDownObserver KeyDown(Func<KeyboardEventArgs, Task>? lambda) => new KeyDownLambdaTaskObserver(lambda);

    /// <summary>
    /// Creates an <see cref="IKeyDownObserver"/> that invokes the specified lambda action on key down events.
    /// </summary>
    /// <param name="lambda">The lambda action to invoke on key down events.</param>
    /// <returns>An instance of <see cref="IKeyDownObserver"/>.</returns>
    public static IKeyDownObserver KeyDown(Action<KeyboardEventArgs>? lambda) => new KeyDownLambdaObserver(lambda);

    /// <summary>
    /// Gets an <see cref="IKeyUpObserver"/> that ignores key up events.
    /// </summary>
    /// <returns>An instance of <see cref="IKeyUpObserver"/> that ignores key up events.</returns>
    public static IKeyUpObserver KeyUpIgnore() => _keyObserverIgnore;

    /// <summary>
    /// Gets an <see cref="IKeyDownObserver"/> that ignores key down events.
    /// </summary>
    /// <returns>An instance of <see cref="IKeyDownObserver"/> that ignores key down events.</returns>
    public static IKeyDownObserver KeyDownIgnore() => _keyObserverIgnore;

    /// <inheritdoc />
    public bool Equals(KeyObserver? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return _elementId == other._elementId;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is KeyObserver keyObserver && Equals(keyObserver);

    /// <inheritdoc />
    public override int GetHashCode() => _elementId.GetHashCode();

    private class KeyDownLambdaObserver : IKeyDownObserver
    {
        private readonly Action<KeyboardEventArgs>? _lambda;

        public KeyDownLambdaObserver(Action<KeyboardEventArgs>? lambda) => _lambda = lambda;

        /// <inheritdoc />
        public Task NotifyOnKeyDownAsync(KeyboardEventArgs args)
        {
            _lambda?.Invoke(args);

            return Task.CompletedTask;
        }
    }

    private class KeyDownLambdaTaskObserver : IKeyDownObserver
    {
        private readonly Func<KeyboardEventArgs, Task>? _lambda;

        public KeyDownLambdaTaskObserver(Func<KeyboardEventArgs, Task>? lambda) => _lambda = lambda;

        /// <inheritdoc />
        public Task NotifyOnKeyDownAsync(KeyboardEventArgs args) => _lambda is null ? Task.CompletedTask : _lambda(args);
    }

    private class KeyUpLambdaObserver : IKeyUpObserver
    {
        private readonly Action<KeyboardEventArgs>? _lambda;

        public KeyUpLambdaObserver(Action<KeyboardEventArgs>? lambda) => _lambda = lambda;

        /// <inheritdoc />
        public Task NotifyOnKeyUpAsync(KeyboardEventArgs args)
        {
            _lambda?.Invoke(args);

            return Task.CompletedTask;
        }
    }

    private class KeyUpLambdaTaskObserver : IKeyUpObserver
    {
        private readonly Func<KeyboardEventArgs, Task>? _lambda;

        public KeyUpLambdaTaskObserver(Func<KeyboardEventArgs, Task>? lambda) => _lambda = lambda;

        /// <inheritdoc />
        public Task NotifyOnKeyUpAsync(KeyboardEventArgs args) => _lambda is null ? Task.CompletedTask : _lambda(args);
    }

    private class KeyObserverIgnore : IKeyDownObserver, IKeyUpObserver;
}
