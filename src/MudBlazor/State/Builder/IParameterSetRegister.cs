// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

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
    /// <param name="parameterStates">The parameter states to add.</param>
    void AddParameterStates(IReadOnlyCollection<IParameterComponentLifeCycle> parameterStates);
}
