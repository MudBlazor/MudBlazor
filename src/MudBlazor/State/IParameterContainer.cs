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
    void OnInitialized();

    void OnParametersSet();

    Task SetParametersAsync(Func<ParameterView, Task> baseSetParametersAsync, ParameterView parameters);

    bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out IParameterComponentLifeCycle parameterComponentLifeCycle);
}
