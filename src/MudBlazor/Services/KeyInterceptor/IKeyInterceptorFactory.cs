// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.Services;

/// <summary>
/// Represents a factory for creating instances of <see cref="IKeyInterceptor"/>.
/// </summary>
public interface IKeyInterceptorFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IKeyInterceptor"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IKeyInterceptor"/>.</returns>
    public IKeyInterceptor Create();
}
