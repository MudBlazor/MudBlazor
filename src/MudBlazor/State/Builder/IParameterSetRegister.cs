// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MudBlazor.State.Builder;

#nullable enable
/// <summary>
/// Represents a mechanism for registering parameter states to <seealso cref="ParameterSet"/>.
/// </summary>
internal interface IParameterSetRegister
{
    /// <summary>
    /// Adds a parameter state to the register.
    /// </summary>
    /// <typeparam name="T">The type of the parameter state.</typeparam>
    /// <param name="parameterState">The parameter state to add.</param>
    void Add<T>(ParameterStateInternal<T> parameterState);

    /// <summary>
    /// Adds a smart attachable to the register.
    /// </summary>
    /// <param name="smartAttachable">The smart attachable to add.</param>
    /// <remarks>
    /// This method is used to keep track of all preattachments. 
    /// In the future, all attachables can be attached using <see cref="ISmartAttachable.Attach"/> 
    /// if they were not already attached.
    /// </remarks>
    void Add(ISmartAttachable smartAttachable);
}
