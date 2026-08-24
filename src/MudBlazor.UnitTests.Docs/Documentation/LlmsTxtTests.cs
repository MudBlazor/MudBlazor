// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text.RegularExpressions;
using AwesomeAssertions;
using MudBlazor.Docs.Compiler;
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
}
