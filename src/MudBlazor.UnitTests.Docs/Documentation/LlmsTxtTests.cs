// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Compiler;
using MudBlazor.Docs.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Documentation;

/// <summary>
/// Guards the curated llms.txt served at the site root.
/// </summary>
/// <remarks>
/// The file is hand-written on purpose: llmstxt.org asks for a short, curated index rather than an
/// exhaustive dump. These tests supply what curation cannot, which is a check that the links still
/// point at pages the docs site serves. The stale sitemap.xml sitting beside it is what happens
/// without them.
/// </remarks>
[TestFixture]
public class LlmsTxtTests
{
    private static readonly Regex LinkPattern = new(@"\[([^\]]+)\]\(([^)\s]+)\)", RegexOptions.Compiled);

    private const string SiteRoot = "https://mudblazor.com";

    private static string LlmsTxtPath => Path.Combine(Paths.SrcDirPath, "MudBlazor.Docs.Wasm", "wwwroot", "llms.txt");

    private static string ReadLlmsTxt()
    {
        File.Exists(LlmsTxtPath).Should().BeTrue($"llms.txt is expected at {LlmsTxtPath}");

        return File.ReadAllText(LlmsTxtPath);
    }

    private static string[] ReadLlmsTxtLines()
    {
        return ReadLlmsTxt().Split('\n').Select(line => line.TrimEnd('\r')).ToArray();
    }

    /// <summary>
    /// Collects every route literal a docs page declares, truncating parameterized templates at the first placeholder.
    /// </summary>
    /// <remarks>
    /// <c>/api/{TypeName}</c> cannot be matched literally, so it contributes <c>/api</c> and any link
    /// beneath it resolves against that prefix.
    /// </remarks>
    private static HashSet<string> GetDocsRouteTemplates()
    {
        var routes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in typeof(MenuService).Assembly.GetTypes().Where(typeof(IComponent).IsAssignableFrom))
        {
            foreach (RouteAttribute route in type.GetCustomAttributes(typeof(RouteAttribute), inherit: false))
            {
                var template = route.Template;
                var placeholder = template.IndexOf('{');
                if (placeholder >= 0)
                {
                    template = template[..placeholder].TrimEnd('/');
                }

                routes.Add(Normalize(template));
            }
        }

        return routes;
    }

    /// <summary>
    /// Lowercases a route or link path and drops a single trailing slash so both sides compare alike.
    /// </summary>
    private static string Normalize(string path)
    {
        var normalized = path.ToLowerInvariant();

        return normalized.Length > 1 ? normalized.TrimEnd('/') : normalized;
    }

    /// <summary>
    /// Collects the site-relative paths of every mudblazor.com link in llms.txt.
    /// </summary>
    private static List<string> GetLinkedSitePaths()
    {
        return LinkPattern.Matches(ReadLlmsTxt())
            .Select(match => match.Groups[2].Value)
            .Where(url => url.StartsWith(SiteRoot, StringComparison.OrdinalIgnoreCase))
            .Select(url => Normalize(new Uri(url).AbsolutePath))
            .ToList();
    }

    /// <summary>
    /// Verifies llms.txt opens with a single H1 project name, as llmstxt.org requires.
    /// </summary>
    [Test]
    public void StartsWithSingleH1()
    {
        var lines = ReadLlmsTxtLines();
        var firstContentLine = lines.First(line => !string.IsNullOrWhiteSpace(line));

        firstContentLine.Should().StartWith("# ", "llmstxt.org requires an H1 project name first");
        lines.Count(line => line.StartsWith("# ", StringComparison.Ordinal))
            .Should().Be(1, "llms.txt has exactly one H1");
    }

    /// <summary>
    /// Verifies a blockquote summary follows the H1, so an agent gets context before the links.
    /// </summary>
    [Test]
    public void SummaryBlockquoteFollowsH1()
    {
        var lines = ReadLlmsTxtLines();
        var headingIndex = Array.FindIndex(lines, line => line.StartsWith("# ", StringComparison.Ordinal));
        var summary = lines.Skip(headingIndex + 1).First(line => !string.IsNullOrWhiteSpace(line));

        summary.Should().StartWith("> ", "llmstxt.org requires a blockquote summary after the H1");
    }

    /// <summary>
    /// Verifies every markdown link has visible text and an absolute URL.
    /// </summary>
    [Test]
    public void LinksAreWellFormedAndAbsolute()
    {
        var matches = LinkPattern.Matches(ReadLlmsTxt());

        matches.Should().NotBeEmpty("llms.txt exists to carry links");

        foreach (Match match in matches)
        {
            match.Groups[1].Value.Trim().Should().NotBeEmpty("every link needs visible text");
            match.Groups[2].Value.Should().MatchRegex("^https?://",
                "llms.txt is fetched standalone, so relative links cannot be resolved");
        }
    }

    /// <summary>
    /// Verifies every mudblazor.com link in llms.txt points at a route the docs site actually serves.
    /// </summary>
    [Test]
    public void SiteLinksResolveToDocsRoutes()
    {
        var routes = GetDocsRouteTemplates();
        routes.Should().NotBeEmpty("the docs assembly declares routed pages");

        var deadLinks = GetLinkedSitePaths().Where(path => !routes.Contains(path)).ToList();

        deadLinks.Should().BeEmpty(
            "every llms.txt link must resolve; a renamed docs route fails here instead of rotting silently");
    }

    /// <summary>
    /// Verifies llms.txt links into every top-level docs area, so a curated file cannot silently drop one.
    /// </summary>
    /// <remarks>
    /// These are the six areas <see cref="NavigationSection" /> models plus the two route prefixes it
    /// does not, <c>/getting-started</c> and <c>/mud</c>.
    /// </remarks>
    [TestCase("/getting-started")]
    [TestCase("/components")]
    [TestCase("/api")]
    [TestCase("/features")]
    [TestCase("/customization")]
    [TestCase("/utilities")]
    [TestCase("/mud")]
    public void CoversDocsArea(string prefix)
    {
        GetLinkedSitePaths().Should().Contain(path => path.StartsWith(prefix, StringComparison.Ordinal),
            $"llms.txt must point an agent at the {prefix} area of the docs");
    }
}
