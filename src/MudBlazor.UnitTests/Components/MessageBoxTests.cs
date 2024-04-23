
using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class MessageBoxTests : BunitTest
    {
        [Test, CancelAfter(3000)]
        [TestCase(0, null)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public async Task MessageBox_Should_ReturnTrue(int clickButtonIndex, bool? expectedResult)
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // open mbox
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBox("Boom!", "I'm a pickle. What do you make of that?", "Great",
                    "Whatever", "Go away!");
            });
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button")[0].TrimmedText().Should().Be("Go away!");
            comp.FindAll("button")[1].TrimmedText().Should().Be("Whatever");
            comp.FindAll("button")[2].TrimmedText().Should().Be("Great");

            // close by click on Great
            comp.FindAll("button")[clickButtonIndex].Click();
            comp.Markup.Trim().Should().BeEmpty();
            yesNoCancel.Result.Should().Be(expectedResult);
        }

        [Test, CancelAfter(3000)]
        [TestCase(0, null)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public async Task MessageBox_Should_ReturnTruewithMarkupVariant(int clickButtonIndex, bool? expectedResult)
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // open mbox
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBox("Boom!", (MarkupString)$"I'm a pickle. What do you make of that?", "Great",
                    "Whatever", "Go away!");
            });
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button")[0].TrimmedText().Should().Be("Go away!");
            comp.FindAll("button")[1].TrimmedText().Should().Be("Whatever");
            comp.FindAll("button")[2].TrimmedText().Should().Be("Great");

            // close by click on Great
            comp.FindAll("button")[clickButtonIndex].Click();
            comp.Markup.Trim().Should().BeEmpty();
            yesNoCancel.Result.Should().Be(expectedResult);
        }

        [Test]
        public async Task MessageBox_KeyNavTest()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // open mbox
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBox("Boom!", (MarkupString)$"I'm a pickle. What do you make of that?", "Great",
                    "Whatever", "Go away!");
            });
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button")[0].TrimmedText().Should().Be("Go away!");
            comp.FindAll("button")[1].TrimmedText().Should().Be("Whatever");
            comp.FindAll("button")[2].TrimmedText().Should().Be("Great");
            comp.FindAll(".mud-dialog-actions div")[0].KeyDown(Key.Escape);

            comp.FindAll("button").Should().BeEmpty();
            yesNoCancel.Result.Should().Be(null);
        }

    }
}
