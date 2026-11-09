// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="SearchService"/> that exercise the algorithm
/// directly with synthetic data — no dependency on real component or docs data.
/// </summary>
[TestFixture]
public sealed class SearchServiceTests
{
    [TestCase("button", "button", 100)]             // exact match
    [TestCase("datepicker", "datepicker", 100)]     // exact match multi-word
    [TestCase("tooltip", "tooltip", 100)]           // exact match
    [TestCase("button", "butt", 94)]                // 4-char prefix on 6-char target
    [TestCase("button", "btn", 0)]                  // no match
    [TestCase("autocomplete", "auto", 76)]          // 4-char prefix on 12-char target
    [TestCase("button", "BUTTON", 100)]             // case ignored (caller lowercases)
    public void ComputeScore_ReturnsExpectedScore_ForKnownPairs(string target, string query, int expectedScore)
    {
        var actual = SearchService.ComputeScore(
            target.ToLowerInvariant().AsSpan(),
            query.ToLowerInvariant().AsSpan());

        actual.Should().Be(expectedScore);
    }

    [Test]
    public void ComputeScore_ReturnsZero_WhenTargetIsEmpty()
    {
        var score = SearchService.ComputeScore(ReadOnlySpan<char>.Empty, "button".AsSpan());

        score.Should().Be(0);
    }

    [Test]
    public void ComputeScore_ReturnsZero_WhenQueryIsEmpty()
    {
        var score = SearchService.ComputeScore("button".AsSpan(), ReadOnlySpan<char>.Empty);

        score.Should().Be(0);
    }

    [Test]
    public void ComputeScore_ReturnsSubstringScore_WhenQueryIsInteriorOfTarget()
    {
        // "button" is not a prefix of "iconbutton" but appears verbatim inside it,
        // so stage 2 (substring) must contribute at least 70.
        var score = SearchService.ComputeScore("iconbutton".AsSpan(), "button".AsSpan());

        score.Should().BeGreaterThanOrEqualTo(70);
    }

    [TestCase("snackbar", "snakbar")]       // missing 'c' (1 deletion)
    [TestCase("dialog", "dialoq")]          // q → g (1 substitution)
    [TestCase("tooltip", "tooltop")]        // i → o (1 substitution)
    [TestCase("slider", "slidr")]           // missing 'e' (1 deletion)
    [TestCase("stepper", "steppr")]         // missing 'e' (1 deletion)
    [TestCase("pagination", "paginaton")]   // missing 'i' (1 deletion)
    [TestCase("select", "selct")]           // missing 'e' (1 deletion)
    [TestCase("checkbox", "chckbox")]       // missing 'e' (1 deletion)
    [TestCase("breadcrumbs", "bredcrumbs")] // missing 'a' (1 deletion)
    [TestCase("rating", "rting")]           // missing 'a' (1 deletion)
    public void ComputeScore_ReturnsAboveMinScore_ForTypoedQuery(string target, string query)
    {
        var score = SearchService.ComputeScore(target.AsSpan(), query.AsSpan());

        score.Should().BeGreaterThanOrEqualTo(SearchService.MinScore);
    }

    [TestCase("color picker", "picker color")]          // reversed
    [TestCase("data grid", "grid data")]                // reversed
    [TestCase("expansion panels", "panel expansion")]   // reversed partial
    [TestCase("button group", "group button")]          // reversed
    [TestCase("date picker", "date pikr")]              // reversed + typo handled in forward direction
    public void ComputeScore_ReturnsAboveMinScore_ForReversedTokenOrder(string target, string query)
    {
        var score = SearchService.ComputeScore(target.AsSpan(), query.AsSpan());

        score.Should().BeGreaterThanOrEqualTo(SearchService.MinScore);
    }

    private static readonly ISearchService Service = new SearchService();

    private static readonly IReadOnlyList<KeyValuePair<string, string>> SyntheticIndex =
    [
        new("button", "Button"),
        new("button group", "ButtonGroup"),
        new("icon button", "IconButton"),
        new("dialog", "Dialog"),
        new("tooltip", "Tooltip"),
        new("color picker", "ColorPicker"),
        new("data grid", "DataGrid"),
        new("autocomplete", "Autocomplete"),
    ];

