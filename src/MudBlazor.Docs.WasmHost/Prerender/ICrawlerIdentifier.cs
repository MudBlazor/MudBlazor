// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MudBlazor.Docs.WasmHost.Prerender;

public interface ICrawlerIdentifier
{
    Task Initialize();

    Task<bool> IsRequestByCrawler(HttpContext context);
}
