using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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
        public async Task MessageBox_CloseOnEscapeKey_NoOptions_NoMudDefaults()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = (DialogService)Context.Services.GetService<IDialogService>()!;
            service.Should().NotBe(null);

            // Open the message box.
            // We need the DialogReference to access the DialogInstance, which allows us to handle key events
            // directly through the HandleKeyDown method since KeyInterceptor does not work with bUnit.
            IDialogReference dialogReference = null;
            Task<DialogResult> dialogResult = null;
            await comp.InvokeAsync(async () =>
            {
                // In DialogService, lines 252 through 291 handle the process of:
                // 1. Assigning the text.
                // 2. Converting it into MessageBoxOptions.
                // 3. Converting it again into DialogParameters.

                // The methods ShowMessageBox and MessageBox.ShowAsync handle the DialogReference
                // and return only the result. However, we need access to the instance from the reference,
                // so we are calling the method directly.
                var messageBoxOptions = new MessageBoxOptions
                {
                    MarkupMessage = (MarkupString)"I'm a pickle. What do you make of that?",
                    Title = "Boom!",
                    YesText = "Great",
                    NoText = "Whatever",
                    CancelText = "Go away!",
                };
                var parameters = new DialogParameters()
                {
                    [nameof(MessageBoxOptions.Title)] = messageBoxOptions.Title,
                    [nameof(MessageBoxOptions.Message)] = messageBoxOptions.Message,
                    [nameof(MessageBoxOptions.MarkupMessage)] = messageBoxOptions.MarkupMessage,
                    [nameof(MessageBoxOptions.CancelText)] = messageBoxOptions.CancelText,
                    [nameof(MessageBoxOptions.NoText)] = messageBoxOptions.NoText,
                    [nameof(MessageBoxOptions.YesText)] = messageBoxOptions.YesText,
                };
                dialogReference = await service.ShowAsync<MudMessageBox>(messageBoxOptions.Title, parameters);
                dialogResult = dialogReference.Result;
            });
            dialogReference.Should().NotBeNull();
            // this component has an instance of MudDialog as a cascading parameter allowing us to access HandleKeyDown
            var dialog = (MudMessageBox)dialogReference.Dialog!;
            // just the same as the above test method 
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button").Count.Should().Be(3);
            comp.Find(".mud-message-box__cancel-button").TrimmedText().Should().Be("Go away!");
            comp.Find(".mud-message-box__no-button").TrimmedText().Should().Be("Whatever");
            comp.Find(".mud-message-box__yes-button").TrimmedText().Should().Be("Great");

            await comp.InvokeAsync(() => dialog.DialogInstance?.HandleKeyDown(new KeyboardEventArgs { Key = "Escape" }));

            comp.FindAll("button").Count.Should().Be(3);

            // close it manually
            comp.FindAll("button")[0].Click();
            comp.FindAll("button").Should().BeEmpty();

            dialogResult?.Result.Data?.Should().BeNull();
        }

        [Test]
        public async Task MessageBox_CloseOnEscapeKey_WithOptions_NoMudDefaults()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = (DialogService)Context.Services.GetService<IDialogService>();
            service.Should().NotBe(null);

            // Open the message box.
            // We need the DialogReference to access the DialogInstance, which allows us to handle key events
            // directly through the HandleKeyDown method since KeyInterceptor does not work with bUnit.
            IDialogReference dialogReference = null;
            Task<DialogResult> dialogResult = null;
            var dialogOptions = new DialogOptions { CloseOnEscapeKey = true };
            await comp.InvokeAsync(async () =>
            {
                // In DialogService, lines 252 through 291 handle the process of:
                // 1. Assigning the text.
                // 2. Converting it into MessageBoxOptions.
                // 3. Converting it again into DialogParameters.
                // The ShowMessageBox method handles the DialogReference and returns the result.
                var messageBoxOptions = new MessageBoxOptions
                {
                    MarkupMessage = (MarkupString)"I'm a pickle. What do you make of that?",
                    Title = "Boom!",
                    YesText = "Great",
                    NoText = "Whatever",
                    CancelText = "Go away!",
                };
                var parameters = new DialogParameters()
                {
                    [nameof(MessageBoxOptions.Title)] = messageBoxOptions.Title,
                    [nameof(MessageBoxOptions.Message)] = messageBoxOptions.Message,
                    [nameof(MessageBoxOptions.MarkupMessage)] = messageBoxOptions.MarkupMessage,
                    [nameof(MessageBoxOptions.CancelText)] = messageBoxOptions.CancelText,
                    [nameof(MessageBoxOptions.NoText)] = messageBoxOptions.NoText,
                    [nameof(MessageBoxOptions.YesText)] = messageBoxOptions.YesText,
                };
                dialogReference = await service.ShowAsync<MudMessageBox>(messageBoxOptions.Title, parameters, dialogOptions);
                dialogResult = dialogReference.Result;
            });
            dialogReference.Should().NotBeNull();
            // this component has an instance of MudDialog as a cascading parameter allowing us to access HandleKeyDown
            var dialog = (MudMessageBox)dialogReference.Dialog!;
            // just the same as the above test method 
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button").Count.Should().Be(3);
            comp.Find(".mud-message-box__cancel-button").TrimmedText().Should().Be("Go away!");
            comp.Find(".mud-message-box__no-button").TrimmedText().Should().Be("Whatever");
            comp.Find(".mud-message-box__yes-button").TrimmedText().Should().Be("Great");

            await comp.InvokeAsync(() => dialog.DialogInstance?.HandleKeyDown(new KeyboardEventArgs { Key = "Escape" }));

            comp.FindAll("button").Should().BeEmpty();

            dialogResult?.Result.Data?.Should().BeNull();
        }

        [Test]
        public async Task MessageBox_CloseOnEscapeKey_NoOptions_WithMudDefaults()
        {
            var comp = Context.RenderComponent<MudDialogProvider>(builder =>
            {
                builder.Add(p => p.CloseOnEscapeKey, true);
            });
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);

            // open message box.
            // we need the DialogReference to access the DialogInstance to access the HandleKeyDown
            // keyinterceptor does not seem to work in unit tests so I can't just "key down" on the correct element
            IDialogReference dialogReference = null;
            Task<DialogResult> dialogResult = null;
            await comp.InvokeAsync(async () =>
            {
                // DialogService line 252 through 291 show assigning the text, turning it into messageboxoptions, then again to dialogparameters
                // showmessagebox itself handles the dialogreference and returns the result only
                var messageBoxOptions = new MessageBoxOptions
                {
                    MarkupMessage = (MarkupString)"I'm a pickle. What do you make of that?",
                    Title = "Boom!",
                    YesText = "Great",
                    NoText = "Whatever",
                    CancelText = "Go away!",
                };
                var parameters = new DialogParameters()
                {
                    [nameof(MessageBoxOptions.Title)] = messageBoxOptions.Title,
                    [nameof(MessageBoxOptions.Message)] = messageBoxOptions.Message,
                    [nameof(MessageBoxOptions.MarkupMessage)] = messageBoxOptions.MarkupMessage,
                    [nameof(MessageBoxOptions.CancelText)] = messageBoxOptions.CancelText,
                    [nameof(MessageBoxOptions.NoText)] = messageBoxOptions.NoText,
                    [nameof(MessageBoxOptions.YesText)] = messageBoxOptions.YesText,
                };
                dialogReference = await service?.ShowAsync<MudMessageBox>(messageBoxOptions.Title, parameters);
                dialogResult = dialogReference.Result;
            });
            dialogReference.Should().NotBeNull();
            // this component has an instance of MudDialog as a cascading parameter allowing us to access HandleKeyDown
            var dialog = (MudMessageBox)dialogReference.Dialog;
            // just the same as the above test method 
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");
            comp.FindAll("button").Count.Should().Be(3);
            comp.Find(".mud-message-box__cancel-button").TrimmedText().Should().Be("Go away!");
            comp.Find(".mud-message-box__no-button").TrimmedText().Should().Be("Whatever");
            comp.Find(".mud-message-box__yes-button").TrimmedText().Should().Be("Great");

            await comp.InvokeAsync(() => dialog.DialogInstance.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape" }));

            comp.FindAll("button").Should().BeEmpty();

            dialogResult?.Result.Data?.Should().BeNull();
        }
    }
}
