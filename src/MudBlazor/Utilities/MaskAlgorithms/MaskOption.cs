// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
namespace MudBlazor;

/// <summary>
/// A filter which determines when to use a mask for a <see cref="MultiMask"/>.
/// </summary>
/// <param name="Id">The unique name for this mask.</param>
/// <param name="Mask">The mask characters defining this mask.</param>
/// <param name="Regex">The regular expression which, when matched, causes this mask to be used.</param>
/// <remarks>
/// Example: to use this mask when an input starts with <c>4</c>, use an expression of <c>^4</c>.
/// </remarks>
public record struct MaskOption(string Id, string Mask, string? Regex);
