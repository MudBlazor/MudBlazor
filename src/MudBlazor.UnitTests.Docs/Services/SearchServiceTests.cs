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
    private static IApiLinkService CreateApiLinkService() => new ApiLinkService(new MenuService());

    // ──────────────────────────────────────────────────────────────────────────
    // Title – typos, partials, and out-of-order words
    // ──────────────────────────────────────────────────────────────────────────
    [TestCase("data gri", "components/datagrid")]           // partial two-word
    [TestCase("muddatagrid", "components/datagrid")]        // component-name prefix
    [TestCase("snakbar", "components/snackbar")]            // missing 'c'
    [TestCase("auto complte", "components/autocomplete")]   // two-word with typo
    [TestCase("date pikr", "components/datepicker")]        // two-word with typo
    [TestCase("expansion panls", "components/expansionpanels")] // two-word with typo
    [TestCase("tree viw", "components/treeview")]           // two-word with typo
    [TestCase("toggl group", "components/togglegroup")]     // two-word with typo
    [TestCase("color pikr", "components/colorpicker")]      // two-word with typo
    [TestCase("breakpoint providr", "components/breakpointprovider")] // two-word with typo
    [TestCase("paginaton", "components/pagination")]        // missing 'i'
    [TestCase("selct", "components/select")]                // missing 'e'
    [TestCase("dialoq", "components/dialog")]               // q → g substitution
    [TestCase("rting", "components/rating")]                // missing 'a'
    [TestCase("swich", "components/switch")]                // missing 't'
    [TestCase("chckbox", "components/checkbox")]            // missing 'e'
    [TestCase("tooltop", "components/tooltip")]             // i → o substitution
    [TestCase("slidr", "components/slider")]                // missing 'e'
    [TestCase("steppr", "components/stepper")]              // missing 'e'
    [TestCase("bredcrumbs", "components/breadcrumbs")]      // missing 'a'
    [TestCase("picker color", "components/colorpicker")]    // reversed word order
    [TestCase("grid data", "components/datagrid")]          // reversed word order
    [TestCase("panel expansion", "components/expansionpanels")] // reversed word order
    [TestCase("group button", "components/buttongroup")]    // reversed word order
    public async Task Search_ReturnsTopResultForPartialOrMisspelledTitle(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Subtitle – words or phrases found in a component's description
    // ──────────────────────────────────────────────────────────────────────────
    [TestCase("filter", "components/table")]                // partial word in subtitle
    [TestCase("filterble", "components/table")]             // typo in subtitle word
    [TestCase("templets", "getting-started/wireframes")]    // typo in subtitle word
    [TestCase("sortabl", "components/table")]               // typo in subtitle word
    [TestCase("resoluton", "components/grid")]              // typo in subtitle word
    [TestCase("brandng", "components/appbar")]              // typo in subtitle word
    [TestCase("tree-lik", "components/navmenu")]            // partial hyphenated phrase
    [TestCase("inspiraton", "getting-started/wireframes")]  // typo in subtitle word
    [TestCase("navigting", "components/button")]            // typo in subtitle word
    [TestCase("current app content", "components/dialog")]  // exact phrase in subtitle
    [TestCase("sortable", "components/table")]              // exact word in subtitle
    [TestCase("filterable", "components/table")]            // exact word in subtitle
    [TestCase("multiselection", "components/table")]        // exact word in subtitle
    [TestCase("screen sizes", "components/grid")]           // phrase from subtitle
    [TestCase("display actions", "components/appbar")]      // phrase from subtitle
    [TestCase("screen titles", "components/appbar")]        // phrase from subtitle
    [TestCase("trigger action", "components/button")]       // phrase from subtitle
    [TestCase("overlay content", "components/dialog")]      // phrase from subtitle
    [TestCase("tree like menu", "components/navmenu")]      // phrase from subtitle
    [TestCase("navigation screen", "components/appbar")]    // phrase from subtitle
    public async Task Search_ReturnsTopResultForPartialOrMisspelledSubtitle(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Ambiguous – short prefixes or single words that compete with similar names
    // ──────────────────────────────────────────────────────────────────────────
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
    [TestCase("color p", "components/colorpicker")]
    [TestCase("time p", "components/timepicker")]
    [TestCase("autoc", "components/autocomplete")]
    [TestCase("checkb", "components/checkbox")]
    [TestCase("snack", "components/snackbar")]
    [TestCase("dialog", "components/dialog")]
    [TestCase("badge", "components/badge")]
    [TestCase("avatar", "components/avatar")]
    [TestCase("rating", "components/rating")]
    [TestCase("slider", "components/slider")]
    [TestCase("tooltip", "components/tooltip")]
    [TestCase("carousel", "components/carousel")]
    public async Task Search_ReturnsTopResultForAmbiguousMatches(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }
}
