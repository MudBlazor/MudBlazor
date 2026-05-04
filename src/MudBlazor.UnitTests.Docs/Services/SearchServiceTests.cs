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

    [TestCase("dialog", "components/dialog")]
    [TestCase("badge", "components/badge")]
    [TestCase("avatar", "components/avatar")]
    [TestCase("rating", "components/rating")]
    [TestCase("slider", "components/slider")]
    [TestCase("tooltip", "components/tooltip")]
    [TestCase("carousel", "components/carousel")]
    [TestCase("stepper", "components/stepper")]
    [TestCase("drawer", "components/drawer")]
    [TestCase("card", "components/card")]
    [TestCase("tabs", "components/tabs")]
    [TestCase("alert", "components/alert")]
    [TestCase("breadcrumbs", "components/breadcrumbs")]
    [TestCase("timeline", "components/timeline")]
    [TestCase("skeleton", "components/skeleton")]
    [TestCase("collapse", "components/collapse")]
    [TestCase("image", "components/image")]
    [TestCase("divider", "components/divider")]
    [TestCase("overlay", "components/overlay")]
    [TestCase("paper", "components/paper")]
    public async Task Search_ReturnsTopResultForExactTitle(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }

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

    [TestCase(" ")]                                      // single space
    [TestCase("   ")]                                    // multiple spaces
    [TestCase("\t")]                                     // tab character
    [TestCase("\n")]                                     // newline
    [TestCase("\r\n")]                                   // Windows newline
    [TestCase("🎨")]                                     // emoji
    [TestCase("🔘 🎨 🖼️")]                              // multiple emojis
    [TestCase("中文")]                                   // Chinese characters
    [TestCase("العربية")]                               // Arabic script
    [TestCase("日本語")]                                 // Japanese
    [TestCase("한국어")]                                 // Korean
    [TestCase("Ωμέγα")]                                  // Greek
    [TestCase("zzzzzzzzzzzzzzz")]                        // long nonsense
    [TestCase("xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx")] // 99 x's
    [TestCase("<script>alert('xss')</script>")]          // XSS attempt
    [TestCase("'; DROP TABLE components; --")]           // SQL injection style
    [TestCase("{}[]()!@#$%^&*")]                         // punctuation barrage
    [TestCase("\0\0\0")]                                 // null characters
    [TestCase("123456789")]                              // digits only
    [TestCase("aaaaaaaaa")]                              // repeated letter (no match)
    [TestCase("qqqqqq")]                                 // another repeated letter
    public async Task Search_ReturnsNoResultsForIrrelevantOrWeirdInput(string search)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.Should().BeEmpty();
    }

    [TestCase("BUTTON", "components/button")]                  // all-caps
    [TestCase("Button", "components/button")]                  // title-case
    [TestCase("  button  ", "components/button")]              // leading/trailing spaces
    [TestCase("button!", "components/button")]                 // trailing punctuation
    [TestCase("button 🎨", "components/button")]               // emoji suffix
    [TestCase("bütton", "components/button")]                  // accented character (ü → u edit)
    [TestCase("DIALOG", "components/dialog")]                  // all-caps multi-char
    [TestCase("SeLeCt", "components/select")]                  // mixed case
    [TestCase("TOOLTIP", "components/tooltip")]                // all-caps
    [TestCase("  slider  ", "components/slider")]              // padded with spaces
    public async Task Search_ReturnsMatchDespiteNoisyInput(string search, string expectedLink)
    {
        var service = CreateApiLinkService();

        var results = await service.Search(search);

        results.First().Link.Should().Be(expectedLink);
    }

    [Test]
    public async Task Search_ReturnsNoResultsForEmptyString()
    {
        var service = CreateApiLinkService();

        var results = await service.Search(string.Empty);

        results.Should().BeEmpty();
    }

    [Test]
    public async Task Search_ReturnsNoResultsForNullString()
    {
        var service = CreateApiLinkService();

        var results = await service.Search((string)null);

        results.Should().BeEmpty();
    }

    [TestCase(null)]
    [TestCase("")]
    [TestCase(" ")]
    [TestCase("\0")]
    [TestCase("\u0000\uFFFF")]
    [TestCase("😀😁😂🤣😃😄😅😆😉😊")]
    [TestCase("\u202E reversed")]       // Right-to-Left Override
    [TestCase("\uFEFF button")]         // BOM prefix
    [TestCase("\u200B button")]         // zero-width space
    [TestCase("button\u0000dialog")]    // embedded null
    [TestCase("café")]                  // composed accent
    [TestCase("cafe\u0301")]            // decomposed accent (combining ´)
    public async Task Search_NeverThrowsForAnyInput(string input)
    {
        var service = CreateApiLinkService();

        var act = async () => await service.Search(input);

        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task Search_NeverThrowsForOverlongInput()
    {
        var service = CreateApiLinkService();

        var act = async () => await service.Search(new string('a', 10_000));

        await act.Should().NotThrowAsync();
    }
}
