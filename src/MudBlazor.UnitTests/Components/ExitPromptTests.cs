using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class ExitPromptTests : BunitTest
{
    [Test]
    public async Task JsInteropIsCalledCorrectly()
    {
        // Arrange: provide a JS runtime mock and wire NavigationManager to bUnit's implementation.
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.enable", It.IsAny<object[]>()));
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.disable", It.IsAny<object[]>()));
        jsRuntimeMock.Setup(x => x.InvokeAsync<bool>("mudExitPrompt.handleBeforeNavigation", It.IsAny<object[]>())).Returns(new ValueTask<bool>(true));
        Context.Services.AddSingleton(jsRuntimeMock.Object);
        Context.Services.AddSingleton<NavigationManager>(s => s.GetRequiredService<BunitNavigationManager>());

        // Initial render with UseNativePrompt enabled should register the prompt once.
        var component = Context.Render<MudExitPrompt>(x => x.Add(p => p.UseNativePrompt, true));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.enable", It.IsAny<object[]>()), Times.Exactly(1));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.disable", It.IsAny<object[]>()), Times.Never);

        // Disabling the component should unregister the prompt and skip navigation interception.
        await component.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.Disabled, true));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.enable", It.IsAny<object[]>()), Times.Exactly(1));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.disable", It.IsAny<object[]>()), Times.Exactly(1));

        var navigationManager = Context.Services.GetRequiredService<BunitNavigationManager>();
        navigationManager.NavigateTo("/test1");
        jsRuntimeMock.Verify(x => x.InvokeAsync<bool>("mudExitPrompt.handleBeforeNavigation", It.IsAny<object[]>()), Times.Never);

        // Re-enabling should register again, and navigation should now invoke before-navigation handling.
        await component.SetParametersAndRenderAsync(parameters => parameters.Add(parameter => parameter.Disabled, false));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.enable", It.IsAny<object[]>()), Times.Exactly(2));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.disable", It.IsAny<object[]>()), Times.Exactly(1));

        navigationManager.NavigateTo("/test2");
        jsRuntimeMock.Verify(x => x.InvokeAsync<bool>("mudExitPrompt.handleBeforeNavigation", It.IsAny<object[]>()), Times.Exactly(1));

        // Disposal should always unregister one final time.
        await Context.DisposeComponentsAsync();
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudExitPrompt.disable", It.IsAny<object[]>()), Times.Exactly(2));
    }
}
