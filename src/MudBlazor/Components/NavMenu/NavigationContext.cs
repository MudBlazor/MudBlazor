// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable

/// <summary>
/// The state of a navigation component based on the state of its parent.
/// </summary>
/// <param name="Disabled">The parent is preventing user interaction.</param>
/// <param name="Expanded">The parent is expanded.</param>
public record NavigationContext(bool Disabled, bool Expanded)
{
    /// <summary>
    /// The unique identifier for this context.
    /// </summary>
    public string MenuId { get; } = Identifier.Create();
}
