// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Moq;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

[TestFixture]
public class ElementReferenceExtensionsTests
{
    private Mock<IJSRuntime> _runtimeMock;
    private ElementReference _element;

    [SetUp]
    public void SetUp()
    {
        _runtimeMock = new Mock<IJSRuntime>(MockBehavior.Strict);
        _element = new ElementReference("element", new WebElementReferenceContext(_runtimeMock.Object));
    }

    /// <summary>
    /// Element calls made after the circuit is gone complete without throwing.
    /// </summary>
    [TestCase("mudElementRef.blur")]
    [TestCase("mudElementRef.select")]
    [TestCase("mudElementRef.selectRange")]
    [TestCase("mudElementRef.changeCss")]
    public async Task Methods_Disconnected_Swallowed(string identifier)
    {
        SetupThrowingInvocation(identifier, new JSDisconnectedException("disconnected"));

        var act = async () => await InvokeMethod(identifier);

        await act.Should().NotThrowAsync();
    }

    /// <summary>
    /// Element calls still surface JavaScript errors to the caller.
    /// </summary>
    [TestCase("mudElementRef.blur")]
    [TestCase("mudElementRef.select")]
    [TestCase("mudElementRef.selectRange")]
    [TestCase("mudElementRef.changeCss")]
    public async Task Methods_JSException_Rethrown(string identifier)
    {
        SetupThrowingInvocation(identifier, new JSException("failed"));

        var act = async () => await InvokeMethod(identifier);

        await act.Should().ThrowAsync<JSException>();
    }

    /// <summary>
    /// Calls the extension method that invokes the given JavaScript identifier.
    /// </summary>
    private ValueTask InvokeMethod(string identifier) => identifier switch
    {
        "mudElementRef.blur" => _element.MudBlurAsync(),
        "mudElementRef.select" => _element.MudSelectAsync(),
        "mudElementRef.selectRange" => _element.MudSelectRangeAsync(1, 2),
        "mudElementRef.changeCss" => _element.MudChangeCssAsync("active"),
        _ => throw new ArgumentOutOfRangeException(nameof(identifier))
    };

    private void SetupThrowingInvocation(string identifier, Exception exception)
    {
        _runtimeMock
            .Setup(x => x.InvokeAsync<IJSVoidResult>(identifier, It.IsAny<object[]>()))
            .Throws(exception);
    }
}
