using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.UnitTests.TestComponents.MessageBox;
using Moq;
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
            var comp = Context.Render<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);

            // open message box.
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBoxAsync(
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

            // Assert there are exactly 3 buttons
            var buttons = comp.FindAll(".mud-dialog-actions button");
            buttons.Count.Should().Be(3);

            // Verify each button's text and class and that they are in the correct order
            buttons[0].TrimmedText().Should().Be("Go away!"); // First button (Cancel)
            buttons[0].ClassList.Should().Contain("mud-message-box__cancel-button");
            buttons[1].TrimmedText().Should().Be("Whatever"); // Second button (No)
            buttons[1].ClassList.Should().Contain("mud-message-box__no-button");
            buttons[2].TrimmedText().Should().Be("Great");    // Third button (Yes)
            buttons[2].ClassList.Should().Contain("mud-message-box__yes-button");

            // close message box by clicking on Great.
            await comp.FindAll(".mud-dialog-actions button")[clickButtonIndex].ClickAsync();
            comp.Markup.Trim().Should().BeEmpty();
            yesNoCancel.Result.Should().Be(expectedResult);
        }

        [Test, CancelAfter(3000)]
        [TestCase(0, null)]
        [TestCase(1, false)]
        [TestCase(2, true)]
        public async Task MessageBox_Should_ReturnTrueWithMarkupVariant(int clickButtonIndex, bool? expectedResult)
        {
            var comp = Context.Render<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);

            // open message box.
            Task<bool?> yesNoCancel = null;
            await comp.InvokeAsync(() =>
            {
                yesNoCancel = service?.ShowMessageBoxAsync(
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

            // Assert there are exactly 3 buttons
            var buttons = comp.FindAll(".mud-dialog-actions button");
            buttons.Count.Should().Be(3);

            // Verify each button's text and class and that they are in the correct order
            buttons[0].TrimmedText().Should().Be("Go away!"); // First button (Cancel)
            buttons[0].ClassList.Should().Contain("mud-message-box__cancel-button");
            buttons[1].TrimmedText().Should().Be("Whatever"); // Second button (No)
            buttons[1].ClassList.Should().Contain("mud-message-box__no-button");
            buttons[2].TrimmedText().Should().Be("Great");    // Third button (Yes)
            buttons[2].ClassList.Should().Contain("mud-message-box__yes-button");

            // close message box by clicking on Great.
            await comp.FindAll(".mud-dialog-actions button")[clickButtonIndex].ClickAsync();
            comp.Markup.Trim().Should().BeEmpty();
            yesNoCancel.Result.Should().Be(expectedResult);
        }

        [Test]
        public async Task MessageBox_CloseOnEscapeKey_NoOptions_NoMudDefaults()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<MudDialogProvider>();
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
            var dialogInstance = dialog.DialogInstance.GetDialogContainer();
            // just the same as the above test method 
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");

            // Assert there are exactly 3 buttons
            var buttons = comp.FindAll(".mud-dialog-actions button");
            buttons.Count.Should().Be(3);

            // Verify each button's text and class and that they are in the correct order
            buttons[0].TrimmedText().Should().Be("Go away!"); // First button (Cancel)
            buttons[0].ClassList.Should().Contain("mud-message-box__cancel-button");
            buttons[1].TrimmedText().Should().Be("Whatever"); // Second button (No)
            buttons[1].ClassList.Should().Contain("mud-message-box__no-button");
            buttons[2].TrimmedText().Should().Be("Great");    // Third button (Yes)
            buttons[2].ClassList.Should().Contain("mud-message-box__yes-button");

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(dialogInstance.ElementId, new KeyboardEventArgs { Key = "Escape" }));

            comp.FindAll("button").Count.Should().Be(3);

            // close it manually
            await comp.FindAll("button")[0].ClickAsync();
            comp.FindAll("button").Should().BeEmpty();

            dialogResult?.Result.Data?.Should().BeNull();
        }

        [Test]
        public async Task MessageBox_CloseOnEscapeKey_WithOptions_NoMudDefaults()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<MudDialogProvider>();
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
            var dialogInstance = dialog.DialogInstance.GetDialogContainer();
            // just the same as the above test method 
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");

            // Assert there are exactly 3 buttons
            var buttons = comp.FindAll(".mud-dialog-actions button");
            buttons.Count.Should().Be(3);

            // Verify each button's text and class and that they are in the correct order
            buttons[0].TrimmedText().Should().Be("Go away!"); // First button (Cancel)
            buttons[0].ClassList.Should().Contain("mud-message-box__cancel-button");
            buttons[1].TrimmedText().Should().Be("Whatever"); // Second button (No)
            buttons[1].ClassList.Should().Contain("mud-message-box__no-button");
            buttons[2].TrimmedText().Should().Be("Great");    // Third button (Yes)
            buttons[2].ClassList.Should().Contain("mud-message-box__yes-button");

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(dialogInstance.ElementId, new KeyboardEventArgs { Key = "Escape" }));

            comp.FindAll("button").Should().BeEmpty();

            dialogResult?.Result.Data?.Should().BeNull();
        }

        [Test]
        public async Task MessageBox_CloseOnEscapeKey_NoOptions_WithMudDefaults()
        {
            var keyInterceptorService = Context.AddKeyInterceptorService();
            var comp = Context.Render<MudDialogProvider>(builder =>
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
            var dialog = (MudMessageBox)dialogReference.Dialog!;
            var dialogInstance = dialog.DialogInstance.GetDialogContainer();
            // just the same as the above test method 
            comp.Find("div.mud-message-box").Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Contain("Boom!");
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Contain("pickle");

            // Assert there are exactly 3 buttons
            var buttons = comp.FindAll(".mud-dialog-actions button");
            buttons.Count.Should().Be(3);

            // Verify each button's text and class and that they are in the correct order
            buttons[0].TrimmedText().Should().Be("Go away!"); // First button (Cancel)
            buttons[0].ClassList.Should().Contain("mud-message-box__cancel-button");
            buttons[1].TrimmedText().Should().Be("Whatever"); // Second button (No)
            buttons[1].ClassList.Should().Contain("mud-message-box__no-button");
            buttons[2].TrimmedText().Should().Be("Great");    // Third button (Yes)
            buttons[2].ClassList.Should().Contain("mud-message-box__yes-button");

            await comp.InvokeAsync(() => keyInterceptorService.OnKeyDown(dialogInstance.ElementId, new KeyboardEventArgs() { Key = "Escape" }));

            comp.FindAll("button").Should().BeEmpty();

            dialogResult?.Result.Data?.Should().BeNull();
        }

        [Test]
        public async Task MessageBox_Should_UseGlobalBackgroundClass_WhenOptionsAreNotProvided()
        {
            var provider = Context.Render<MudDialogProvider>(builder => builder.Add(x => x.BackgroundClass, "global-background"));
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBeNull();
            Task<bool?> messageBoxTask = null!;

            await provider.InvokeAsync(() =>
            {
                messageBoxTask = service!.ShowMessageBoxAsync("Boom!", "I'm a pickle. What do you make of that?");
            });

            provider.Find("div.mud-overlay-dialog").ClassList.Should().Contain("global-background");
            await provider.Find(".mud-message-box__yes-button").ClickAsync();
            (await messageBoxTask).Should().BeTrue();
        }

        [Test]
        public async Task MessageBox_Should_PreferExplicitBackgroundClass_OverGlobalBackgroundClass()
        {
            var provider = Context.Render<MudDialogProvider>(builder => builder.Add(x => x.BackgroundClass, "global-background"));
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBeNull();
            var dialogOptions = new DialogOptions { BackgroundClass = "explicit-background" };
            Task<bool?> messageBoxTask = null!;

            await provider.InvokeAsync(() =>
            {
                messageBoxTask = service!.ShowMessageBoxAsync("Boom!", "I'm a pickle. What do you make of that?", options: dialogOptions);
            });

            var overlayClasses = provider.Find("div.mud-overlay-dialog").ClassList;
            overlayClasses.Should().Contain("explicit-background");
            overlayClasses.Should().NotContain("global-background");
            await provider.Find(".mud-message-box__yes-button").ClickAsync();
            (await messageBoxTask).Should().BeTrue();
        }

        [Test]
        public async Task MessageBox_Should_RenderReverseButtonOrder_WhenGlobalOptionIsSet()
        {
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            var provider = Context.Render<MudDialogProvider>(builder =>
            {
                builder.Add(x => x.ReverseMessageBoxButtonOrder, true);
            });

            Task<bool?> messageBoxTask = null!;
            await provider.InvokeAsync(() =>
            {
                messageBoxTask = service!.ShowMessageBoxAsync(new MessageBoxOptions
                {
                    Title = "Boom!",
                    Message = "I'm a pickle. What do you make of that?",
                    YesText = "Yes",
                    NoText = "No",
                    CancelText = "Cancel"
                });
            });

            var dialogMessageBox = provider.FindComponent<MudMessageBox>();
            dialogMessageBox.Instance.IsButtonOrderReversed.Should().BeTrue();

            var buttons = provider.FindAll(".mud-dialog-actions button");
            buttons.Count.Should().Be(3);
            buttons[0].TrimmedText().Should().Be("Yes");
            buttons[0].ClassList.Should().Contain("mud-message-box__yes-button");
            buttons[1].TrimmedText().Should().Be("No");
            buttons[1].ClassList.Should().Contain("mud-message-box__no-button");
            buttons[2].TrimmedText().Should().Be("Cancel");
            buttons[2].ClassList.Should().Contain("mud-message-box__cancel-button");

            await provider.Find(".mud-message-box__yes-button").ClickAsync();
            (await messageBoxTask).Should().BeTrue();
        }

        [Test]
        public async Task InlineMessageBox_ShouldNot_RenderReverseButtonOrder()
        {
            var provider = Context.Render<MudDialogProvider>();

            var inlineMessageBox = Context.Render<MudMessageBox>(parameters => parameters
                .Add(p => p.YesText, "Yes")
                .Add(p => p.NoText, "No")
                .Add(p => p.CancelText, "Cancel")
            );

            Task<bool?> messageBoxTask = null!;
            await inlineMessageBox.InvokeAsync(() =>
            {
                messageBoxTask = inlineMessageBox.Instance.ShowAsync();
            });

            var dialogMessageBox = provider.FindComponent<MudMessageBox>();
            dialogMessageBox.Instance.IsButtonOrderReversed.Should().BeFalse();

            var buttons = provider.FindAll(".mud-dialog-actions button");
            buttons.Count.Should().Be(3);
            buttons[0].TrimmedText().Should().Be("Cancel");
            buttons[0].ClassList.Should().Contain("mud-message-box__cancel-button");
            buttons[1].TrimmedText().Should().Be("No");
            buttons[1].ClassList.Should().Contain("mud-message-box__no-button");
            buttons[2].TrimmedText().Should().Be("Yes");
            buttons[2].ClassList.Should().Contain("mud-message-box__yes-button");

            await provider.Find(".mud-message-box__yes-button").ClickAsync();
            (await messageBoxTask).Should().BeTrue();
        }

        [Test]
        public async Task MessageBox_Show_ForwardsParametersToDialogService()
        {
            var expectedOptions = new DialogOptions
            {
                BackgroundClass = "custom-background",
                CloseOnEscapeKey = true
            };

            DialogParameters capturedParameters = null;
            string capturedTitle = null;
            DialogOptions capturedOptions = null;

            var dialogServiceMock = CreateDialogServiceMock(DialogResult.Ok(true), (title, parameters, options) =>
            {
                capturedTitle = title;
                capturedParameters = parameters;
                capturedOptions = options;
            });

            var titleContent = CreateMarkupFragment("custom-title", "Preferred title");
            var messageContent = CreateMarkupFragment("custom-message", "Preferred message");
            var yesButton = CreateButtonFragment("custom-yes", "Yes");
            var noButton = CreateButtonFragment("custom-no", "No");
            var cancelButton = CreateButtonFragment("custom-cancel", "Cancel");

            var inlineMessageBox = Context.Render<MudMessageBox>(parameters => parameters
                .Add(x => x.Title, "Title parameter")
                .Add(x => x.TitleContent, titleContent)
                .Add(x => x.Message, "Ignored message")
                .Add(x => x.MarkupMessage, (MarkupString)"<b>Ignored markup</b>")
                .Add(x => x.MessageContent, messageContent)
                .Add(x => x.YesText, "Ignored yes")
                .Add(x => x.YesButton, yesButton)
                .Add(x => x.NoText, "Ignored no")
                .Add(x => x.NoButton, noButton)
                .Add(x => x.CancelText, "Ignored cancel")
                .Add(x => x.CancelButton, cancelButton)
            );

            var result = await inlineMessageBox.Instance.ShowAsync(expectedOptions);

            result.Should().BeTrue();
            capturedTitle.Should().Be("Title parameter");
            capturedOptions.Should().BeSameAs(expectedOptions);
            capturedOptions.BackgroundClass.Should().Be("custom-background");
            capturedOptions.CloseOnEscapeKey.Should().BeTrue();
            capturedParameters.Should().NotBeNull();
            capturedParameters![nameof(MudMessageBox.Title)].Should().Be("Title parameter");
            capturedParameters[nameof(MudMessageBox.TitleContent)].Should().BeSameAs(titleContent);
            capturedParameters[nameof(MudMessageBox.Message)].Should().Be("Ignored message");
            capturedParameters[nameof(MudMessageBox.MarkupMessage)].Should().Be((MarkupString)"<b>Ignored markup</b>");
            capturedParameters[nameof(MudMessageBox.MessageContent)].Should().BeSameAs(messageContent);
            capturedParameters[nameof(MudMessageBox.YesText)].Should().Be("Ignored yes");
            capturedParameters[nameof(MudMessageBox.YesButton)].Should().BeSameAs(yesButton);
            capturedParameters[nameof(MudMessageBox.NoText)].Should().Be("Ignored no");
            capturedParameters[nameof(MudMessageBox.NoButton)].Should().BeSameAs(noButton);
            capturedParameters[nameof(MudMessageBox.CancelText)].Should().Be("Ignored cancel");
            capturedParameters[nameof(MudMessageBox.CancelButton)].Should().BeSameAs(cancelButton);

            dialogServiceMock.Verify(x => x.ShowAsync<MudMessageBox>("Title parameter", It.IsAny<DialogParameters>(), expectedOptions), Times.Once);
        }

        [TestCase(null, null)]
        [TestCase("cancel", null)]
        [TestCase("invalid", null)]
        [TestCase("false", false)]
        [TestCase("true", true)]
        public async Task MessageBox_Show_MapsDialogResults(string resultKind, bool? expectedResult)
        {
            var dialogResult = resultKind switch
            {
                null => null,
                "cancel" => DialogResult.Cancel(),
                "invalid" => DialogResult.Ok("not-a-bool"),
                "false" => DialogResult.Ok(false),
                "true" => DialogResult.Ok(true),
                _ => throw new ArgumentOutOfRangeException(nameof(resultKind))
            };

            CreateDialogServiceMock(dialogResult);

            var inlineMessageBox = Context.Render<MudMessageBox>(parameters => parameters
                .Add(x => x.Title, "Title")
                .Add(x => x.Message, "Message")
            );

            var result = await inlineMessageBox.Instance.ShowAsync();

            result.Should().Be(expectedResult);
        }

        [Test]
        public async Task MessageBox_VisibleParameter_OpensAndClosesTheDialog()
        {
            var provider = Context.Render<MudDialogProvider>();
            var visibleStateTest = Context.Render<MessageBoxVisibleStateTest>();

            provider.Markup.Trim().Should().BeEmpty();

            await visibleStateTest.Find(".open-message-box").ClickAsync();

            await provider.WaitForAssertionAsync(() =>
                provider.Find(".mud-message-box").Should().NotBeNull());

            await visibleStateTest.Find(".close-message-box").ClickAsync();

            await provider.WaitForAssertionAsync(() =>
                provider.Markup.Trim().Should().BeEmpty());
        }

        [TestCase(".custom-cancel", null)]
        [TestCase(".custom-no", false)]
        [TestCase(".custom-yes", true)]
        public async Task MessageBox_PrefersCustomContentAndButtons(string buttonSelector, bool? expectedResult)
        {
            var provider = Context.Render<MudDialogProvider>();

            var titleContent = CreateMarkupFragment("custom-title", "Preferred title");
            var messageContent = CreateMarkupFragment("custom-message", "Preferred message");

            var inlineMessageBox = Context.Render<MudMessageBox>(parameters => parameters
                .Add(x => x.Title, "Fallback title")
                .Add(x => x.TitleContent, titleContent)
                .Add(x => x.Message, "Ignored message")
                .Add(x => x.MarkupMessage, (MarkupString)"<b>Ignored markup</b>")
                .Add(x => x.MessageContent, messageContent)
                .Add(x => x.YesButton, CreateButtonFragment("custom-yes", "Custom yes"))
                .Add(x => x.NoButton, CreateButtonFragment("custom-no", "Custom no"))
                .Add(x => x.CancelButton, CreateButtonFragment("custom-cancel", "Custom cancel"))
            );

            Task<bool?> messageBoxTask = null;
            await inlineMessageBox.InvokeAsync(() =>
            {
                messageBoxTask = inlineMessageBox.Instance.ShowAsync();
            });

            await provider.WaitForAssertionAsync(() =>
            {
                provider.Find(".custom-title").TrimmedText().Should().Be("Preferred title");
                provider.Find(".custom-message").TrimmedText().Should().Be("Preferred message");
            });

            provider.Find("div.mud-dialog-title").TextContent.Should().NotContain("Fallback title");
            provider.Find("div.mud-dialog-content").TextContent.Should().NotContain("Ignored message");
            provider.Find("div.mud-dialog-content").TextContent.Should().NotContain("Ignored markup");

            var buttons = provider.FindAll(".mud-dialog-actions button");
            buttons.Count.Should().Be(3);
            buttons[0].ClassList.Should().Contain("custom-cancel");
            buttons[1].ClassList.Should().Contain("custom-no");
            buttons[2].ClassList.Should().Contain("custom-yes");

            await provider.Find(buttonSelector).ClickAsync();

            (await messageBoxTask).Should().Be(expectedResult);
        }

        /// <summary>
        /// Replaces the dialog service with a mock that returns a predefined result for <see cref="MudMessageBox"/> dialogs.
        /// </summary>
        /// <param name="dialogResult">The result returned from the mocked dialog reference.</param>
        /// <param name="onShow">An optional callback used to inspect the title, parameters, and options passed to the dialog service.</param>
        /// <returns>The configured dialog service mock.</returns>
        private Mock<IDialogService> CreateDialogServiceMock(DialogResult dialogResult, Action<string, DialogParameters, DialogOptions> onShow = null)
        {
            var dialogServiceMock = new Mock<IDialogService>();
            var dialogReference = new DialogReference(Guid.NewGuid(), dialogServiceMock.Object);
            dialogReference.Dismiss(dialogResult);

            dialogServiceMock
                .Setup(x => x.ShowAsync<MudMessageBox>(It.IsAny<string>(), It.IsAny<DialogParameters>(), It.IsAny<DialogOptions>()))
                .Callback<string, DialogParameters, DialogOptions>((title, parameters, options) => onShow?.Invoke(title, parameters, options))
                .ReturnsAsync(dialogReference);

            Context.Services.RemoveAll<IDialogService>();
            Context.Services.AddSingleton(dialogServiceMock.Object);

            return dialogServiceMock;
        }

        /// <summary>
        /// Creates a simple markup fragment with a CSS class and text content for title or message assertions.
        /// </summary>
        /// <param name="cssClass">The CSS class applied to the generated element.</param>
        /// <param name="text">The text content rendered inside the element.</param>
        /// <returns>A render fragment that outputs the requested markup.</returns>
        private static RenderFragment CreateMarkupFragment(string cssClass, string text)
        {
            return builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", cssClass);
                builder.AddContent(2, text);
                builder.CloseElement();
            };
        }

        /// <summary>
        /// Creates a <see cref="MudButton"/> fragment used to exercise custom MessageBox button rendering and activation.
        /// </summary>
        /// <param name="cssClass">The CSS class applied to the generated button.</param>
        /// <param name="text">The button label.</param>
        /// <returns>A render fragment that renders the requested button.</returns>
        private static RenderFragment CreateButtonFragment(string cssClass, string text)
        {
            return builder =>
            {
                builder.OpenComponent<MudButton>(0);
                builder.AddAttribute(1, nameof(MudButton.Class), cssClass);
                builder.AddAttribute(2, nameof(MudButton.ChildContent), (RenderFragment)(childBuilder => childBuilder.AddContent(3, text)));
                builder.CloseComponent();
            };
        }
    }
}
