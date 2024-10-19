// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// Factory interface for creating instances of <see cref="IScrollSpy"/>.
/// </summary>
public interface IScrollSpyFactory
{
    /// <summary>
    /// Creates a new instance of <see cref="IScrollSpy"/>.
    /// </summary>
    /// <returns>A new instance of <see cref="IScrollSpy"/>.</returns>
    IScrollSpy Create();
}
