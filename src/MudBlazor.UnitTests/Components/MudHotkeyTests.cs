// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AngleSharp.Dom;
using Bunit;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.TestComponents.Hotkey;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Components;

[TestFixture]
public class MudHotkeyTests : BunitTest
{
    [Test]
    public async Task Hotkey_ShouldShowChildContent()
    {
        // Arrange
        var comp = Context.RenderComponent<MudHotkeyTest>(p => p.Add(x => x.HideChildContentOnRepress, false));
        var hotKeyComponent = comp.FindComponent<MudHotkey>();
        IElement HotKeyChildContent() => comp.Find("#hotkey-child");

        // Act
        await comp.InvokeAsync(hotKeyComponent.Instance.MudHotkeyProviderJsCallback);

        // Assert
        comp.Instance.PressedCount.Should().Be(1);
        HotKeyChildContent().TextContent.Trim().Should().Be("Child Content");
    }

    [Test]
    public async Task Hotkey_ShouldNotShowChildContent()
    {
        // Arrange
        var comp = Context.RenderComponent<MudHotkeyTest>(p => p.Add(x => x.HideChildContentOnRepress, true));
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
    public async Task Hotkey_JsTest()
    {
        // Arrange
        var jsRuntimeMock = new Mock<IJSRuntime>();

        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerHotkey", It.IsAny<object[]>()));
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()));

        Context.Services.AddSingleton(jsRuntimeMock.Object);
        var comp = Context.RenderComponent<MudHotkeyTest>();

        // Assert
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerHotkey", It.IsAny<object[]>()), Times.Exactly(1));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Never);

        // Act
        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, true));

        // Assert
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerHotkey", It.IsAny<object[]>()), Times.Exactly(1));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));
    }
}
