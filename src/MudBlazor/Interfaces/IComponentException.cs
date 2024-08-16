// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components;

namespace MudBlazor.Interfaces;

/// <summary>
/// Represents a component that can handle exceptions and dispatch them to the renderer.
/// </summary>
public interface IComponentException : IComponent
{
    /// <summary>
    /// Treats the supplied <paramref name="exception"/> as being thrown by this component. This will cause the
    /// enclosing ErrorBoundary to transition into a failed state. If there is no enclosing ErrorBoundary,
    /// it will be regarded as an exception from the enclosing renderer.
    ///
    /// This is useful if an exception occurs outside the component lifecycle methods, but you wish to treat it
    /// the same as an exception from a component lifecycle method.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> that will be dispatched to the renderer.</param>
    /// <returns>A <see cref="Task"/> that will be completed when the exception has finished dispatching.</returns>
    Task DispatchExceptionAsync(Exception exception);
}
