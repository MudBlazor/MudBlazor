// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Docs.Services;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Docs.Services;

[TestFixture]
public class DocsJsApiServiceTests : BunitTest
{
    [Test]
    public async Task ScrollToActiveNavLinkAsync_ShouldInvokeWithoutError()
    {
        // Arrange
        Context.Services.AddSingleton<IDocsJsApiService, MockDocsJsApiService>();
        var service = Context.Services.GetRequiredService<IDocsJsApiService>();

        // Act
        var act = async () => await service.ScrollToActiveNavLinkAsync();

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Test]
    public async Task GetInnerTextByIdAsync_ShouldReturnText()
    {
        // Arrange
        Context.Services.AddSingleton<IDocsJsApiService, MockDocsJsApiService>();
        var service = Context.Services.GetRequiredService<IDocsJsApiService>();

        // Act
        var result = await service.GetInnerTextByIdAsync("test-id");

        // Assert
        result.Should().Be("inner text");
    }
}
