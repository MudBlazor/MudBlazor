// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.Resources;
using NUnit.Framework;

#nullable enable

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class ExitPromptTests : BunitTest
{
    [Test]
    public void RegistersOnFirstRender_WhenNotDisabled()
    {
        // Arrange
        var js = UseJsInteropRecorder();

        // Act
        RenderExitPrompt();

        // Assert
        js.EnabledPromptIds.Should().HaveCount(1);
        js.EnabledPromptIds.Single().Should().NotBeNullOrWhiteSpace();
        js.DisabledPromptIds.Should().BeEmpty();
    }

    [Test]
    public void DoesNotRegisterOnFirstRender_WhenDisabled()
    {
        // Arrange
        var js = UseJsInteropRecorder();

        // Act
        RenderExitPrompt(disabled: true);

        // Assert
        js.EnabledPromptIds.Should().BeEmpty();
        js.DisabledPromptIds.Should().BeEmpty();
    }

    [Test]
    public async Task DisabledFirstDispose_DoesNotCallDisableInterop()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        RenderExitPrompt(disabled: true);

        // Act
        await Context.DisposeComponentsAsync();

        // Assert
        js.EnabledPromptIds.Should().BeEmpty();
        js.DisabledPromptIds.Should().BeEmpty();
    }

    [Test]
    public async Task DisposesAndUnregistersPrompt()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        RenderExitPrompt();
        var promptId = js.EnabledPromptIds.Single();

        // Act
        await Context.DisposeComponentsAsync();

        // Assert
        js.DisabledPromptIds.Should().ContainSingle().Which.Should().Be(promptId);
    }

    [Test]
    public async Task DisabledToggle_UnregistersThenRegistersSamePrompt()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var component = RenderExitPrompt();
        var promptId = js.EnabledPromptIds.Single();

        // Act
        await component.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, true));
        await component.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, false));

        // Assert
        js.DisabledPromptIds.Should().ContainSingle().Which.Should().Be(promptId);
        js.EnabledPromptIds.Should().HaveCount(2);
        js.EnabledPromptIds.Last().Should().Be(promptId);
    }

    [Test]
    public async Task TextChange_UpdatesInteropTextForPrompt()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var component = RenderExitPrompt(text: "Initial text");
        var promptId = js.EnabledPromptIds.Single();
        var initialTextUpdateCount = js.TextUpdates.Count;

        // Act
        await component.SetParametersAndRenderAsync(p => p.Add(x => x.Text, "Updated text"));

        // Assert
        js.TextUpdates.Should().HaveCount(initialTextUpdateCount + 1);
        js.TextUpdates.Last().Should().Be((promptId, "Updated text"));
    }

    [Test]
    public async Task TextUpdateWhileDisabled_DoesNotCallSetTextInterop()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var component = RenderExitPrompt(disabled: true, text: "Initial text");

        // Act
        await component.SetParametersAndRenderAsync(p => p
            .Add(x => x.Disabled, true)
            .Add(x => x.Text, "Updated while disabled"));

        // Assert
        js.EnabledPromptIds.Should().BeEmpty();
        js.TextUpdates.Should().BeEmpty();
    }

    [Test]
    public async Task TextUpdateWhileUnregistered_DoesNotCallSetTextInterop()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var component = RenderExitPrompt(text: "Initial text");
        var promptId = js.EnabledPromptIds.Single();

        // Act
        await component.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, true));
        var textUpdateCount = js.TextUpdates.Count;
        await component.SetParametersAndRenderAsync(p => p.Add(x => x.Text, "Updated while unregistered"));

        // Assert
        js.DisabledPromptIds.Should().ContainSingle().Which.Should().Be(promptId);
        js.TextUpdates.Should().HaveCount(textUpdateCount);
    }

    [Test]
    public async Task NativePrompt_NavigationCallsHandleBeforeNavigation()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var component = RenderExitPrompt(useNativePrompt: true);
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();
        var promptId = js.EnabledPromptIds.Single();

        // Act
        await component.InvokeAsync(() => navigationManager.NavigateTo("/native-navigation"));

        // Assert
        js.NativeNavigationChecks.Should().ContainSingle().Which.Should().Be(promptId);
    }

    [Test]
    public async Task DialogPrompt_NavigationUsesDialogServiceAndSkipsNativeInterop()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var dialogServiceMock = ReplaceDialogServiceWithMock(true);
        var component = RenderExitPrompt(useNativePrompt: false);
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

        // Act
        await component.InvokeAsync(() => navigationManager.NavigateTo("/dialog-navigation"));

        // Assert
        dialogServiceMock.Verify(x => x.ShowMessageBoxAsync(
            It.IsAny<string?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<DialogOptions?>()), Times.Once);
        js.NativeNavigationChecks.Should().BeEmpty();
    }

    [Test]
    public async Task DialogPrompt_BlocksNavigationWhenDialogReturnsFalse()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var dialogServiceMock = ReplaceDialogServiceWithMock(false);
        var component = RenderExitPrompt(useNativePrompt: false);
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();
        var currentUri = navigationManager.Uri;

        // Act
        await component.InvokeAsync(() => navigationManager.NavigateTo("/dialog-blocked-false"));

        // Assert
        navigationManager.Uri.Should().Be(currentUri);
        dialogServiceMock.Verify(x => x.ShowMessageBoxAsync(
            It.IsAny<string?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<DialogOptions?>()), Times.Once);
        js.NativeNavigationChecks.Should().BeEmpty();
    }

    [Test]
    public async Task DialogPrompt_BlocksNavigationWhenDialogReturnsNull()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var dialogServiceMock = ReplaceDialogServiceWithMock(null);
        var component = RenderExitPrompt(useNativePrompt: false);
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();
        var currentUri = navigationManager.Uri;

        // Act
        await component.InvokeAsync(() => navigationManager.NavigateTo("/dialog-blocked-null"));

        // Assert
        navigationManager.Uri.Should().Be(currentUri);
        dialogServiceMock.Verify(x => x.ShowMessageBoxAsync(
            It.IsAny<string?>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string?>(),
            It.IsAny<string?>(),
            It.IsAny<DialogOptions?>()), Times.Once);
        js.NativeNavigationChecks.Should().BeEmpty();
    }

    [Test]
    public async Task DialogPrompt_UsesLocalizedDefaultsWhenTitleAndTextAreNotProvided()
    {
        // Arrange
        UseJsInteropRecorder();
        var dialogServiceMock = ReplaceDialogServiceWithMock(true);
        var component = RenderExitPrompt(useNativePrompt: false);
        var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

        // Act
        await component.InvokeAsync(() => navigationManager.NavigateTo("/default-dialog-text"));

        // Assert
        VerifyDialogMessage(
            dialogServiceMock,
            localizer[LanguageResource.MudExitPrompt_Title],
            localizer[LanguageResource.MudExitPrompt_Text],
            localizer[LanguageResource.MudExitPrompt_Exit],
            localizer[LanguageResource.MudExitPrompt_Cancel]);
    }

    [Test]
    public async Task DialogPrompt_UsesCustomTitleAndTextWhenProvided()
    {
        // Arrange
        const string Title = "Unsaved profile";
        const string Text = "Leave profile edit page?";

        UseJsInteropRecorder();
        var dialogServiceMock = ReplaceDialogServiceWithMock(true);
        var component = RenderExitPrompt(useNativePrompt: false, title: Title, text: Text);
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

        // Act
        await component.InvokeAsync(() => navigationManager.NavigateTo("/custom-dialog-text"));

        // Assert
        var localizer = Context.Services.GetRequiredService<InternalMudLocalizer>();
        VerifyDialogMessage(
            dialogServiceMock,
            Title,
            Text,
            localizer[LanguageResource.MudExitPrompt_Exit],
            localizer[LanguageResource.MudExitPrompt_Cancel]);
    }

    [Test]
    public async Task MultipleInstances_DisablingOneStillAllowsOtherToHandleNavigation()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var first = RenderExitPrompt(useNativePrompt: true, text: "First prompt");
        var second = RenderExitPrompt(useNativePrompt: true, text: "Second prompt");

        var firstPromptId = js.EnabledPromptIds[0];
        var secondPromptId = js.EnabledPromptIds[1];
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

        // Act
        await first.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, true));
        await second.InvokeAsync(() => navigationManager.NavigateTo("/two-prompts"));

        // Assert
        firstPromptId.Should().NotBe(secondPromptId);
        js.DisabledPromptIds.Should().Contain(firstPromptId);
        js.NativeNavigationChecks.Should().ContainSingle().Which.Should().Be(secondPromptId);
    }

    [Test]
    public async Task MultipleInstances_DisposingOneStillAllowsOtherToHandleNavigation()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var first = RenderExitPrompt(useNativePrompt: true, text: "First prompt");
        var second = RenderExitPrompt(useNativePrompt: true, text: "Second prompt");

        var firstPromptId = js.EnabledPromptIds[0];
        var secondPromptId = js.EnabledPromptIds[1];
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

        // Act
        await first.InvokeAsync(async () => await first.Instance.DisposeAsync());
        await second.InvokeAsync(() => navigationManager.NavigateTo("/two-prompts-dispose"));

        // Assert
        js.DisabledPromptIds.Should().Contain(firstPromptId);
        js.NativeNavigationChecks.Should().ContainSingle().Which.Should().Be(secondPromptId);
    }

    [Test]
    public async Task MultipleInstances_DisposeOrder_DisablesMatchingPromptIds()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var first = RenderExitPrompt(useNativePrompt: true, text: "First prompt");
        var second = RenderExitPrompt(useNativePrompt: true, text: "Second prompt");

        var firstPromptId = js.EnabledPromptIds[0];
        var secondPromptId = js.EnabledPromptIds[1];

        // Act
        await second.InvokeAsync(async () => await second.Instance.DisposeAsync());
        await first.InvokeAsync(async () => await first.Instance.DisposeAsync());

        // Assert
        js.DisabledPromptIds.Should().Equal(secondPromptId, firstPromptId);
    }

    [Test]
    public async Task DisposedPrompt_DoesNotIssueFurtherNativeNavigationChecks()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        var activePrompt = RenderExitPrompt(useNativePrompt: true, text: "Active prompt");
        var promptId = js.EnabledPromptIds.Single();
        var driver = RenderExitPrompt(disabled: true);
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();

        // Act
        await activePrompt.InvokeAsync(async () => await activePrompt.Instance.DisposeAsync());
        await driver.InvokeAsync(() => navigationManager.NavigateTo("/after-dispose"));

        // Assert
        js.DisabledPromptIds.Should().Contain(promptId);
        js.NativeNavigationChecks.Should().BeEmpty();
    }

    [Test]
    public async Task NativePrompt_BlocksNavigationWhenInteropReturnsFalse()
    {
        // Arrange
        var js = UseJsInteropRecorder();
        js.NativeNavigationResult = false;
        var component = RenderExitPrompt(useNativePrompt: true);
        var navigationManager = Context.Services.GetRequiredService<NavigationManager>();
        var currentUri = navigationManager.Uri;

        // Act
        await component.InvokeAsync(() => navigationManager.NavigateTo("/blocked-navigation"));

        // Assert
        navigationManager.Uri.Should().Be(currentUri);
        js.NativeNavigationChecks.Should().HaveCount(1);
    }

    /// <summary>
    /// Centralizes prompt rendering so test setup changes are made in one place.
    /// </summary>
    private IRenderedComponent<MudExitPrompt> RenderExitPrompt(
        bool disabled = false,
        bool useNativePrompt = false,
        string? title = null,
        string? text = null)
    {
        return Context.Render<MudExitPrompt>(parameters =>
        {
            parameters.Add(x => x.Disabled, disabled);
            parameters.Add(x => x.UseNativePrompt, useNativePrompt);
            if (title is not null)
            {
                parameters.Add(x => x.Title, title);
            }

            if (text is not null)
            {
                parameters.Add(x => x.Text, text);
            }
        });
    }

    /// <summary>
    /// Installs a shared interop spy so tests can assert behavior without repeating mock plumbing.
    /// </summary>
    private JsInteropRecorder UseJsInteropRecorder()
    {
        var recorder = new JsInteropRecorder();
        Context.Services.AddSingleton(recorder.JsRuntimeMock.Object);
        return recorder;
    }

    /// <summary>
    /// Replaces the dialog service to isolate ExitPrompt behavior from dialog infrastructure details.
    /// </summary>
    private Mock<IDialogService> ReplaceDialogServiceWithMock(bool? returnValue)
    {
        var dialogServiceMock = new Mock<IDialogService>();
        dialogServiceMock.Setup(x => x.ShowMessageBoxAsync(
                It.IsAny<string?>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<DialogOptions?>()))
            .ReturnsAsync(returnValue);

        Context.Services.RemoveAll<IDialogService>();
        Context.Services.AddSingleton(dialogServiceMock.Object);
        return dialogServiceMock;
    }

    /// <summary>
    /// Keeps dialog-argument verification consistent and readable across tests.
    /// </summary>
    private static void VerifyDialogMessage(
        Mock<IDialogService> dialogServiceMock,
        string expectedTitle,
        string expectedMessage,
        string expectedYesText,
        string expectedNoText)
    {
        dialogServiceMock.Verify(x => x.ShowMessageBoxAsync(
                expectedTitle,
                expectedMessage,
                expectedYesText,
                expectedNoText,
                null,
                null),
            Times.Once);
    }

    private sealed class JsInteropRecorder
    {
        public Mock<IJSRuntime> JsRuntimeMock { get; }
        public List<string> EnabledPromptIds { get; } = [];
        public List<string> DisabledPromptIds { get; } = [];
        public List<(string PromptId, string Text)> TextUpdates { get; } = [];
        public List<string> NativeNavigationChecks { get; } = [];

        public bool NativeNavigationResult { get; set; } = true;

        /// <summary>
        /// Captures interop calls as structured data so tests verify contracts instead of callback mechanics.
        /// </summary>
        public JsInteropRecorder()
        {
            JsRuntimeMock = new Mock<IJSRuntime>();

            JsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.enable", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>)
                .Callback<string, object[]>((_, args) => EnabledPromptIds.Add((string)args[0]));

            JsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.disable", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>)
                .Callback<string, object[]>((_, args) => DisabledPromptIds.Add((string)args[0]));

            JsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.setText", It.IsAny<object[]>()))
                .ReturnsAsync(Mock.Of<IJSVoidResult>)
                .Callback<string, object[]>((_, args) => TextUpdates.Add(((string)args[0], (string)args[1])));

            JsRuntimeMock.Setup(x => x.InvokeAsync<bool>("mudExitPrompt.handleBeforeNavigation", It.IsAny<object[]>()))
                .Callback<string, object[]>((_, args) => NativeNavigationChecks.Add((string)args[0]))
                .Returns(() => new ValueTask<bool>(NativeNavigationResult));
        }
    }
}
