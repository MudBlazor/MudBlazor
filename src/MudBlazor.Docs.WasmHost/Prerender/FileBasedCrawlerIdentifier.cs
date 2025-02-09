// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MudBlazor.Docs.WasmHost.Prerender;

public class FileBasedCrawlerIdentifier : ICrawlerIdentifier
{
    private record CrawlerEntry(string Pattern, string Url, IEnumerable<string> Instances);

    private readonly string _filename;
    private readonly LimitedConcurrentDictionary<string, bool> _cache = new(1_000);

    private IEnumerable<Regex> _patterns = [];

    public FileBasedCrawlerIdentifier(string filename)
    {
        _filename = filename;
    }

    public async Task Initialize()
    {
        var content = await File.ReadAllTextAsync(_filename);

        var crawlers = JsonSerializer.Deserialize<IEnumerable<CrawlerEntry>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        _patterns = crawlers?.Select(x => new Regex(x.Pattern, RegexOptions.Compiled)).ToArray() ?? Enumerable.Empty<Regex>();
    }

    public Task<bool> IsRequestByCrawler(HttpContext context)
    {
        var userAgentHeader = context.Request.Headers.UserAgent;
        if (!userAgentHeader.Any())
        {
            return Task.FromResult(false);
        }

        var value = userAgentHeader.FirstOrDefault(string.Empty)!;
        if (_cache.ContainsKey(value))
        {
            return Task.FromResult(_cache[value]);
        }

        foreach (var item in _patterns)
        {
            if (item.IsMatch(value))
            {
                _cache.TryAdd(value, true);

                return Task.FromResult(true);
            }
        }

        _cache.TryAdd(value, false);

        return Task.FromResult(false);
    }
}
