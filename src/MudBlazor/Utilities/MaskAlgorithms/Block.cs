// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor;

#nullable enable
/// <summary>
/// A set of contiguous characters used to build a <see cref="BlockMask"/>.
/// </summary>
/// <remarks>
/// Example: a mask character of <c>a</c>, <c>Min</c> of <c>2</c>, and <c>Max</c> of <c>3</c>, would allow <c>ABC</c> as a valid value.<br />
/// Example: a mask character of <c>0</c>, <c>Min</c> of <c>5</c>, and <c>Max</c> of <c>7</c>, would allow <c>09123</c> as a valid value.<br />
/// Example: a mask character of <c>*</c>, <c>Min</c> of <c>1</c>, and <c>Max</c> of <c>4</c>, would allow <c>B2A7</c> as a valid value.<br />
/// </remarks>
/// <param name="MaskChar">The mask character.</param>
/// <param name="Min">The minimum required number of characters.</param>
/// <param name="Max">The maximum allowed number of characters.</param>
public record struct Block(char MaskChar, int Min = 1, int Max = 1);
