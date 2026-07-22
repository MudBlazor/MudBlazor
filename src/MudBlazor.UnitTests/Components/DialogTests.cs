using System;
using System.Linq;
using System.Threading.Tasks;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.UnitTests.TestComponents;
using MudBlazor.UnitTests.TestComponents.Dialog;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components
{
    [TestFixture]
    public class DialogTests : BunitTest
    {
        /// <summary>
        /// Testing lifecycle of dialogs in dialogprovider
        /// </summary>
        [Test]
        public async Task LifecycleTest()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;
            await comp.InvokeAsync(async () =>
            {
                dialogReference = await service?.ShowAsync<DialogRender>();
                var result1 = await dialogReference.Result;
                //The second Dialog is added here, but the first dialog is still in the _dialogs collection of the dialogprovider, as only the result task was set to completion.
                //So DialogProvider will render again with 2 dialogs, but 1 is completed. This one needs to be excluded from rendering to prevent double initialize with no params.
                dialogReference = await service?.ShowAsync<DialogRender>();
                var result2 = await dialogReference.Result;
            });
            DialogRender.OnInitializedCount.Should().Be(2);
            //Reset global value
            DialogRender.OnInitializedCount = 0;
        }

        /// <summary>
        /// Opening and closing a simple dialog
        /// </summary>
        [Test]
        public async Task SimpleTest()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            dialogReference.Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Wabalabadubdub!");
            // close by click outside the dialog
            comp.Find("div.mud-overlay").Click();
            comp.Markup.Trim().Should().BeEmpty();
            var result = await dialogReference.Result;
            result.Canceled.Should().BeTrue();
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            // close by click on cancel button
            comp.FindAll("button")[0].Click();
            result = await dialogReference.Result;
            result.Canceled.Should().BeTrue();
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            // close by click on ok button
            comp.FindAll("button")[1].Click();
            result = await dialogReference.Result;
            result.Canceled.Should().BeFalse();

            //create 2 instances and dismiss all
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            var cont = comp.FindAll("div.mud-dialog-container");
            cont.Count.Should().Be(2);
            await comp.InvokeAsync(() => comp.Instance.DismissAll());
            cont = comp.FindAll("div.mud-dialog-container");
            cont.Count.Should().Be(0);

            // Close by using default close method
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            comp.FindAll("button")[2].Click();
            result = await dialogReference.Result;
            result.Data.Should().BeNull();
            result.DataType.Should().BeNull();
            result.Canceled.Should().BeFalse();
        }

        /// <summary>
        /// <para>Opening and closing an inline dialog. Click on open will open the inlined dialog.</para>
        /// <para>
        /// Note: this test uses two different components, one containing the dialog provider and
        /// one containing the open button and the inline dialog
        /// </para>
        /// </summary>
        [Test]
        public void InlineDialogTest()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // displaying the component with the inline dialog only renders the open button
            var comp1 = Context.RenderComponent<TestInlineDialog>();
            comp1.FindComponents<MudButton>().Count.Should().Be(1);
            // open the dialog
            comp1.Find("button").Click();
            comp1.WaitForAssertion(() =>
                comp.Find("div.mud-dialog-container").Should().NotBe(null)
            );
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Wabalabadubdub!");
            comp.Find("div.mud-dialog").GetAttribute("class").Should().Contain("mud-dialog-width-full");
            // close by click on ok button
            comp.Find("button").Click();
            comp.Markup.Trim().Should().BeEmpty();
        }

        /// <summary>
        /// https://github.com/MudBlazor/MudBlazor/issues/4098
        /// https://github.com/MudBlazor/MudBlazor/issues/8746
        /// </summary>
        [Test]
        public void InlineDialogShowMethodTest()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);

            var comp1 = Context.RenderComponent<InlineDialogShowMethod>();

            comp.Markup.Should().NotContain("Here be dragons");

            // open the dialog
            comp1.Find(".open-dialog-button").Click();
            comp1.WaitForAssertion(() => comp.Find("div.mud-dialog-container").Should().NotBe(null));

            comp.Markup.Should().Contain("Here be dragons");

            // close the dialog
            comp.Find(".close-dialog-button").Click();

            // messagebox should have opened
            comp.Markup.Should().Contain("dialog was successfully closed");

            // close by click on ok button
            comp.Find(".mud-message-box button").Click();

            comp.Markup.Trim().Should().BeEmpty();
        }

        /// <summary>
        /// Nested dialogs should not appear unless manually shown
        /// </summary>
        [Test]
        public void NestedInlineDialogTest()
        {
            var provider = Context.RenderComponent<MudDialogProvider>();
            provider.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // displaying the component with the inline dialog only renders the open button
            var comp = Context.RenderComponent<TestNestedInlineDialog>();
            comp.FindComponents<MudButton>().Count.Should().Be(1);
            // open the dialog
            comp.Find("button").Click();
            comp.WaitForAssertion(() =>
                provider.Find("div.mud-dialog-container").Should().NotBe(null)
            );
            provider.Find("p.mud-typography").TrimmedText().Should().Be("Scorpiany!");
            provider.FindComponents<MudText>().Count.Should().Be(2); //counts both the dialog header and the text in our test component

            provider.Find("button").Click(); //open nested dialog
            comp.WaitForAssertion(() =>
                provider.Find(".nested").Should().NotBe(null)
            );

            provider.FindAll("p.mud-typography")[1].TrimmedText().Should().Be("Nested dialog!");
            provider.FindComponents<MudText>().Count.Should().Be(4); //now we have another MudText
        }

        /// <summary>
        /// Click outside the dialog (or any other method) must update the Visible parameter two-way binding on close
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task InlineDialog_Should_UpdateIsVisibleOnClose()
        {
            await ImproveChanceOfSuccess(() =>
            {
                var comp = Context.RenderComponent<MudDialogProvider>();
                comp.Markup.Trim().Should().BeEmpty();
                var service = Context.Services.GetService<IDialogService>() as DialogService;
                service.Should().NotBe(null);
                // displaying the component with the inline dialog only renders the open button
                var comp1 = Context.RenderComponent<InlineDialogIsVisibleStateTest>();
                // open the dialog
                comp1.Find("button").Click();
                comp.WaitForAssertion(() => comp.Find("div.mud-dialog-container").Should().NotBe(null));
                // close by click outside
                comp.Find("div.mud-overlay").Click();
                comp.WaitForAssertion(() => comp.Markup.Trim().Should().BeEmpty(), TimeSpan.FromSeconds(5));
                // open again
                comp1.Find("button").Click();
                comp.WaitForAssertion(() => comp.Find("div.mud-dialog-container").Should().NotBe(null),
                    TimeSpan.FromSeconds(5));
                // close again by click outside
                comp.WaitForAssertion(() => comp.Find("div.mud-overlay").Should().NotBeNull());
                comp.Find("div.mud-overlay").Click();
                comp.WaitForAssertion(() => comp.Markup.Trim().Should().BeEmpty(), TimeSpan.FromSeconds(5));

                return Task.CompletedTask;
            });
        }

        /// <summary>
        /// Based on bug report #3128
        /// Dialog Class and Style parameters should be honored for inline dialog
        /// </summary>
        [Test]
        public async Task InlineDialogShouldHonorClassAndStyle()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<TestInlineDialog>());
            comp.WaitForAssertion(() => dialogReference.Should().NotBe(null));
            comp.Find("button").Click();
            comp.WaitForAssertion(() => comp.Find("div.mud-dialog").ClassList.Should().Contain("test-class"));
            comp.Find("div.mud-dialog").Attributes["style"].Value.Should().Be("color: red;");
            comp.Find("div.mud-dialog-content").Attributes["style"].Value.Should().Be("color: blue;");
            comp.Find("div.mud-dialog-content").ClassList.Should().NotContain("test-class");
            comp.Find("div.mud-dialog-content").ClassList.Should().Contain("content-class");
            // check if tag is ok
            var dialogInstance = comp.FindComponent<MudDialog>().Instance;
            dialogInstance.Tag.Should().Be("test-tag");
        }

        /// <summary>
        /// Based on bug report #3701 #3687
        /// Dialog inline should not be closed after any event inside
        /// </summary>
        [Test]
        public void InlineDialogShouldNotCloseAfterStateHasChanged()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // displaying the component with the inline dialog only renders the open button
            var comp1 = Context.RenderComponent<TestInlineDialog>();
            // open the dialog
            comp1.Find("button").Click();
            // rate star
            comp.Find("span.mud-rating-item").FirstElementChild.Click();
            // check if is still opened
            comp.WaitForAssertion(() => comp.Find("div.mud-dialog-container").Should().NotBe(null), TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Based on bug report by Porkopek:
        /// Updating values that are referenced in TitleContent render fragment won't result in an update of the dialog title
        /// when they change. This is solved by allowing the user to call ForceRender() on DialogInstance
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DialogShouldUpdateTitleContent()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogThatUpdatesItsTitle>());
            dialogReference.Should().NotBe(null);
            //comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Be("Body:");
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Be("Title:");
            // click on ok button should set title and content
            comp.FindAll("button")[1].Click();
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Be("Body: Test123");
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Be("Title: Test123");
        }

        /// <summary>
        /// A test that ensures parameters are not overwritten when dialog is updated
        /// </summary>
        [Test]
        public async Task DialogShouldNotOverwriteParameters()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;

            var parameters = new DialogParameters<DialogWithParameters>
            {
                { x => x.TestValue, "test" },
                { x => x.ColorTest, Color.Error } // !! comment me !!
            };

            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogWithParameters>(string.Empty, parameters));
            dialogReference.Should().NotBe(null);

            var textField = comp.FindComponent<MudInput<string>>().Instance;
            textField.Text.Should().Be("test");

            comp.Find("input").Change("new_test");
            comp.Find("input").Blur();
            textField.Text.Should().Be("new_test");

            comp.FindAll("button")[0].Click();

            ((DialogWithParameters)dialogReference.Dialog).TestValue.Should().Be("new_test");
            ((DialogWithParameters)dialogReference.Dialog).ParametersSetCounter.Should().Be(1);
            textField.Text.Should().Be("new_test");
        }

        /// <summary>
        /// Based on bug report #1385
        /// Dialog Class and Style parameters should be honored
        /// </summary>
        [Test]
        public async Task DialogShouldHonorClassAndStyle()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>());
            dialogReference.Should().NotBe(null);
            comp.Find("div.mud-dialog").ClassList.Should().Contain("test-class");
            comp.Find("div.mud-dialog").Attributes["style"].Value.Should().Be("color: red;");
            comp.Find("div.mud-dialog-content").Attributes["style"].Value.Should().Be("color: blue;");
            comp.Find("div.mud-dialog-content").ClassList.Should().NotContain("test-class");
            comp.Find("div.mud-dialog-content").ClassList.Should().Contain("content-class");
        }

        [Test]
        public void PassingEventCallbackToDialogViaParameters()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);

            var testComp = Context.RenderComponent<DialogWithEventCallbackTest>();
            // open dialog
            testComp.Find("button").Click();
            // in the opened dialog find the text field
            var tf = comp.FindComponent<MudTextField<string>>();
            tf.Find("input").Input("User input ...");
            // the user input should be passed out of the dialog into the outer component and displayed there.
            testComp.WaitForAssertion(() =>
                testComp.Find("p").TextContent.Trim().Should().Be("Search Text:  User input ...")
            );
        }

        [Test]
        public async Task CustomDialogService()
        {
            //Remove default IDialogService so we can provide our custom implementation
            //This is not necessary in normal cases, you would rather just not register all services in the beginning, but the test environment requires here to do so.
            Context.Services.Remove(Context.Services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IDialogService)));
            //Register our custom dialog service implementation as the new service instance behind IDialogService
            Context.Services.AddScoped<IDialogService>(sp => new CustomDialogService());

            //Render our dialog provider and make sure everything is fine
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();

            //Try to get the current service instance for the type IDialogService and make sure it is our custom implementation
            var service = Context.Services.GetService<IDialogService>();
            service.Should().NotBe(null);
            service.Should().BeAssignableTo(typeof(CustomDialogService));

            //Show the dialog, create reference and make sure it is our custom dialog reference implementation
            IDialogReference dialogReference = null;
            //The type of the dialog does not really matter, for the sake of laziness I will re-use this existing one
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogThatUpdatesItsTitle>());
            dialogReference.Should().NotBe(null);
            dialogReference.Should().BeAssignableTo(typeof(CustomDialogReference));

            //After above checks have passed, we can safely cast the generic reference into our specific implementation  
            var customDialogReference = (CustomDialogReference)dialogReference;
            //The custom property should be false by default otherwise the rest of the test logic would be incorrect
            customDialogReference.AllowDismiss.Should().BeFalse();

            //Dialog should not be closable through backdrop click
            comp.Find("div.mud-overlay").Click();
            comp.WaitForAssertion(() => comp.Markup.Trim().Should().NotBeEmpty(), TimeSpan.FromSeconds(5));

            //Allow dismiss
            customDialogReference.AllowDismiss = true;

            //Dialog should now be closable through backdrop click
            comp.Find("div.mud-overlay").Click();
            comp.WaitForAssertion(() => comp.Markup.Trim().Should().BeEmpty(), TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Getting return value from dialog
        /// </summary>
        [Test]
        public async Task DialogShouldReturnTheReturnValue()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;
            // open dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogWithReturnValue>());
            dialogReference.Should().NotBe(null);
            // close by click on cancel button
            comp.FindAll("button")[0].Click();
            var rv = await dialogReference.GetReturnValueAsync<string>();
            rv.Should().BeNull();
            // open dialog
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogWithReturnValue>());
            // close by click on ok button
            comp.FindAll("button")[1].Click();
            rv = await dialogReference.GetReturnValueAsync<string>();
            rv.Should().Be("Closed via OK");
        }

        [Test]
        public async Task DialogKeyboardNavigation()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;
            //dialog with clickable backdrop
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>(string.Empty, new DialogOptions() { CloseOnEscapeKey = true }));
            dialogReference.Should().NotBe(null);
            var dialog1 = (DialogOkCancel)dialogReference.Dialog;
            comp.Markup.Trim().Should().NotBeEmpty();
            await comp.InvokeAsync(() => dialog1.MudDialog.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.Markup.Trim().Should().BeEmpty();
            //dialog with disabled backdrop click
            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogOkCancel>(string.Empty, new DialogOptions() { CloseOnEscapeKey = false }));
            dialogReference.Should().NotBe(null);
            var dialog2 = (DialogOkCancel)dialogReference.Dialog;
            comp.Markup.Trim().Should().NotBeEmpty();
            await comp.InvokeAsync(() => dialog2.MudDialog.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.Markup.Trim().Should().NotBeEmpty();
        }

        [Test]
        public async Task DialogHandlesOnBackdropClickEvent()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();

            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;

            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogWithOnBackdropClickEvent>());
            dialogReference.Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Be("Title:");

            //Click on backdrop
            comp.Find("div.mud-overlay").Click();

            comp.Find("div.mud-dialog-title").TrimmedText().Should().Be("Title: Backdrop clicked");
        }

        /// <summary>
        /// Open Inline Dialog and the from the inline dialog another normal dialog
        /// while closing the inline dialog.
        /// https://github.com/MudBlazor/MudBlazor/issues/4871
        /// </summary>
        [Test]
        public void InlineDialogBug4871Test()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            // displaying the component with the inline dialog only renders the open button
            var comp1 = Context.RenderComponent<TestInlineDialog>();
            comp1.FindComponents<MudButton>().Count.Should().Be(1);
            // open the dialog
            comp1.Find("button").Click();
            comp1.WaitForAssertion(() =>
                comp.Find("div.mud-dialog-container").Should().NotBe(null)
            );
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Wabalabadubdub!");
            comp.Find("div.mud-dialog").GetAttribute("class").Should().Contain("mud-dialog-width-full");
            // close by click on ok button
            comp.FindAll("button").Last().Click();
            comp.WaitForAssertion(() => comp.FindComponent<MudMessageBox>());
            var messageBox = comp.FindComponent<MudMessageBox>();
            messageBox.Should().NotBeNull();
            messageBox.Instance.YesText.Should().Be("BUG4871");
        }

        [Test]
        public async Task DialogToggleFullscreenOptions()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();

            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;

            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogToggleFullscreen>());
            dialogReference.Should().NotBe(null);

            comp.Find("div.mud-dialog").GetAttribute("class").Should().Be("mud-dialog mud-dialog-width-sm");
            comp.Find("button").Click();
            comp.Find("div.mud-dialog").GetAttribute("class").Should().Be("mud-dialog mud-dialog-fullscreen");
            comp.Find("button").Click();
            comp.Find("div.mud-dialog").GetAttribute("class").Should().Be("mud-dialog mud-dialog-width-sm");
        }

        ///-------------------------------
        /// <summary>
        /// Testing lifecycle of dialogs in dialogprovider
        /// </summary>
        [Test]
        public async Task AsyncLifecycleTest()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            await comp.InvokeAsync(async () =>
            {
                var dialogReference = await service!.ShowAsync<DialogRender>();
                await dialogReference.Result;
                //The second Dialog is added here, but the first dialog is still in the _dialogs collection of the dialogprovider, as only the result task was set to completion.
                //So DialogProvider will render again with 2 dialogs, but 1 is completed. This one needs to be excluded from rendering to prevent double initialize with no params.
                dialogReference = await service.ShowAsync<DialogRender>();
                await dialogReference.Result;
            });
            DialogRender.OnInitializedCount.Should().Be(2);
            //Reset global value
            DialogRender.OnInitializedCount = 0;
        }

        /// <summary>
        /// Opening and closing a simple dialog
        /// </summary>
        [Test]
        public async Task AsyncSimpleTest()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>());
            //open simple test dialog
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);
            comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("p.mud-typography").TrimmedText().Should().Be("Wabalabadubdub!");
            //close by click outside the dialog
            comp.Find("div.mud-overlay").Click();
            comp.Markup.Trim().Should().BeEmpty();
            var result = await dialogReference.Result;
            result.Canceled.Should().BeTrue();
            //open simple test dialog
            dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>());
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            dialogReference = await dialogReferenceLazy.Value;
            //close by click on cancel button
            comp.FindAll("button")[0].Click();
            result = await dialogReference.Result;
            result.Canceled.Should().BeTrue();
            //open simple test dialog
            dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>());
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            dialogReference = await dialogReferenceLazy.Value;
            //close by click on ok button
            comp.FindAll("button")[1].Click();
            result = result = await dialogReference.Result;
            result.Canceled.Should().BeFalse();

            //create 2 instances and dismiss all
            dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>());
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>());
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var cont = comp.FindAll("div.mud-dialog-container");
            cont.Count.Should().Be(2);
            await comp.InvokeAsync(() => comp.Instance.DismissAll());
            cont = comp.FindAll("div.mud-dialog-container");
            cont.Count.Should().Be(0);
        }

        /// <summary>
        /// Based on bug report #3128
        /// Dialog Class and Style parameters should be honored for inline dialog
        /// </summary>
        [Test]
        public async Task InlineAsyncDialogShouldHonorClassAndStyle()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<TestInlineDialog>());
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            comp.WaitForAssertion(() => dialogReference.Should().NotBe(null));
            comp.Find("button").Click();
            comp.WaitForAssertion(() => comp.Find("div.mud-dialog").ClassList.Should().Contain("test-class"));
            comp.Find("div.mud-dialog").Attributes["style"].Value.Should().Be("color: red;");
            comp.Find("div.mud-dialog-content").Attributes["style"].Value.Should().Be("color: blue;");
            comp.Find("div.mud-dialog-content").ClassList.Should().NotContain("test-class");
            comp.Find("div.mud-dialog-content").ClassList.Should().Contain("content-class");
            // check if tag is ok
            var dialogInstance = comp.FindComponent<MudDialog>().Instance;
            dialogInstance.Tag.Should().Be("test-tag");
        }

        /// <summary>
        /// Based on bug report by Porkopek:
        /// Updating values that are referenced in TitleContent render fragment won't result in an update of the dialog title
        /// when they change. This is solved by allowing the user to call ForceRender() on DialogInstance
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task AsyncDialogShouldUpdateTitleContent()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogThatUpdatesItsTitle>());
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);
            //comp.Find("div.mud-dialog-container").Should().NotBe(null);
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Be("Body:");
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Be("Title:");
            // click on ok button should set title and content
            comp.FindAll("button")[1].Click();
            comp.Find("div.mud-dialog-content").TrimmedText().Should().Be("Body: Test123");
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Be("Title: Test123");
        }

        /// <summary>
        /// A test that ensures parameters are not overwritten when dialog is updated
        /// </summary>
        [Test]
        public async Task AsyncDialogShouldNotOverwriteParameters()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);

            var parameters = new DialogParameters<DialogWithParameters>
            {
                { x => x.TestValue, "test" },
                { x => x.ColorTest, Color.Error } // !! comment me !!
            };

            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogWithParameters>(string.Empty, parameters));
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);

            var textField = comp.FindComponent<MudInput<string>>().Instance;
            textField.Text.Should().Be("test");

            comp.Find("input").Change("new_test");
            comp.Find("input").Blur();
            textField.Text.Should().Be("new_test");

            comp.FindAll("button")[0].Click();

            ((DialogWithParameters)dialogReference.Dialog).TestValue.Should().Be("new_test");
            ((DialogWithParameters)dialogReference.Dialog).ParametersSetCounter.Should().Be(1);
            textField.Text.Should().Be("new_test");
        }

        /// <summary>
        /// Based on bug report #1385
        /// Dialog Class and Style parameters should be honored
        /// </summary>
        [Test]
        public async Task AsyncDialogShouldHonorClassAndStyle()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>());
            // open simple test dialog
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);
            comp.Find("div.mud-dialog").ClassList.Should().Contain("test-class");
            comp.Find("div.mud-dialog").Attributes["style"].Value.Should().Be("color: red;");
            comp.Find("div.mud-dialog-content").Attributes["style"].Value.Should().Be("color: blue;");
            comp.Find("div.mud-dialog-content").ClassList.Should().NotContain("test-class");
            comp.Find("div.mud-dialog-content").ClassList.Should().Contain("content-class");
        }

        [Test]
        public async Task AsyncCustomDialogService()
        {
            //Remove default IDialogService so we can provide our custom implementation
            //This is not necessary in normal cases, you would rather just not register all services in the beginning, but the test environment requires here to do so.
            Context.Services.Remove(Context.Services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IDialogService)));
            //Register our custom dialog service implementation as the new service instance behind IDialogService
            Context.Services.AddScoped<IDialogService>(sp => new CustomDialogService());

            //Render our dialog provider and make sure everything is fine
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();

            //Try to get the current service instance for the type IDialogService and make sure it is our custom implementation
            var service = Context.Services.GetService<IDialogService>();
            service.Should().NotBe(null);
            service.Should().BeAssignableTo(typeof(CustomDialogService));

            //Show the dialog, create reference and make sure it is our custom dialog reference implementation
            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogThatUpdatesItsTitle>());
            //The type of the dialog does not really matter, for the sake of laziness I will re-use this existing one
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);
            dialogReference.Should().BeAssignableTo(typeof(CustomDialogReference));

            //After above checks have passed, we can safely cast the generic reference into our specific implementation  
            var customDialogReference = (CustomDialogReference)dialogReference;
            //The custom property should be false by default otherwise the rest of the test logic would be incorrect
            customDialogReference.AllowDismiss.Should().BeFalse();

            //Dialog should not be closable through backdrop click
            comp.Find("div.mud-overlay").Click();
            comp.WaitForAssertion(() => comp.Markup.Trim().Should().NotBeEmpty(), TimeSpan.FromSeconds(5));

            //Allow dismiss
            customDialogReference.AllowDismiss = true;

            //Dialog should now be closable through backdrop click
            comp.Find("div.mud-overlay").Click();
            comp.WaitForAssertion(() => comp.Markup.Trim().Should().BeEmpty(), TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Getting return value from dialog
        /// </summary>
        [Test]
        public async Task AsyncDialogShouldReturnTheReturnValue()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogWithReturnValue>());
            // open dialog
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);
            // close by click on cancel button
            comp.FindAll("button")[0].Click();
            var rv = await dialogReference.GetReturnValueAsync<string>();
            rv.Should().BeNull();
            // open dialog
            dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogWithReturnValue>());
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            // close by click on ok button
            comp.FindAll("button")[1].Click();
            dialogReference = await dialogReferenceLazy.Value;
            rv = await dialogReference.GetReturnValueAsync<string>();
            rv.Should().Be("Closed via OK");
        }

        [Test]
        public async Task AsyncDialogKeyboardNavigation()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>(string.Empty, new DialogOptions() { CloseOnEscapeKey = true }));
            //dialog with clickable backdrop
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            dialogReferenceLazy.Value.Should().NotBe(null);
            var dialogReference = await dialogReferenceLazy.Value;
            var dialog1 = (DialogOkCancel)dialogReference.Dialog;
            comp.Markup.Trim().Should().NotBeEmpty();
            await comp.InvokeAsync(() => dialog1.MudDialog.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.Markup.Trim().Should().BeEmpty();
            //dialog with disabled backdrop click
            dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>(string.Empty, new DialogOptions() { CloseOnEscapeKey = false }));
            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);
            var dialog2 = (DialogOkCancel)dialogReference.Dialog;
            comp.Markup.Trim().Should().NotBeEmpty();
            await comp.InvokeAsync(() => dialog2.MudDialog.HandleKeyDown(new KeyboardEventArgs() { Key = "Escape", Type = "keydown", }));
            comp.Markup.Trim().Should().NotBeEmpty();
        }

        [Test]
        public async Task AsyncDialogHandlesOnBackdropClickEvent()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();

            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);

            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogWithOnBackdropClickEvent>());

            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);
            comp.Find("div.mud-dialog-title").TrimmedText().Should().Be("Title:");

            //Click on backdrop
            comp.Find("div.mud-overlay").Click();

            comp.Find("div.mud-dialog-title").TrimmedText().Should().Be("Title: Backdrop clicked");
        }

        [Test]
        public async Task AsyncDialogToggleFullscreenOptions()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();

            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogToggleFullscreen>());

            await comp.InvokeAsync(() => dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;
            dialogReference.Should().NotBe(null);

            comp.Find("div.mud-dialog").GetAttribute("class").Should().Be("mud-dialog mud-dialog-width-sm");
            comp.Find("button").Click();
            comp.Find("div.mud-dialog").GetAttribute("class").Should().Be("mud-dialog mud-dialog-fullscreen");
            comp.Find("button").Click();
            comp.Find("div.mud-dialog").GetAttribute("class").Should().Be("mud-dialog mud-dialog-width-sm");
        }

        [Test]
        public async Task ShowAsyncRenderCompleteTest()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();

            var service = Context.Services.GetService<IDialogService>();

            var dialogReferenceLazy = new Lazy<Task<IDialogReference>>(() => service?.ShowAsync<DialogOkCancel>());

            await comp.InvokeAsync(async () => await dialogReferenceLazy.Value);
            var dialogReference = await dialogReferenceLazy.Value;

            var result = await dialogReference.RenderCompleteTaskCompletionSource.Task;

            result.Should().Be(true);
        }

        [Test]
        public void MockIDialogReferenceShouldWork()
        {
            Func<IDialogReference> createMock = Moq.Mock.Of<IDialogReference>;
            createMock.Should().NotThrow();
        }

        [Test]
        public async Task AsyncDialogParametersGenericShouldPassParameters()
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBe(null);
            IDialogReference dialogReference = null;

            var parameters = new DialogParameters<DialogWithParameters>
            {
                { x => x.TestValue, "test" },
                { x => x.ColorTest, Color.Error }
            };

            await comp.InvokeAsync(() => dialogReference = service?.Show<DialogWithParameters>(string.Empty, parameters));
            dialogReference.Should().NotBe(null);

            var textField = comp.FindComponent<MudInput<string>>().Instance;
            textField.Text.Should().Be("test");
        }

        [Test]
        public async Task ShowGeneric_ShouldProvideDefaultOptions_WhenOverloadIsCalled()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.Show<DialogOkCancel>("Custom title"));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Markup.Should().Contain("Custom title");
            dialogInstance.Instance.Options.Position.Should().BeNull();
            dialogInstance.Instance.Options.MaxWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackdropClick.Should().BeNull();
            dialogInstance.Instance.Options.CloseOnEscapeKey.Should().BeNull();
            dialogInstance.Instance.Options.NoHeader.Should().BeNull();
            dialogInstance.Instance.Options.CloseButton.Should().BeNull();
            dialogInstance.Instance.Options.FullWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackgroundClass.Should().BeNull();
        }

        [Test]
        public async Task ShowGeneric_ShouldProvideCorrectOptions_WhenOverloadIsCalled()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.Show<DialogOkCancel>("Custom title", new DialogOptions { CloseButton = true }));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Instance.Options.CloseButton.Should().BeTrue();
        }

        [Test]
        public async Task Show_ShouldRenderComponent()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.Show(typeof(DialogOkCancel)));

            // Assert
            provider.FindComponents<DialogOkCancel>().Should().HaveCount(1);
        }

        [Test]
        public async Task Show_ShouldProvideDefaultOptions_WhenOverloadIsCalled()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.Show(typeof(DialogOkCancel), "Custom title"));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Markup.Should().Contain("Custom title");
            dialogInstance.Instance.Options.Position.Should().BeNull();
            dialogInstance.Instance.Options.MaxWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackdropClick.Should().BeNull();
            dialogInstance.Instance.Options.CloseOnEscapeKey.Should().BeNull();
            dialogInstance.Instance.Options.NoHeader.Should().BeNull();
            dialogInstance.Instance.Options.CloseButton.Should().BeNull();
            dialogInstance.Instance.Options.FullWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackgroundClass.Should().BeNull();
        }

        [Test]
        public async Task Show_ShouldProvideCorrectOptions_WhenOverloadIsCalled()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.Show(typeof(DialogOkCancel), "Custom title", new DialogOptions { CloseButton = true }));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Instance.Options.CloseButton.Should().BeTrue();
        }

        [Test]
        public async Task Show_ShouldPassDialogParametersToDialog()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() =>
                service.Show(typeof(DialogWithActionsClass), "Custom title", new DialogParameters<DialogWithActionsClass> { { x => x.ActionsClass, "custom-class" } }));

            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.FindAll(".custom-class").Should().HaveCount(1);
        }

        [Test]
        public async Task ShowGenericAsync_ShouldProvideDefaultOptions_WhenOverloadIsCalled()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.ShowAsync<DialogOkCancel>("Custom title"));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Markup.Should().Contain("Custom title");
            dialogInstance.Instance.Options.Position.Should().BeNull();
            dialogInstance.Instance.Options.MaxWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackdropClick.Should().BeNull();
            dialogInstance.Instance.Options.CloseOnEscapeKey.Should().BeNull();
            dialogInstance.Instance.Options.NoHeader.Should().BeNull();
            dialogInstance.Instance.Options.CloseButton.Should().BeNull();
            dialogInstance.Instance.Options.FullWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackgroundClass.Should().BeNull();
        }

        [Test]
        public async Task ShowGenericAsync_ShouldProvideCorrectOptions_WhenOverloadIsCalled()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.ShowAsync<DialogOkCancel>("Custom title", new DialogOptions { CloseButton = true }));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Instance.Options.CloseButton.Should().BeTrue();
        }

        [Test]
        public async Task Close_ShouldCloseDialogAndInvokeOnDialogCloseRequested()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;
            var reference = (DialogReference)service.CreateReference();
            var invoked = false;
            Type type = null;
            service.OnDialogCloseRequested += (_, result) =>
            {
                invoked = true;
                type = result.DataType;
            };

            // Act 
            await provider.InvokeAsync(() => service.Close(reference));

            // Assert
            invoked.Should().BeTrue();
            type.Should().BeNull();
        }

        [Test]
        public async Task ShowAsync_FromNonUiThreadShouldAddInstance()
        {
            // Arrange
            var service = Context.Services.GetRequiredService<IDialogService>();
            _ = Context.RenderComponent<MudDialogProvider>();
            var invoked = false;
            service.DialogInstanceAddedAsync += _ =>
            {
                invoked = true;

                return Task.CompletedTask;
            };

            // Act 
            await Task.Factory.StartNew(() => service.ShowAsync(typeof(DialogOkCancel))).ConfigureAwait(false);

            // Assert
            invoked.Should().BeTrue();
        }

        [Test]
        public async Task ShowAsync_ShouldProvideDefaultOption()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.ShowAsync(typeof(DialogOkCancel)));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Instance.Options.Position.Should().BeNull();
            dialogInstance.Instance.Options.MaxWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackdropClick.Should().BeNull();
            dialogInstance.Instance.Options.CloseOnEscapeKey.Should().BeNull();
            dialogInstance.Instance.Options.NoHeader.Should().BeNull();
            dialogInstance.Instance.Options.CloseButton.Should().BeNull();
            dialogInstance.Instance.Options.FullWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackgroundClass.Should().BeNull();
        }

        [Test]
        public async Task ShowAsync_ShouldProvideDefaultOptions_WhenOverloadIsCalled()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.ShowAsync(typeof(DialogOkCancel), "Custom title"));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Markup.Should().Contain("Custom title");
            dialogInstance.Instance.Options.Position.Should().BeNull();
            dialogInstance.Instance.Options.MaxWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackdropClick.Should().BeNull();
            dialogInstance.Instance.Options.CloseOnEscapeKey.Should().BeNull();
            dialogInstance.Instance.Options.NoHeader.Should().BeNull();
            dialogInstance.Instance.Options.CloseButton.Should().BeNull();
            dialogInstance.Instance.Options.FullWidth.Should().BeNull();
            dialogInstance.Instance.Options.BackgroundClass.Should().BeNull();
        }

        [Test]
        public async Task ShowAsync_ShouldProvideCorrectOptions_WhenOverloadIsCalled()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() => service.ShowAsync(typeof(DialogOkCancel), "Custom title", new DialogOptions { CloseButton = true }));
            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.Instance.Options.CloseButton.Should().BeTrue();
        }

        [Test]
        public async Task ShowAsync_ShouldPassDialogParametersToDialog()
        {
            // Arrange
            var provider = Context.RenderComponent<MudDialogProvider>();
            var service = (Context.Services.GetRequiredService<IDialogService>() as DialogService)!;

            // Act 
            await provider.InvokeAsync(() =>
                service.ShowAsync(typeof(DialogWithActionsClass), "Custom title", new DialogParameters<DialogWithActionsClass> { { x => x.ActionsClass, "custom-class" } }));

            var dialogInstance = provider.FindComponent<MudDialogInstance>();

            // Assert
            dialogInstance.FindAll(".custom-class").Should().HaveCount(1);
        }

        [TestCase("", false, "mud-dialog-content")]
        [TestCase("", true, "mud-dialog-content mud-dialog-no-side-padding")]
        [TestCase("my-class", false, "mud-dialog-content my-class")]
        [TestCase("my-class", true, "mud-dialog-content mud-dialog-no-side-padding my-class")]
        public async Task DialogWithContentClassValueShouldRenderExpectedClassname(string contentClass, bool disablePadding, string expectedClassname)
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBeNull();
            IDialogReference dialogReference = null;

            var parameters = new DialogParameters<DialogWithContentClass>
            {
                { x => x.ContentClass, contentClass },
                { x => x.Gutters, !disablePadding }
            };

            await comp.InvokeAsync(async () => dialogReference = await service!.ShowAsync<DialogWithContentClass>(string.Empty, parameters));
            dialogReference.Should().NotBeNull();

            comp.Find("div.mud-dialog-content").GetAttribute("class").Should().Be(expectedClassname);
        }

        [TestCase("", "mud-dialog-actions")]
        [TestCase("my-class", "mud-dialog-actions my-class")]
        public async Task DialogWithActionsClassValueShouldRenderExpectedClassname(string actionsClass, string expectedClassname)
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBeNull();
            IDialogReference dialogReference = null;

            var parameters = new DialogParameters<DialogWithActionsClass>
            {
                { x => x.ActionsClass, actionsClass }
            };

            await comp.InvokeAsync(async () => dialogReference = await service!.ShowAsync<DialogWithActionsClass>(string.Empty, parameters));
            dialogReference.Should().NotBeNull();

            comp.Find("div.mud-dialog-actions").GetAttribute("class").Should().Be(expectedClassname);
        }

        [TestCase("", "mud-dialog-title")]
        [TestCase("my-title-class my-second-class", "mud-dialog-title my-title-class my-second-class")]
        public async Task DialogWithTitleClassValueShouldRenderExpectedClassname(string titleClass, string expectedClassname)
        {
            var comp = Context.RenderComponent<MudDialogProvider>();
            comp.Markup.Trim().Should().BeEmpty();
            var service = Context.Services.GetService<IDialogService>() as DialogService;
            service.Should().NotBeNull();
            IDialogReference dialogReference = null;

            var parameters = new DialogParameters<DialogWithTitleClass>
            {
                { x => x.TitleClass, titleClass }
            };

            await comp.InvokeAsync(async () => dialogReference = await service!.ShowAsync<DialogWithTitleClass>(string.Empty, parameters));
            dialogReference.Should().NotBeNull();

            comp.Find("div.mud-dialog-title").GetAttribute("class").Should().Be(expectedClassname);
        }
    }

    internal class CustomDialogService : DialogService
    {
        public override IDialogReference CreateReference() => new CustomDialogReference(Guid.NewGuid(), this);
    }

    internal class CustomDialogReference : DialogReference
    {
        public bool AllowDismiss { get; set; }

        public CustomDialogReference(Guid dialogInstanceId, IDialogService dialogService) : base(dialogInstanceId, dialogService) { }

        public override bool Dismiss(DialogResult result)
        {
            return AllowDismiss;
        }
    }
}
