// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using MudBlazor.Docs.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Services;

[TestFixture]
public sealed class SearchServiceTests
{
    private static IApiLinkService CreateApiLinkService() => new ApiLinkService(new MenuService(), new SearchService());

    [TestCase("data gri", "components/datagrid")]
    [TestCase("muddatagrid", "components/datagrid")]
    [TestCase("snakbar", "components/snackbar")]
    [TestCase("auto complte", "components/autocomplete")]
    [TestCase("date pikr", "components/datepicker")]
    [TestCase("expansion panls", "components/expansionpanels")]
    [TestCase("tree viw", "components/treeview")]
    [TestCase("toggl group", "components/togglegroup")]
    [TestCase("color pikr", "components/colorpicker")]
    [TestCase("breakpoint providr", "components/breakpointprovider")]
    public async Task Search_ReturnsTopResultForPartialOrMisspelledTitle(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }

    [TestCase("filter", "components/table")]
    [TestCase("filterble", "components/table")]
    [TestCase("templets", "getting-started/wireframes")]
    [TestCase("sortabl", "components/table")]
    [TestCase("resoluton", "components/grid")]
    [TestCase("brandng", "components/appbar")]
    [TestCase("tree-lik", "components/navmenu")]
    [TestCase("inspiraton", "getting-started/wireframes")]
    [TestCase("navigting", "components/button")]
    [TestCase("current app content", "components/dialog")]
    public async Task Search_ReturnsTopResultForPartialOrMisspelledSubtitle(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }

    [TestCase("table", "components/table")]
    [TestCase("pagination", "components/pagination")]
    [TestCase("butt", "components/button")]
    [TestCase("select", "components/select")]
    [TestCase("simple table", "components/simpletable")]
    [TestCase("button g", "components/buttongroup")]
    [TestCase("icon b", "components/iconbutton")]
    [TestCase("nav", "components/navmenu")]
    [TestCase("data g", "components/datagrid")]
    [TestCase("date", "components/datepicker")]
    public async Task Search_ReturnsTopResultForAmbiguousMatches(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }
}
