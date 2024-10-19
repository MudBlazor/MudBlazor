// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Factory interface for creating instances of <see cref="IEventListener"/>.
/// </summary>
public interface IEventListenerFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IEventListener"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IEventListener"/>.</returns>
    IEventListener Create();
}
