#pragma warning disable CS1998 // async without await
#pragma warning disable IDE1006 // leading underscore

using System;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using MudBlazor.UnitTests.TestComponents;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MudBlazor.UnitTests.Components
{

    [TestFixture]
    public class DialogTests
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

        /// <summary>
        /// Opening and closing a simple dialog
        /// </summary>
        [Test]
        public async Task SimpleTest()
        {
            var comp = ctx.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = ctx.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            dialogReference.Should().NotBe(null);
            Console.WriteLine(comp.Markup);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Wabalabadubdub!");
            // close by click outside the dialog
            comp.Find("div.mud-overlay").Click();
            comp.Markup.Trim().Should().BeEmpty();
            var result = await dialogReference.Result;
            result.Cancelled.Should().BeTrue();
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            // close by click on cancel button
            comp.FindAll("button")[0].Click();
            result = await dialogReference.Result;
            result.Cancelled.Should().BeTrue();
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            // close by click on ok button
            comp.FindAll("button")[1].Click();
            result = await dialogReference.Result;
            result.Cancelled.Should().BeFalse();
        }

        /// <summary>
        /// Opening and closing an inline dialog. Click on open will open the inlined dialog.
        ///
        /// Note: this test uses two different components, one containing the dialog provider and
        /// one containing the open button and the inline dialog
        /// </summary>
        [Test]
        public async Task InlineDialogTest()
        {
            var comp = ctx.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = ctx.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // displaying the component with the inline dialog only renders the open button
            var comp1 = ctx.RenderComponent<TestInlineDialog>();
            comp1.FindComponents<MudButton>().Count.Should().Be(1);
            Console.WriteLine("Open button: " + comp1.Markup);
            // open the dialog
            comp1.Find("button").Click();
            Console.WriteLine("\nOpened dialog: " + comp.Markup);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Wabalabadubdub!");
            // close by click on ok button
            comp.Find("button").Click();
            comp.Markup.Trim().Should().BeEmpty();
        }

        /// <summary>
        /// Click outside the dialog (or any other method) must update the IsVisible parameter two-way binding on close
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task InlineDialog_Should_UpdateIsVisibleOnClose()
        {
            var comp = ctx.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = ctx.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // displaying the component with the inline dialog only renders the open button
            var comp1 = ctx.RenderComponent<InlineDialogIsVisibleStateTest>();
            // open the dialog
            comp1.Find("button").Click();
            Console.WriteLine("\nOpened dialog: " + comp.Markup);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            // close by click outside
            comp.Find("div.mud-overlay").Click();
            comp.Markup.Trim().Should().BeEmpty();
            // open again
            comp1.Find("button").Click();
            comp.WaitForAssertion(() => comp.Find("div.mud-dialog-container").Should().NotBe(null), TimeSpan.FromSeconds(2));
            // close again by click outside
            comp.Find("div.mud-overlay").Click();
            comp.WaitForAssertion(() => comp.Markup.Trim().Should().BeEmpty(), TimeSpan.FromSeconds(2));
        }

    }
}
