// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.TestComponents.Hotkey;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

#nullable enable
[TestFixture]
public class MudHotkeyTests : BunitTest
{
    [Test]
    public async Task Hotkey_ShouldShowChildContent()
    {
        // Arrange
        var comp = Context.Render<MudHotkeyTest>(p => p.Add(x => x.HideChildContentOnRepress, false));
        var hotKeyComponent = comp.FindComponent<MudHotkey>();

        // Act
        await comp.InvokeAsync(hotKeyComponent.Instance.MudHotkeyProviderJsCallback);

        // Assert
        comp.Instance.PressedCount.Should().Be(1);
        comp.Find("#hotkey-child").TextContent.Trim().Should().Be("Child Content");
    }

    [Test]
    public async Task Hotkey_ShouldNotShowChildContent()
    {
        // Arrange
        var comp = Context.Render<MudHotkeyTest>(p => p.Add(x => x.HideChildContentOnRepress, true));
        var hotKeyComponent = comp.FindComponent<MudHotkey>();
        var hotKeyChildContent = () => comp.Find("#hotkey-child");

        // Act
        await comp.InvokeAsync(hotKeyComponent.Instance.MudHotkeyProviderJsCallback);
        await comp.InvokeAsync(hotKeyComponent.Instance.MudHotkeyProviderJsCallback);

        // Assert
        comp.Instance.PressedCount.Should().Be(2);
        hotKeyChildContent.Should().Throw<ElementNotFoundException>();
    }

    [Test]
    public async Task Hotkey_JsTestComponentLifetimeCycle()
    {
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()));
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()));
        Context.Services.AddSingleton(jsRuntimeMock.Object);

        Context.Render<MudHotkeyTest>();
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(1));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Never);

        await Context.DisposeComponentsAsync();
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(1));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));
    }

    [Test]
    public async Task Hotkey_JsTestParameters()
    {
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()));
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()));
        Context.Services.AddSingleton(jsRuntimeMock.Object);

        var comp = Context.Render<MudHotkeyTest>();

        // Enabled by default
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Once);
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Never);

        await comp.SetParametersAndRenderAsync(p => p
            .Add(x => x.Key, JsKey.KeyB)
            .Add(x => x.KeyModifiers, [JsKeyModifier.ShiftLeft])
            .Add(x => x.PreventEventPropagation, false));

        // Shared handler, so only one update called
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(2));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Never);

        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, true));

        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(2));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));

        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, false));

        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(3));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));

        // Changing ChildContent / HideChildContentOnRepress should not unregister/register hotkey
        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ChildContent, builder => builder.AddContent(0, "New Child Content")));
        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.HideChildContentOnRepress, true));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(3));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));
    }
}
