#nullable enable

using System.Linq;
using AwesomeAssertions;
using MudBlazor.Docs.Services;
using MudBlazor.UnitTests.Shared.Search;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Services;

[TestFixture]
public sealed class ApiLinkServiceTests
{
    private static ApiLinkService CreateService() => new(new MenuService(), new FuzzySearchService());

    [Test]
    public async Task Search_FindsAliasesIgnoringCase()
    {
        var service = CreateService();

        var results = await service.Search("typeAHEAD");

        results.Should().NotBeEmpty();
        results.First().Link.Should().Be("components/autocomplete");
    }

    [Test]
    public async Task Search_FindsEntriesFromSecondarySearchData()
    {
        var service = CreateService();

        var results = await service.Search("sortable filterable table");

        results.Should().NotBeEmpty();
        results.First().Title.Should().Be("Table");
    }
}
