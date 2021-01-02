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
    public class DialogTests
    {
        private Bunit.TestContext ctx;

        [SetUp]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            ctx.JSInterop.Mode = JSRuntimeMode.Loose;
            ctx.Services.AddSingleton<NavigationManager>(new MockNavigationManager());
            ctx.Services.AddSingleton<IDialogService>(new DialogService());
            ctx.Services.AddSingleton<ISnackbar>(new MockSnackbar());
            ctx.Services.AddSingleton<IResizeListenerService>(new MockResizeListenerService());
            ctx.Services.AddScoped(sp => new HttpClient());
            ctx.Services.AddOptions();
        }

        [TearDown]
        public void TearDown() => ctx.Dispose();

        /// <summary>
        /// Opening and closing a simple dialog
        /// </summary>
        [Test]
        public async Task SimpleTest() {
            var comp = ctx.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service=ctx.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference=null;
            // open simple test dialog
            await comp.InvokeAsync(()=> dialogReference=service?.Show<TestDialogOkCancel>());
            dialogReference.Should().NotBe(null);
            Console.WriteLine(comp.Markup);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Wabalabadubdub!");
            // close by click outside the dialog
            comp.Find("div.mud-overlay").Click();
            comp.Markup.Trim().Should().BeEmpty();
            var result=await dialogReference.Result;
            result.Cancelled.Should().BeTrue();
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<TestDialogOkCancel>());
            // close by click on cancel button
            comp.FindAll("button")[0].Click();
            result = await dialogReference.Result;
            result.Cancelled.Should().BeTrue();
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<TestDialogOkCancel>());
            // close by click on ok button
            comp.FindAll("button")[1].Click();
            result = await dialogReference.Result;
            result.Cancelled.Should().BeFalse();
        }

    }
}
