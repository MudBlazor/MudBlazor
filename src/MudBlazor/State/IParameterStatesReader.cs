// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace MudBlazor.State;

#nullable enable
/// <summary>
/// Represents a reader for parameter states.
/// </summary>
internal interface IParameterStatesReader
{
    /// <summary>
    /// Reads and returns a collection of parameter states.
    /// </summary>
    /// <returns>The collection of parameter states read from the reader.</returns>
    IEnumerable<IParameterComponentLifeCycle> ReadParameters();

    /// <summary>
    /// Completes the reading process.
    /// </summary>
    void Complete();
}
