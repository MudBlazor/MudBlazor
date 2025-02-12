// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MudBlazor.Docs.WasmHost.Prerender;

public class PrerenderMiddleware
{
    private static LimitedConcurrentDictionary<string, byte[]> _responseCache = new(1_000);
    private readonly RequestDelegate _next;
    private readonly ICrawlerIdentifier _crawlerIdentifier;

    public PrerenderMiddleware(RequestDelegate next, ICrawlerIdentifier crawlerIdentifier)
    {
        _next = next;
        _crawlerIdentifier = crawlerIdentifier;
    }

    public async Task Invoke(HttpContext context)
    {
        var path = context.Request.Path.ToString().ToLower();
        if (path.Contains("webapi"))
        {
            await _next(context);
            return;
        }

        if (context.Request.Query.ContainsKey("force-prerender") == false && await _crawlerIdentifier.IsRequestByCrawler(context) == false)
        {
            await _next(context);
            return;
        }

        if (_responseCache.ContainsKey(path))
        {
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.BodyWriter.WriteAsync(_responseCache[path]);
            return;
        }

        context.Request.Headers.Append("UsePrerender", "true");

        var originalBodyStream = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        await _next(context);

        if (context.Response.StatusCode != 200)
        {
            return;
        }

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);

        responseBody.Seek(0, SeekOrigin.Begin);
        var cachedValue = responseBody.ToArray();

        _responseCache.TryAdd(path, cachedValue);
    }
}
