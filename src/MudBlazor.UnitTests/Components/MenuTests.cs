#pragma warning disable IDE1006 // leading underscore

using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MudBlazor.UnitTests.Mocks;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using NUnit.Framework.Internal;
using static Bunit.ComponentParameterFactory;

namespace MudBlazor.UnitTests
{
    [TestFixture]
    public class MenuTests
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
        public void OpenMenu_ClickFirstItem_CheckClosed()
        {
            var comp = ctx.RenderComponent<MenuTest1>();
            comp.FindAll("button.mud-button-root").First().Click();
            comp.FindAll("div.mud-list-item").Count().Should().Be(3);
            comp.FindAll("div.mud-list-item").First().Click();
            comp.FindAll("div.mud-popover-open").Count().Should().Be(0);
        }
    }
}
