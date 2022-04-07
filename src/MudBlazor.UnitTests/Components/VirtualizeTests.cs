
#pragma warning disable CS1998 // async without await

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using static Bunit.ComponentParameterFactory;

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
