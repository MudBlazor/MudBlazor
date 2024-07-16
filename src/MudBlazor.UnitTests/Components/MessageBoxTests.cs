using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
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

            // open message box.
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBox(
                    "Boom!",
                    "I'm a pickle. What do you make of that?",
                    "Great",
                    "Whatever",
                    "Go away!");
            });

            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button").Count.Should().Be(3);
            comp.Find(".mud-message-box__cancel-button").TrimmedText().Should().Be("Go away!");
            comp.Find(".mud-message-box__no-button").TrimmedText().Should().Be("Whatever");
            comp.Find(".mud-message-box__yes-button").TrimmedText().Should().Be("Great");

            // close message box by clicking on Great.
            comp.FindAll("button")[clickButtonIndex].Click();
            comp.Markup.Trim().Should().BeEmpty();
            yesNoCancel.Result.Should().Be(expectedResult);
        }

        [Test, CancelAfter(3000)]
        [TestCase(0, null)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public async Task MessageBox_Should_ReturnTrueWithMarkupVariant(int clickButtonIndex, bool? expectedResult)
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);

            // open message box.
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBox(
                    "Boom!",
                    (MarkupString)"I'm a pickle. What do you make of that?",
                    "Great",
                    "Whatever",
                    "Go away!");
            });

            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button").Count.Should().Be(3);
            comp.Find(".mud-message-box__cancel-button").TrimmedText().Should().Be("Go away!");
            comp.Find(".mud-message-box__no-button").TrimmedText().Should().Be("Whatever");
            comp.Find(".mud-message-box__yes-button").TrimmedText().Should().Be("Great");

            // close message box by clicking on Great.
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

            // open message box.
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBox(
                    "Boom!",
                    (MarkupString)"I'm a pickle. What do you make of that?",
                    "Great",
                    "Whatever",
                    "Go away!");
            });

            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button").Count.Should().Be(3);
            comp.Find(".mud-message-box__cancel-button").TrimmedText().Should().Be("Go away!");
            comp.Find(".mud-message-box__no-button").TrimmedText().Should().Be("Whatever");
            comp.Find(".mud-message-box__yes-button").TrimmedText().Should().Be("Great");

            comp.FindAll(".mud-dialog-actions div")[0].KeyDown(Key.Escape);

            comp.FindAll("button").Should().BeEmpty();
            yesNoCancel.Result.Should().Be(null);
        }

    }
}
