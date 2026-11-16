using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Examples;
using MudBlazor.UnitTests.Docs.Mocks;
using MudBlazor.UnitTests.Shared;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Examples;

[TestFixture]
public class DataGridGroupingCustomizationExampleTests : BunitTest
{
    private const string LevelTwoBorder = "border-inline-start:2px solid var(--mud-palette-secondary)";

    public override void Setup()
    {
        base.Setup();
        Context.Services.AddScoped(_ => new HttpClient(new MockDocsMessageHandler()) { BaseAddress = new Uri("https://localhost/") });
    }

    /// <summary>
    /// Every group row gets the shared class and style, followed by the class and style for its level.
    /// </summary>
    [Test]
    public void GroupRows_ApplySharedAndPerLevelCustomization()
    {
        var comp = Context.Render<DataGridGroupingCustomizationExample>();

        comp.WaitForAssertion(() => GroupCells(comp).Should().HaveCount(4));
        var cells = GroupCells(comp);

        cells.Select(x => x.TextContent.Trim()).Should().Equal("Engineering (1)", "Team Platform", "Sales (1)", "Team SMB");
        cells.Should().AllSatisfy(x =>
        {
            x.ClassList.Should().Contain("py-1").And.Contain("mud-row-group-indented-0");
            x.GetAttribute("style").Should().StartWith("border-inline-start: 4px solid var(--mud-palette-primary);");
        });
        cells.Where((_, i) => i % 2 == 0).Should().AllSatisfy(x => ShouldBeLevel(x, 1));
        cells.Where((_, i) => i % 2 == 1).Should().AllSatisfy(x => ShouldBeLevel(x, 2));
    }

    /// <summary>
    /// The expand and collapse buttons follow IsGrouped through ungrouping and regrouping from the column menus.
    /// </summary>
    [Test]
    public async Task ExpandCollapseButtons_FollowIsGrouped_WhenUngroupedAndRegrouped()
    {
        var popoverProvider = Context.Render<MudPopoverProvider>();
        var comp = Context.Render<DataGridGroupingCustomizationExample>();
        comp.WaitForAssertion(() => GroupCells(comp).Should().HaveCount(4));
        ButtonsShouldBeEnabled(comp, true);

        await ClickColumnMenuItemAsync(comp, popoverProvider, "Department", "Ungroup");
        GroupCells(comp).Select(x => x.TextContent.Trim()).Should().Equal("Team Platform", "Team SMB");
        GroupCells(comp).Should().AllSatisfy(x => ShouldBeLevel(x, 1));
        ButtonsShouldBeEnabled(comp, true);

        await ClickColumnMenuItemAsync(comp, popoverProvider, "Team", "Ungroup");
        GroupCells(comp).Should().BeEmpty();
        comp.FindAll("tbody tr.mud-table-row").Should().HaveCount(2);
        ButtonsShouldBeEnabled(comp, false);

        await ClickColumnMenuItemAsync(comp, popoverProvider, "Department", "Group");
        GroupCells(comp).Select(x => x.TextContent.Trim()).Should().Equal("Engineering (1)", "Sales (1)");
        GroupCells(comp).Should().AllSatisfy(x => ShouldBeLevel(x, 1));
        ButtonsShouldBeEnabled(comp, true);

        await comp.FindAll("button").Single(x => x.TextContent.Trim() == "Collapse all").ClickAsync();
        comp.FindAll("tbody td").Should().OnlyContain(x => x.ClassList.Contains("mud-datagrid-group"));
    }

    private static IReadOnlyList<IElement> GroupCells(IRenderedComponent<DataGridGroupingCustomizationExample> comp)
        => comp.FindAll("td.mud-datagrid-group");

    private static void ShouldBeLevel(IElement cell, int level)
    {
        cell.ClassList.Contains("mud-background").Should().Be(level > 1);
        var style = cell.GetAttribute("style");
        style.Should().Contain($"padding-inline-start:{level * 24}px");
        if (level == 1)
        {
            style.Should().NotContain(LevelTwoBorder);
        }
        else
        {
            // The per-level border follows the shared one, so it wins for nested rows.
            style.IndexOf(LevelTwoBorder, StringComparison.Ordinal).Should().BeGreaterThan(style.IndexOf("4px solid", StringComparison.Ordinal));
        }
    }

    private static void ButtonsShouldBeEnabled(IRenderedComponent<DataGridGroupingCustomizationExample> comp, bool enabled)
    {
        foreach (var text in new[] { "Expand all", "Collapse all" })
        {
            comp.FindAll("button").Single(x => x.TextContent.Trim() == text).HasAttribute("disabled").Should().Be(!enabled, text);
        }
    }

    private static async Task ClickColumnMenuItemAsync(IRenderedComponent<DataGridGroupingCustomizationExample> comp,
        IRenderedComponent<MudPopoverProvider> popoverProvider, string column, string item)
    {
        var header = comp.FindAll("th").Single(x => x.TextContent.Contains(column));
        await header.QuerySelector("button[aria-label='Column options']")!.ClickAsync();
        await popoverProvider.FindAll(".mud-menu-item").Single(x => x.TextContent.Trim() == item).ClickAsync();
    }
}
