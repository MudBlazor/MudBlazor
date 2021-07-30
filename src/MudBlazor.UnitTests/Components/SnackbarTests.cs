﻿
using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class SnackbarTests : BunitTest
    {
        [Test]
        public async Task SimpleTest()
        {
            var comp = Context.RenderComponent<MudSnackbarProvider>();
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<ISnackbar>() as SnackbarService;
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
            var comp = Context.RenderComponent<MudSnackbarProvider>();
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<ISnackbar>() as SnackbarService;
            service.Should().NotBe(null);
            // shoot out a snackbar
            await comp.InvokeAsync(() => service?.Add("Hello <span>World</span>"));
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().NotBeEmpty();
            comp.Find("div.mud-snackbar-content-message>span").Should().NotBeNull();
        }

        [Test]
        public async Task DisposeTest()
        {
            var comp = Context.RenderComponent<MudSnackbarProvider>();
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<ISnackbar>() as SnackbarService;
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

        [Test]
        public async Task IconTest()
        {
            var comp = Context.RenderComponent<MudSnackbarProvider>();
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<ISnackbar>() as SnackbarService;
            // shoot out a snackbar
            await comp.InvokeAsync(() => service?.Add("Boom, big reveal. Im a pickle!"));
            Console.WriteLine(comp.Markup);
            // Test that the snackbar has an icon.
            comp.Find("#mud-snackbar-container .mud-snackbar .mud-snackbar-icon").InnerHtml.Trim().Should().NotBeEmpty();
            // close by click on the snackbar
            comp.Find("button").Click();
            comp.WaitForAssertion(() => comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty(), TimeSpan.FromMilliseconds(100));
        }

        [Test]
        public async Task HideIconTest()
        {
            var comp = Context.RenderComponent<MudSnackbarProvider>();
            Console.WriteLine(comp.Markup);
            comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<ISnackbar>() as SnackbarService;
            // shoot out a snackbar
            await comp.InvokeAsync(() => service?.Add("Boom, big reveal. Im a pickle!", Severity.Success, config => { config.HideIcon = true; }));
            Console.WriteLine(comp.Markup);
            // Test that the snackbar does NOT have an icon.
            var hasIcon = comp.Find("#mud-snackbar-container .mud-snackbar").FirstElementChild.ClassName.Contains("mud-snackbar-icon");
            Assert.IsFalse(hasIcon);
            // close by click on the snackbar
            comp.Find("button").Click();
            comp.WaitForAssertion(() => comp.Find("#mud-snackbar-container").InnerHtml.Trim().Should().BeEmpty(), TimeSpan.FromMilliseconds(100));
        }

    }
}
