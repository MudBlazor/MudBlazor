// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.AspNetCore.Builder;

namespace MudBlazor.Docs.WasmHost.Prerender;

public static class PrerenderMiddlewareExtensions
{
    public static IApplicationBuilder UsePrerenderMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PrerenderMiddleware>();
    }
}
