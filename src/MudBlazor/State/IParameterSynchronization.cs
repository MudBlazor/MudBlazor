// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;

namespace MudBlazor.State;

internal interface IParameterSynchronization
{
    void OnInitialized();

    Task OnParametersSetAsync();
}
