using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class ToggleIconButtonTest
    {
        [Test]
        public void DefaultValueTest()
        {
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            var comp = ctx.RenderComponent<MudToggleIconButton>();

            comp.Instance.Toggled.Should().BeFalse();
        }

        [Test]
        public async Task ToggleTest()
        {
            using var ctx = new Bunit.TestContext();
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());

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
