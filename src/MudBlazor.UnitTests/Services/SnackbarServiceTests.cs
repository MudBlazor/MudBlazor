// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Bunit.TestDoubles;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class SnackbarServiceTests : BunitTest
{
    private FakeNavigationManager _navigationManager;

    public override void Setup()
    {
        base.Setup();
        _navigationManager = Context.Services.GetRequiredService<FakeNavigationManager>();
    }

    [Test]
    public void NavigationManager_LocationChanged_ClearsSnackbarsWhenClearAfterNavigationIsTrue()
    {
        // Arrange
        var configuration = Options.Create(new SnackbarConfiguration { ClearAfterNavigation = true });
        var sut = new SnackbarService(_navigationManager, configuration);
        sut.Add("Test message");
        sut.ShownSnackbars.Should().NotBeEmpty();

        // Act
        _navigationManager.NavigateTo("/new-location");

        // Assert
        sut.ShownSnackbars.Should().BeEmpty();
    }

    [Test]
    public void NavigationManager_LocationChanged_DoesNotClearSnackbarsWhenClearAfterNavigationIsFalse()
    {
        // Arrange
        var configuration = Options.Create(new SnackbarConfiguration { ClearAfterNavigation = false });
        var sut = new SnackbarService(_navigationManager, configuration);
        sut.Add("Test message");

        // Act
        _navigationManager.NavigateTo("/new-location");

        // Assert
        sut.ShownSnackbars.Should().NotBeEmpty();
    }

    [Test]
    public void NavigationManager_LocationChanged_RemovesSnackbarsWithCloseAfterNavigationEnabled()
    {
        // Arrange
        var configuration = Options.Create(new SnackbarConfiguration { ClearAfterNavigation = false });
        var sut = new SnackbarService(_navigationManager, configuration);
        sut.Add("Test message", configure: options => options.CloseAfterNavigation = true);
        sut.Add("Another message", configure: options => options.CloseAfterNavigation = false);

        // Act
        _navigationManager.NavigateTo("/new-location");

        // Assert
        sut.ShownSnackbars.Should().ContainSingle().Which.SnackbarMessage.Text.Should().Be("Another message");
    }
}
