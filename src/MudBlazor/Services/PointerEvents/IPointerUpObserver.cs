// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Components.Web;

namespace MudBlazor;

#nullable enable

public interface IPointerUpObserver
{
    Task NotifyOnPointerUpAsync(EventArgs args) => Task.CompletedTask;
}
