// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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

    private string[] _literals = [];
    private Regex[] _patterns = [];

    public FileBasedCrawlerIdentifier(string filename)
    {
        _filename = filename;
    }

    public async Task Initialize()
    {
        var content = await File.ReadAllTextAsync(_filename);

        var crawlers = JsonSerializer.Deserialize<IEnumerable<CrawlerEntry>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var entries = crawlers?.Select(crawler => crawler.Pattern).ToArray() ?? [];

        _literals = entries.Where(IsLiteral).ToArray();
        _patterns = entries.Where(pattern => !IsLiteral(pattern)).Select(pattern => new Regex(pattern)).ToArray();
    }

    /// <summary>
    /// Pattern contains no regular expression syntax, so a substring search answers it.
    /// </summary>
    private static bool IsLiteral(string pattern) => Regex.Escape(pattern) == pattern;

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

        var isCrawler = IsCrawler(value);
        _cache.TryAdd(value, isCrawler);

        return Task.FromResult(isCrawler);
    }

    private bool IsCrawler(string userAgent)
    {
        foreach (var literal in _literals)
        {
            if (userAgent.Contains(literal, StringComparison.Ordinal))
            {
                return true;
            }
        }

        foreach (var pattern in _patterns)
        {
            if (pattern.IsMatch(userAgent))
            {
                return true;
            }
        }

        return false;
    }
}
