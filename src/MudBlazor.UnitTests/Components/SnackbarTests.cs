#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class SnackbarTests
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

        [Test]
        public async Task HtmlInMessages()
        {
            var comp = ctx.RenderComponent<MudSnackbarProvider>();
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty();
            var service = ctx.Services.GetService<ISnackbar>() as SnackbarService;
            service.Should().NotBe(null);
            // shoot out a snackbar
            await comp.InvokeAsync(() => service?.Add("Hello <span>World</span>"));
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            comp.Find("div.mud-snackbar-content-message>span").Should().NotBeNull();
        }

        [Test]
        public async Task DisposeTest()
        {
            var comp = ctx.RenderComponent<MudSnackbarProvider>();
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty();
            var service = ctx.Services.GetService<ISnackbar>() as SnackbarService;
            service.Should().NotBe(null);

            // shoot out a snackbar
            Snackbar snackbar = null;
            await comp.InvokeAsync(() => snackbar = service?.Add("Boom, big reveal. Im a pickle!"));
            Console.WriteLine(comp.Markup);

            snackbar?.Dispose();

            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            comp.Find("div.mud-snackbar-content-message").TrimmedText().Should().Be("Boom, big reveal. Im a pickle!");
            // close by click on the snackbar
            comp.Find("button").Click();
            comp.WaitForAssertion(() => comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty(), TimeSpan.FromMilliseconds(100));
        }
    }
}
