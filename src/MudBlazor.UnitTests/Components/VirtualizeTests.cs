using AngleSharp.Dom;
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
    public void VirtualizeNoRecordTest()
    {
        var comp = Context.RenderComponent<VirtualizeNoRecordsContentTest>();

        IElement ItemNoData() => comp.Find("#items_nodata");
        IElement ItemVirtualizedNoData() => comp.Find("#items_virtualized_nodata");
        IElement ItemProviderNoData() => comp.Find("#item_provider_nodata");

        ItemNoData().InnerHtml.Should().Be("No data");
        ItemVirtualizedNoData().InnerHtml.Should().Be("No data");
        ItemProviderNoData().InnerHtml.Should().Be("No data");
    }
}
