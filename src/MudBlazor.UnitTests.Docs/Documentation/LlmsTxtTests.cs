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

    private const string SiteHost = "mudblazor.com";

    // The file and the route table are read once. Every [TestCase] would otherwise re-read the file,
    // re-run the regex, and re-walk the directory tree looking for src.
    private static readonly Lazy<string> Content = new(ReadLlmsTxt);

    private static readonly Lazy<string[]> Lines = new(() =>
        Content.Value.Split('\n').Select(line => line.TrimEnd('\r')).ToArray());

    private static readonly Lazy<DocsRoutes> Routes = new(GetDocsRoutes);

    private static readonly Lazy<List<string>> LinkedSitePaths = new(GetLinkedSitePaths);

    /// <summary>
    /// The routes a docs page declares, split by whether the template takes a parameter.
    /// </summary>
    /// <param name="Exact">Templates with no placeholder, such as <c>/api</c>, matched literally.</param>
    /// <param name="ParameterPrefixes">
    /// The literal part of a parameterized template, such as <c>/api</c> from <c>/api/{TypeName}</c>.
    /// A link is only matched against these when it is <em>deeper</em> than the prefix, so
    /// <c>/api/mudbutton</c> resolves while a bare <c>/api</c> still needs its own exact route.
    /// </param>
    private sealed record DocsRoutes(HashSet<string> Exact, HashSet<string> ParameterPrefixes);

    private static string LlmsTxtPath
    {
        get
        {
            var srcDir = Paths.SrcDirPath;
            srcDir.Should().NotBeNullOrEmpty(
                "the tests locate llms.txt relative to the src directory, so they must run inside the repository");

            return Path.Combine(srcDir, "MudBlazor.Docs.Wasm", "wwwroot", "llms.txt");
        }
    }

    private static string ReadLlmsTxt()
    {
        var path = LlmsTxtPath;
        File.Exists(path).Should().BeTrue($"llms.txt is expected at {path}");

        return File.ReadAllText(path);
    }

    /// <summary>
    /// Reads the file's lines with fenced code blocks removed, so markdown inside a fence is not
    /// mistaken for document structure.
    /// </summary>
    /// <remarks>
    /// A fenced shell snippet such as <c># dotnet add package MudBlazor</c> is a comment, not a
    /// second H1.
    /// </remarks>
    private static IEnumerable<string> LinesOutsideCodeFences()
    {
        var inFence = false;

        foreach (var line in Lines.Value)
        {
            if (line.TrimStart().StartsWith("```", StringComparison.Ordinal))
            {
                inFence = !inFence;

                continue;
            }

            if (!inFence)
            {
                yield return line;
            }
        }
    }

    /// <summary>
    /// Collects every route a docs page declares, separating literal templates from parameterized ones.
    /// </summary>
    private static DocsRoutes GetDocsRoutes()
    {
        var exact = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var parameterPrefixes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var type in typeof(MenuService).Assembly.GetTypes().Where(typeof(IComponent).IsAssignableFrom))
        {
            foreach (RouteAttribute route in type.GetCustomAttributes(typeof(RouteAttribute), inherit: false))
            {
                var template = route.Template;
                var placeholder = template.IndexOf('{');
                if (placeholder < 0)
                {
                    exact.Add(Normalize(template));
                }
                else
                {
                    parameterPrefixes.Add(Normalize(template[..placeholder].TrimEnd('/')));
                }
            }
        }

        return new DocsRoutes(exact, parameterPrefixes);
    }

    /// <summary>
    /// Lowercases a route or link path and drops trailing slashes so both sides compare alike.
    /// </summary>
    private static string Normalize(string path)
    {
        var normalized = path.ToLowerInvariant();

        return normalized.Length > 1 ? normalized.TrimEnd('/') : normalized;
    }

    /// <summary>
    /// Determines whether a path equals a prefix or sits beneath it, respecting segment boundaries.
    /// </summary>
    /// <remarks>
    /// A bare <see cref="string.StartsWith(string, StringComparison)" /> would let <c>/apidocs</c>
    /// satisfy <c>/api</c>.
    /// </remarks>
    private static bool IsAtOrUnder(string path, string prefix)
    {
        return path.Equals(prefix, StringComparison.Ordinal)
               || path.StartsWith(prefix + "/", StringComparison.Ordinal);
    }

    /// <summary>
    /// Collects the site-relative paths of every mudblazor.com link in llms.txt.
    /// </summary>
    /// <remarks>
    /// Matching is done on the URI host rather than a string prefix, so <c>www.</c> and <c>http</c>
    /// variants are still validated and a lookalike domain such as <c>mudblazor.community</c> is not
    /// mistaken for the docs site. Other hosts under the domain, such as <c>try.mudblazor.com</c>,
    /// are separate apps and are deliberately not validated against docs routes.
    /// </remarks>
    private static List<string> GetLinkedSitePaths()
    {
        return LinkPattern.Matches(Content.Value)
            .Select(match => match.Groups[2].Value)
            .Where(url => Uri.TryCreate(url, UriKind.Absolute, out var uri)
                          && (uri.Host.Equals(SiteHost, StringComparison.OrdinalIgnoreCase)
                              || uri.Host.Equals("www." + SiteHost, StringComparison.OrdinalIgnoreCase)))
            .Select(url => Normalize(new Uri(url).AbsolutePath))
            .ToList();
    }

    /// <summary>
    /// Verifies llms.txt opens with a single H1 project name, as llmstxt.org requires.
    /// </summary>
    [Test]
    public void StartsWithSingleH1()
    {
        var lines = LinesOutsideCodeFences().ToArray();
        var firstContentLine = lines.FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));

        firstContentLine.Should().NotBeNull("llms.txt must not be empty");
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
        var lines = LinesOutsideCodeFences().ToArray();
        var headingIndex = Array.FindIndex(lines, line => line.StartsWith("# ", StringComparison.Ordinal));

        headingIndex.Should().BeGreaterThanOrEqualTo(0, "there is no H1 for a summary to follow");

        var summary = lines.Skip(headingIndex + 1).FirstOrDefault(line => !string.IsNullOrWhiteSpace(line));

        summary.Should().NotBeNull("llms.txt ends after its H1, so it carries no summary");
        summary.Should().StartWith("> ", "llmstxt.org requires a blockquote summary after the H1");
    }

    /// <summary>
    /// Verifies every markdown link has visible text and an absolute URL.
    /// </summary>
    [Test]
    public void LinksAreWellFormedAndAbsolute()
    {
        var matches = LinkPattern.Matches(Content.Value);

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
        var routes = Routes.Value;
        routes.Exact.Should().NotBeEmpty("the docs assembly declares routed pages");

        var deadLinks = LinkedSitePaths.Value
            .Where(path => !routes.Exact.Contains(path)
                           && !routes.ParameterPrefixes.Any(prefix =>
                               path.StartsWith(prefix + "/", StringComparison.Ordinal)))
            .ToList();

        deadLinks.Should().BeEmpty(
            "every llms.txt link must resolve; a renamed docs route fails here instead of rotting silently");
    }

    /// <summary>
    /// Verifies llms.txt links into every top-level docs area, so a curated file cannot silently drop one.
    /// </summary>
    /// <remarks>
    /// These are the five real areas <see cref="NavigationSection" /> models — every member except
    /// <see cref="NavigationSection.Unspecified" /> — plus <c>/getting-started</c> and <c>/mud</c>,
    /// which it does not model. The list is spelled out rather than derived from the enum: a guard
    /// that reads its expectations from the same source it validates would accept an area silently
    /// disappearing from both.
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
        LinkedSitePaths.Value.Should().Contain(path => IsAtOrUnder(path, prefix),
            $"llms.txt must point an agent at the {prefix} area of the docs");
    }
}
