// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Represents an interface for <see cref="ParameterState{T}"/> that can be smartly attached.
/// </summary>
internal interface ISmartParameterAttachable
{
    /// <summary>
    /// Gets a value indicating whether the <see cref="ParameterState{T}"/> is attached.
    /// </summary>
    bool IsAttached { get; }

    /// <summary>
    /// Attaches the <see cref="ParameterState{T}"/>.
    /// </summary>
    void Attach();
}
