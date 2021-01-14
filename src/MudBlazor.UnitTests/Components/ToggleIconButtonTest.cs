#pragma warning disable 1998

using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToggleIconButtonTest
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddTestServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public void DefaultValueTest()
        {
            var comp = ctx.RenderComponent<MudToggleIconButton>();
            comp.Instance.Toggled.Should().BeFalse();
        }

        [Test]
        public void ToggleTest()
        {
            var boundValue = false;
            var comp = ctx.RenderComponent<MudToggleIconButton>(parameters => parameters
                .Add(p => p.Toggled, boundValue)
                .Add(p => p.ToggledChanged, (toggleValue) => boundValue = toggleValue)
                );
            comp.Find("button").Click();
            boundValue.Should().BeTrue();
            comp.Find("button").Click();
            boundValue.Should().BeFalse();
            comp.RenderCount.Should().Be(3);
        }
    }
}
