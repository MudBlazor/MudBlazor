#pragma warning disable 1998

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
using MudBlazor.UnitTests.TestComponents.Dialog;
using NUnit.Framework;
using NUnit.Framework.Internal;


namespace MudBlazor.UnitTests
{

    [TestFixture]
    public class SnackbarTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.AddMudBlazorServices();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        [Test]
        public async Task SimpleTest()
        {
            var comp = ctx.RenderComponent<MudSnackbarProvider>();
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty();
            var service = ctx.Services.GetService<ISnackbar>() as SnackbarService;
            service.Should().NotBe(null);
            // shoot out a snackbar
            await comp.InvokeAsync(() => service?.Add("Boom, big reveal. Im a pickle!"));
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            comp.Find("div.mud-snackbar-content-message").TrimmedText().Should().Be("Boom, big reveal. Im a pickle!");
            // close by click on the snackbar
            comp.Find("button").Click();
            comp.WaitForAssertion(() => comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty(), TimeSpan.FromMilliseconds(100));
        }

    }
}
