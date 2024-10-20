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
    /// <remarks>
    /// If you are creating this <see cref="IEventListener"/> instance yourself using this factory, then you need to manually call <see cref="EventListener.DisposeAsync"/>; otherwise, you will get a memory leak.
    /// </remarks>
    /// <returns>A new instance of <see cref="IEventListener"/>.</returns>
    IEventListener Create();
}
