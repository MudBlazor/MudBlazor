using AngleSharp.Dom;
using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using MudBlazor.UnitTests.TestComponents.Virtualize;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

#nullable enable
[TestFixture]
public class VirtualizeTests : BunitTest
{
    [Test]
    public void VirtualizeRender()
    {
        var comp = Context.Render<VirtualizeTest>();
        var virtualize = comp.FindComponent<MudVirtualize<string>>();
        virtualize.Instance.ChildContent.Should().NotBeNull();
        comp.FindComponents<MudText>().Count.Should().Be(1);
    }

    [Test]
    public void VirtualizeNoRecord()
    {
        var comp = Context.Render<VirtualizeNoRecordsContentTest>();

        IElement ItemNoData() => comp.Find("#items_nodata");
        IElement ItemVirtualizedNoData() => comp.Find("#items_virtualized_nodata");
        IElement ItemProviderNoData() => comp.Find("#item_provider_nodata");

        ItemNoData().InnerHtml.Should().Be("No data");
        ItemVirtualizedNoData().InnerHtml.Should().Be("No data");
        ItemProviderNoData().InnerHtml.Should().Be("No data");
    }

    [Test]
    public void VirtualizeMaxItemCount_IsPassedToInnerVirtualize()
    {
        var comp = Context.Render<MudVirtualize<string>>(p => p
            .Add(x => x.Items, new List<string> { "a" })
            .Add(x => x.Enabled, true)
            .Add(x => x.MaxItemCount, 42));

        var innerVirtualize = comp.FindComponent<Virtualize<string>>();
        innerVirtualize.Instance.MaxItemCount.Should().Be(42);
    }
}
