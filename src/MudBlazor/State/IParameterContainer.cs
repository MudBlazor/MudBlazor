// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

internal interface IParameterContainer : IEnumerable<IParameterComponentLifeCycle>
{
    /// <summary>
    /// Executes <see cref="ComponentBase.OnInitialized"/>.
    /// </summary>
    void OnInitialized();

    /// <summary>
    /// Executes <see cref="ComponentBase.OnParametersSet"/>.
    /// </summary>
    void OnParametersSet();

    /// <summary>
    /// Executes <see cref="ComponentBase.SetParametersAsync"/>.
    /// </summary>
    Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters);

    /// <summary>Searches the set for a given value and returns the equal value it finds, if any.</summary>
    /// <param name="parameterName">The value to search for.</param>
    /// <param name="parameterComponentLifeCycle">The value from the set that the search found, or the default value when the search yielded no match.</param>
    /// <returns>A value indicating whether the search was successful.</returns>
    bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle);
}
