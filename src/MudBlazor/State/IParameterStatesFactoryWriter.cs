// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a factory writer for parameter states.
/// </summary>
internal interface IParameterStatesFactoryWriter
{
    /// <summary>
    /// Writes the parameter states for the specified collection of parameters.
    /// </summary>
    /// <param name="parameters">The collection of parameters for which to write the states.</param>
    void WriteParameters(IEnumerable<IParameterComponentLifeCycle> parameters);

    /// <summary>
    /// Closes the factory writer.
    /// </summary>
    void Close();
}
