// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Threading.Tasks;
using AwesomeAssertions;
using MudBlazor.Docs.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Services;

/// <summary>
/// Integration tests for <see cref="ApiLinkService"/> that verify the search
/// pipeline against the real documentation component registry.
/// </summary>
[TestFixture]
public sealed class ApiLinkServiceTests
{
    private static IApiLinkService CreateApiLinkService() => new ApiLinkService(new MenuService());

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

    [TestCase("table", "components/table")]                     // Table vs SimpleTable vs DataGrid
    [TestCase("pagination", "components/pagination")]           // Pagination vs AppBar (has prev/next)
    [TestCase("butt", "components/button")]                     // Button vs ButtonGroup vs IconButton
    [TestCase("select", "components/select")]                   // Select vs Autocomplete
    [TestCase("simple table", "components/simpletable")]        // SimpleTable vs Table
    [TestCase("button g", "components/buttongroup")]            // ButtonGroup vs Button
    [TestCase("icon b", "components/iconbutton")]               // IconButton vs Icons
    [TestCase("nav", "components/navmenu")]                     // NavMenu vs NavLink vs NavGroup
    [TestCase("data g", "components/datagrid")]                 // DataGrid vs Grid
    [TestCase("date", "components/datepicker")]                 // DatePicker vs DateRangePicker
    [TestCase("color p", "components/colorpicker")]             // ColorPicker vs Color (features)
    [TestCase("time p", "components/timepicker")]               // TimePicker vs Timeline vs TimeSeries
    [TestCase("autoc", "components/autocomplete")]              // Autocomplete vs Select
    [TestCase("checkb", "components/checkbox")]                 // Checkbox vs Check...
    [TestCase("snack", "components/snackbar")]                  // Snackbar vs Alert
    [TestCase("button", "components/button")]                   // Button vs ButtonGroup vs IconButton vs FAB
    [TestCase("icon", "components/icons")]                      // Icons vs Icon Button vs Toggle Icon Button
    [TestCase("toggle", "components/togglegroup")]              // Toggle Group vs Toggle Icon Button
    [TestCase("chip", "components/chips")]                      // Chips vs Chip Set
    [TestCase("date range", "components/daterangepicker")]      // DateRangePicker vs DatePicker
    [TestCase("bar chart", "components/barchart")]              // BarChart vs StackedBarChart
    [TestCase("grid", "components/grid")]                       // Grid (layout) vs DataGrid
    public async Task Search_ReturnsTopResultForAmbiguousMatches(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }

    [TestCase("BUTTON", "components/button")]           // all-caps
    [TestCase("Button", "components/button")]           // title-case
    [TestCase("  button  ", "components/button")]       // leading/trailing spaces
    [TestCase("button!", "components/button")]          // trailing punctuation
    [TestCase("button 🎨", "components/button")]        // emoji suffix
    [TestCase("bütton", "components/button")]           // accented character (ü → u edit)
    [TestCase("DIALOG", "components/dialog")]           // all-caps multi-char
    [TestCase("SeLeCt", "components/select")]           // mixed case
    [TestCase("TOOLTIP", "components/tooltip")]         // all-caps
    [TestCase("  slider  ", "components/slider")]       // padded with spaces
    public async Task Search_ReturnsMatchDespiteNoisyInput(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }

    [Test]
    public async Task Search_ReturnsGroupMembersMatchedByGroupSubtitle()
    {
        var service = CreateApiLinkService();

        // "flex" is a clear partial match for the "Flexbox" group subtitle.
        // Items like Order, Gap, and Align Content don't mention "flex" in
        // their own titles, but they all belong to the Flexbox group and should
        // therefore appear in search results.
        var results = await service.Search("flex");
        var links = results.Select(r => r.Link).ToList();

        links.Should().Contain("utilities/order");
        links.Should().Contain("utilities/gap");
        links.Should().Contain("utilities/align-content");
        links.Should().Contain("utilities/align-items");
        links.Should().Contain("utilities/align-self");
        links.Should().Contain("utilities/justify-content");
    }

    [Test]
    public async Task Search_ReturnsGroupMembersMatchedByGroupSubtitleTypo()
    {
        var service = CreateApiLinkService();

        // A small typo in the group name ("flexx" vs "flexbox") — the extra x
        // still has a reasonable edit distance and the prefix "flex" should still
        // surface the Flexbox group members.
        var results = await service.Search("flexx");
        var links = results.Select(r => r.Link).ToList();

        links.Should().Contain("utilities/order");
        links.Should().Contain("utilities/gap");
    }

    [Test]
    public async Task Search_ReturnsGroupMembersMatchedByFullGroupSubtitle()
    {
        var service = CreateApiLinkService();

        // Typing the full group name "flexbox" should return all members.
        var results = await service.Search("flexbox");
        var links = results.Select(r => r.Link).ToList();

        links.Should().Contain("utilities/order");
        links.Should().Contain("utilities/gap");
        links.Should().Contain("utilities/align-content");
        links.Should().Contain("utilities/align-items");
        links.Should().Contain("utilities/align-self");
        links.Should().Contain("utilities/justify-content");
        links.Should().Contain("utilities/enable-flex");
        links.Should().Contain("utilities/flex-direction");
        links.Should().Contain("utilities/flex-wrap");
    }
}
