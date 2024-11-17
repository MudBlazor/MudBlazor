using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents.Virtualize;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

#nullable enable
[TestFixture]
public class VirtualizeTests : BunitTest
{
    [Test]
    public void VirtualizeRenderTest()
    {
        var comp = Context.RenderComponent<VirtualizeTest>();
        var virtualize = comp.FindComponent<MudVirtualize<string>>();
        virtualize.Instance.ChildContent.Should().NotBeNull();
        comp.FindComponents<MudText>().Count.Should().Be(1);
    }

    [Test]
    public async Task VirtualizeNoRecordTest()
    {
        var comp = Context.RenderComponent<VirtualizeNoRecordsContentTest>();
        await comp.Instance.CompleteServerDataFunc;

        var itemNoData = comp.Find("#items_nodata");
        var itemProviderNoData = comp.Find("#item_provider_nodata");
        var itemVirtualizedNoData = comp.Find("#items_virtualized_nodata");

        itemNoData.InnerHtml.Should().Be("No data");
        itemProviderNoData.InnerHtml.Should().Be("No data");
        itemVirtualizedNoData.InnerHtml.Should().Be("No data");
    }
}
