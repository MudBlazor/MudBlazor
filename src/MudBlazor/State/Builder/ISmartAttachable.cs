// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Represents an interface for objects that can be smartly attached.
/// </summary>
internal interface ISmartAttachable
{
    /// <summary>
    /// Gets a value indicating whether the object is attached.
    /// </summary>
    bool IsAttached { get; }

    /// <summary>
    /// Attaches the object.
    /// </summary>
    void Attach();
}
