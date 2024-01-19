// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.JSInterop;
using Moq;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class RenderContextTests
{
    // This test just clarifies the behaviour of the Mocked IJSRuntime with RenderContext
    // This is why we need to use MockRenderContext and MockPreRenderContext to get sane test results
    // We add the Mocks here for clarity
    [Test]
    public void CheckValuesWithMockJSRuntime()
    {
        var jsruntimeMock = new Mock<IJSRuntime>();
        var renderContext = new RenderContext(jsruntimeMock.Object);
        renderContext.IsInteractiveWebAssembly().Should().BeFalse();
        renderContext.IsInteractiveServer().Should().BeFalse();
        renderContext.IsStaticServer().Should().BeTrue();
    }
}
