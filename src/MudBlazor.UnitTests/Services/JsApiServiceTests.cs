// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class JsApiServiceTests
{
    private Mock<IJSRuntime> _runtimeMock;
    private JsApiService _service;

    [SetUp]
    public void SetUp()
    {
        _runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
        _service = new JsApiService(_runtimeMock.Object);
    }

    /// <summary>
    /// Browser calls made after the circuit is gone complete without throwing.
    /// </summary>
    [TestCase("mudWindow.copyToClipboard")]
    [TestCase("open")]
    [TestCase("mudWindow.open")]
    [TestCase("mudWindow.updateStyleProperty")]
    public async Task Methods_Disconnected_Swallowed(string identifier)
    {
        SetupThrowingInvocation(identifier, new JSDisconnectedException("disconnected"));

        var act = async () => await InvokeMethod(identifier);

        await act.Should().NotThrowAsync();
    }

    /// <summary>
    /// Browser calls still surface JavaScript errors to the caller.
    /// </summary>
    [TestCase("mudWindow.copyToClipboard")]
    [TestCase("open")]
    [TestCase("mudWindow.open")]
    [TestCase("mudWindow.updateStyleProperty")]
    public async Task Methods_JSException_Rethrown(string identifier)
    {
        SetupThrowingInvocation(identifier, new JSException("failed"));

        var act = async () => await InvokeMethod(identifier);

        await act.Should().ThrowAsync<JSException>();
    }

    /// <summary>
    /// Calls the service method that invokes the given JavaScript identifier.
    /// </summary>
    private ValueTask InvokeMethod(string identifier) => identifier switch
    {
        "mudWindow.copyToClipboard" => _service.CopyToClipboardAsync("text"),
        "open" => _service.Open("https://mudblazor.com", "_self"),
        "mudWindow.open" => _service.Open("https://mudblazor.com", "_blank"),
        "mudWindow.updateStyleProperty" => _service.UpdateStyleProperty("element", "color", "red"),
        _ => throw new ArgumentOutOfRangeException(nameof(identifier))
    };

    private void SetupThrowingInvocation(string identifier, Exception exception)
    {
        _runtimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>(identifier, It.IsAny<object[]>()))
            .Throws(exception);
    }
}
