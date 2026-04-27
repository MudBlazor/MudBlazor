#nullable enable

using System.Linq;
using AwesomeAssertions;
using MudBlazor.UnitTests.Shared.Search;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

[TestFixture]
public sealed class FuzzySearchServiceTests
{
    private readonly IFuzzySearchService _service = new FuzzySearchService();

    [Test]
    public async Task Search_IgnoresCaseAcrossMultipleDataPoints()
    {
        FuzzySearchEntry<SearchItem>[] entries =
        [
            new FuzzySearchEntry<SearchItem>(
                new SearchItem("Component Playground", "Utilities"),
                "Component Playground",
                [
                    new FuzzySearchDataPoint("Component Playground"),
                    new FuzzySearchDataPoint("Utilities", 0.60)
                ]),
            new FuzzySearchEntry<SearchItem>(
                new SearchItem("Dialog Playground", "Overlays"),
                "Dialog Playground",
                [
                    new FuzzySearchDataPoint("Dialog Playground"),
                    new FuzzySearchDataPoint("Overlays", 0.60)
                ])
        ];

        var result = await _service.SearchAsync("uTiLiTiEs", entries);

        result.TotalMatchCount.Should().Be(1);
        result.Results.Select(static x => x.Name).Should().ContainSingle().Which.Should().Be("Component Playground");
    }

    [Test]
    public async Task Search_RespectsLimitsWhileKeepingTotalMatchCount()
    {
        FuzzySearchEntry<SearchItem>[] entries =
        [
            new FuzzySearchEntry<SearchItem>(new SearchItem("Table", "Data"), "Table", [new FuzzySearchDataPoint("Table")]),
            new FuzzySearchEntry<SearchItem>(new SearchItem("Table Column", "Data"), "Table Column", [new FuzzySearchDataPoint("Table Column")]),
            new FuzzySearchEntry<SearchItem>(new SearchItem("Table Pager", "Data"), "Table Pager", [new FuzzySearchDataPoint("Table Pager")])
        ];

        var result = await _service.SearchAsync("table", entries, new FuzzySearchOptions { Limit = 2 });

        result.TotalMatchCount.Should().Be(3);
        result.Results.Should().HaveCount(2);
        result.Results.Select(static x => x.Name).Should().ContainInOrder("Table", "Table Column");
    }

    private sealed record SearchItem(string Name, string Category);
}
