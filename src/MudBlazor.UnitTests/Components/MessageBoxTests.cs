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
    public class MessageBoxTests
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

        [Test, Timeout(3000)]
        [TestCase(0, null)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public async Task MessageBox_Should_ReturnTrue(int clickButtonIndex, bool? expectedResult)
        {
            var comp = ctx.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = ctx.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // open mbox
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBox("Boom!", "I'm a pickle. What do you make of that?", "Great",
                    "Whatever", "Go away!");
            });
            Console.WriteLine(comp.Markup);
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
    }

}
