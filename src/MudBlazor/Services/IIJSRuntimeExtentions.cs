// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace MudBlazor
{
    public static class IIJSRuntimeExtentions
    {
        /// <summary>
        /// Invokes the specified JavaScript function asynchronously and catches JSException, JSDisconnectedException and TaskCanceledException
        /// </summary>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
        public static async ValueTask InvokeVoidAsyncIgnoreErrors(this IJSRuntime jsRuntime, string identifier, params object[] args)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync(identifier, args);
            }
#if DEBUG
#else
            catch (JSException)
            {
            }
#endif
            // catch prerending errors since there is no browser at this point.
            catch (InvalidOperationException ex) when (ex.Message.Contains("prerender", StringComparison.InvariantCultureIgnoreCase))
            {
            }
            catch (JSDisconnectedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously and catches JSException, JSDisconnectedException and TaskCanceledException
        /// </summary>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
        /// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
        /// </param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation.</returns>
        public static async ValueTask InvokeVoidAsyncIgnoreErrors(this IJSRuntime jsRuntime, string identifier, CancellationToken cancellationToken, params object[] args)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync(identifier, cancellationToken, args);
            }
#if DEBUG
#else
            catch (JSException)
            {
            }
#endif
            // catch prerending errors since there is no browser at this point.
            catch (InvalidOperationException ex) when (ex.Message.Contains("prerender", StringComparison.InvariantCultureIgnoreCase))
            {
            }
            catch (JSDisconnectedException)
            {
            }
            catch (TaskCanceledException)
            {
            }
        }

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously and catches JSException, JSDisconnectedException and TaskCanceledException
        /// </summary>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation and resolves to true in case no exception has occured otherwise false.</returns>
        public static async ValueTask<bool> InvokeVoidAsyncWithErrorHandling(this IJSRuntime jsRuntime, string identifier, params object[] args)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync(identifier, args);
                return true;
            }
#if DEBUG
#else
            catch (JSException)
            {
                return false;
            }
#endif
            // catch prerending errors since there is no browser at this point.
            catch (InvalidOperationException ex) when (ex.Message.Contains("prerender", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            catch (JSDisconnectedException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously and catches JSException, JSDisconnectedException and TaskCanceledException
        /// </summary>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
        /// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
        /// </param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>A <see cref="ValueTask"/> that represents the asynchronous invocation operation and resolves to true in case no exception has occured otherwise false.</returns>
        public static async ValueTask<bool> InvokeVoidAsyncWithErrorHandling(this IJSRuntime jsRuntime, string identifier, CancellationToken cancellationToken, params object[] args)
        {
            try
            {
                await jsRuntime.InvokeVoidAsync(identifier, cancellationToken, args);
                return true;
            }
#if DEBUG
#else
            catch (JSException)
            {
                return false;
            }
#endif
            // catch prerending errors since there is no browser at this point.
            catch (InvalidOperationException ex) when (ex.Message.Contains("prerender", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }
            catch (JSDisconnectedException)
            {
                return false;
            }
            catch (TaskCanceledException)
            {
                return false;
            }
        }

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously and catches JSException, JSDisconnectedException and TaskCanceledException. In case an exception occured the default value of <typeparamref name="TValue"/> is returned
        /// </summary>
        /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-deserializing the return value into a tuple. The first item (sucess) is true in case where there was no exception, otherwise fall.</returns>
        public static async ValueTask<(bool success, TValue value)> InvokeAsyncWithErrorHandling<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(this IJSRuntime jsRuntime, string identifier, params object[] args)
            => await jsRuntime.InvokeAsyncWithErrorHandling(default(TValue), identifier, args);

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously and catches JSException, JSDisconnectedException and TaskCanceledException. In case an exception occured the default value of <typeparamref name="TValue"/> is returned
        /// </summary>
        /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
        /// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
        /// </param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-deserializing the return value into a tuple. The first item (sucess) is true in case where there was no exception, otherwise fall.</returns>
        public static async ValueTask<(bool success, TValue value)> InvokeAsyncWithErrorHandling<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(this IJSRuntime jsRuntime, string identifier, CancellationToken cancellationToken, params object[] args)
            => await jsRuntime.InvokeAsyncWithErrorHandling(default(TValue), identifier, cancellationToken, args);

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously and catches JSException, JSDisconnectedException and TaskCanceledException
        /// </summary>
        /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        /// <param name="fallbackValue">The value that should be returned in case an exception occured</param>
        /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-deserializing the return value into a tuple. The first item (sucess) is true in case where there was no exception, otherwise fall.</returns>
        public static async ValueTask<(bool success, TValue value)> InvokeAsyncWithErrorHandling<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(this IJSRuntime jsRuntime, TValue fallbackValue, string identifier, params object[] args)
        {
            try
            {
                var result = await jsRuntime.InvokeAsync<TValue>(identifier: identifier, args: args);
                return (true, result);
            }
#if DEBUG
#else
            catch (JSException)
            {
                return (false, fallbackValue);
            }
#endif
            // catch prerending errors since there is no browser at this point.
            catch (InvalidOperationException ex) when (ex.Message.Contains("prerender", StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, fallbackValue);
            }
            catch (JSDisconnectedException)
            {
                return (false, fallbackValue);
            }
            catch (TaskCanceledException)
            {
                return (false, fallbackValue);
            }
        }

        /// <summary>
        /// Invokes the specified JavaScript function asynchronously and catches JSException, JSDisconnectedException and TaskCanceledException
        /// </summary>
        /// <typeparam name="TValue">The JSON-serializable return type.</typeparam>
        /// <param name="jsRuntime">The <see cref="IJSRuntime"/>.</param>
        /// <param name="fallbackValue">The value that should be returned in case an exception occured</param>
        /// <param name="identifier">An identifier for the function to invoke. For example, the value <c>"someScope.someFunction"</c> will invoke the function <c>window.someScope.someFunction</c>.</param>
        /// <param name="cancellationToken">
        /// A cancellation token to signal the cancellation of the operation. Specifying this parameter will override any default cancellations such as due to timeouts
        /// (<see cref="JSRuntime.DefaultAsyncTimeout"/>) from being applied.
        /// </param>
        /// <param name="args">JSON-serializable arguments.</param>
        /// <returns>An instance of <typeparamref name="TValue"/> obtained by JSON-deserializing the return value into a tuple. The first item (sucess) is true in case where there was no exception, otherwise fall.</returns>
        public static async ValueTask<(bool success, TValue value)> InvokeAsyncWithErrorHandling<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(this IJSRuntime jsRuntime, TValue fallbackValue, string identifier, CancellationToken cancellationToken, params object[] args)
        {
            try
            {
                var result = await jsRuntime.InvokeAsync<TValue>(identifier: identifier, cancellationToken, args: args);
                return (true, result);
            }
#if DEBUG
#else
            catch (JSException)
            {
                return (false, fallbackValue);
            }
#endif
            // catch prerending errors since there is no browser at this point.
            catch (InvalidOperationException ex) when (ex.Message.Contains("prerender", StringComparison.InvariantCultureIgnoreCase))
            {
                return (false, fallbackValue);
            }
            catch (JSDisconnectedException)
            {
                return (false, fallbackValue);
            }
            catch (TaskCanceledException)
            {
                return (false, fallbackValue);
            }
        }
    }
}
