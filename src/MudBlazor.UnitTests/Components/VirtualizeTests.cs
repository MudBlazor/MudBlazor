using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
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
    }
}