    [TestCase("button", "Button")]
    [TestCase("dialog", "Dialog")]
    [TestCase("tooltip", "Tooltip")]
    [TestCase("color picker", "ColorPicker")]
    [TestCase("data grid", "DataGrid")]
    [TestCase("autocomplete", "Autocomplete")]
    public void Search_ReturnsTopMatch_ForExactQuery(string query, string expected)
    {
        var results = Service.Search(SyntheticIndex, e => [e.Key], query);

        results.Should().NotBeEmpty();
        results.First().Value.Should().Be(expected);
    }

    [TestCase("dialoq")]    // typo
    [TestCase("tooltop")]   // typo
    [TestCase("autoc")]     // prefix
    public void Search_ReturnsMatch_ForTypoedOrPartialQuery(string query)
    {
        var results = Service.Search(SyntheticIndex, e => [e.Key], query);

        results.Should().NotBeEmpty();
    }

    [Test]
    public void Search_ReturnsEmpty_ForQueryTooDistantFromAnyKeyword()
    {
        // "btn" is three edits away from "button" so it should not match
        var results = Service.Search(SyntheticIndex, e => [e.Key], "btn");

        results.Should().BeEmpty();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("   ")]
    [TestCase("\t")]
    [TestCase("\n")]
    [TestCase("\r\n")]
    [TestCase("\0")]
    [TestCase("\0\0\0")]
    [TestCase("🎨")]
    [TestCase("🔘 🎨 🖼️")]
    [TestCase("中文")]
    [TestCase("العربية")]
    [TestCase("日本語")]
    [TestCase("한국어")]
    [TestCase("Ωμέγα")]
    [TestCase("zzzzzzzzzzzzzzz")]
    [TestCase("<script>alert('xss')</script>")]
    [TestCase("'; DROP TABLE components; --")]
    [TestCase("{}[]()!@#$%^&*")]
    [TestCase("123456789")]
    [TestCase("aaaaaaaaa")]
    [TestCase("\u202E reversed")]  // Right-to-Left Override
    [TestCase("café")]             // composed accent (no component named café)
    [TestCase("cafe\u0301")]       // decomposed accent (combining ´) — no component match
    public void Search_NeverThrowsAndReturnsEmpty_ForWeirdInput(string input)
    {
        var act = () => Service.Search(SyntheticIndex, e => [e.Key], input);

        act.Should().NotThrow();
        Service.Search(SyntheticIndex, e => [e.Key], input).Should().BeEmpty();
    }

    // Inputs that contain real words (like "button") prefixed by Unicode noise;
    // the service must not throw, regardless of whether a result is returned.
    [TestCase("\uFEFF button")]      // BOM prefix before "button"
    [TestCase("\u200B button")]      // zero-width space before "button"
    [TestCase("button\u0000dialog")] // embedded null between two words
    [TestCase("\u0000\uFFFF")]       // null + high-code-point character
    public void Search_NeverThrows_ForUnicodeNoisyInput(string input)
    {
        var act = () => Service.Search(SyntheticIndex, e => [e.Key], input);

        act.Should().NotThrow();
    }

    [Test]
    public void Search_NeverThrowsAndReturnsEmpty_ForOverlongInput()
    {
        var input = new string('a', 10_000);

        var act = () => Service.Search(SyntheticIndex, e => [e.Key], input);

        act.Should().NotThrow();
        Service.Search(SyntheticIndex, e => [e.Key], input).Should().BeEmpty();
    }

    [TestCase("BUTTON")]
    [TestCase("Button")]
    [TestCase("bUtToN")]
    [TestCase("  button  ")]  // padded spaces
    [TestCase("DIALOG")]
    [TestCase("Dialog")]
    [TestCase("TOOLTIP")]
    [TestCase("Tooltip")]
    public void Search_ReturnsResults_RegardlessOfCasing(string query)
    {
        var results = Service.Search(SyntheticIndex, e => [e.Key], query);

        results.Should().NotBeEmpty();
    }

    [Test]
    public void Search_OrdersExactMatchAboveFuzzyMatch()
    {
        // "dialog" is an exact match (100); "dialoq" is a one-edit typo (< 100).
        // The contract returns results ordered by relevance, so the exact match wins.
        var index = new[]
        {
            new KeyValuePair<string, string>("dialoq", "Fuzzy"),
            new KeyValuePair<string, string>("dialog", "Exact"),
        };

        var results = Service.Search(index, e => [e.Key], "dialog");

        results.Should().HaveCount(2);
        results[0].Value.Should().Be("Exact");
        results[1].Value.Should().Be("Fuzzy");
    }

}
