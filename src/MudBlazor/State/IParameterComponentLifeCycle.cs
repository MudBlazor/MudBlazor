// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace MudBlazor.State;

#nullable enable
public interface IParameterComponentLifeCycle
{
    string ParameterName { get; }

    bool HasHandler { get; }

    bool HasParameterChanged(ParameterView parameters);

    Task ParameterChangeHandleAsync();

    void OnInitialized();

    void OnParametersSet();
}
