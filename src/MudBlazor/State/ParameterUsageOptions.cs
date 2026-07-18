// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State;

/// <summary>
/// Specifies whether a registered component parameter takes part in reading values, writing values, or both.
/// </summary>
[Flags]
public enum ParameterUsageOptions
{
    None = 0,
    Read = 1 << 1,
    Write = 1 << 2,
    All = Read | Write
}
