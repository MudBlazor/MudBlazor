// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using MudBlazor.UnitTests.TestComponents.Hotkey;
using MudBlazor.Utilities;
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
    public void Hotkey_JsTestComponentLifetimeCycle()
    {
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()));
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()));
        Context.Services.AddSingleton(jsRuntimeMock.Object);

        Context.RenderComponent<MudHotkeyTest>();
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(1));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Never);

        Context.DisposeComponents();
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(1));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));
    }

    [Test, Ignore("Currently doesn't work due to the requirement for an EnumerableEqualityComparer in MudHotkey.razor.cs")]
    public async Task Hotkey_JsTestParameters()
    {
        var jsRuntimeMock = new Mock<IJSRuntime>();
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()));
        jsRuntimeMock.Setup(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()));
        Context.Services.AddSingleton(jsRuntimeMock.Object);

        var comp = Context.RenderComponent<MudHotkeyTest>();
        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Key, JsKey.KeyB));
        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.KeyModifiers, [JsKeyModifier.ShiftLeft]));
        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, true));
        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.PreventEventPropagation, false));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(4));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));

        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.Disabled, false));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(5));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));

        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.ChildContent, builder => builder.AddContent(0, "New Child Content")));
        await comp.SetParametersAndRenderAsync(p => p.Add(x => x.HideChildContentOnRepress, true));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.registerOrUpdateHotkey", It.IsAny<object[]>()), Times.Exactly(5));
        jsRuntimeMock.Verify(x => x.InvokeAsync<IJSVoidResult>("mudHotkeyListener.unregisterHotkey", It.IsAny<object[]>()), Times.Exactly(1));
    }
}
