// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using FluentAssertions;
using Microsoft.Extensions.Options;
using MudBlazor.UnitTests.Components;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class SnackbarServiceTests : BunitTest
{
    [Test]
    public void SnackbarService_CanBeInstantiatedWithoutNavigationManager()
    {
        // Arrange & Act
        var configuration = Options.Create(new SnackbarConfiguration());
        var sut = new SnackbarService(configuration);

        // Assert
        sut.Should().NotBeNull();
        sut.Configuration.Should().NotBeNull();
    }

    [Test]
    public void SnackbarService_CanAddMessage()
    {
        // Arrange
        var configuration = Options.Create(new SnackbarConfiguration());
        var sut = new SnackbarService(configuration);

        // Act
        sut.Add("Test message");

        // Assert
        sut.ShownSnackbars.Should().NotBeEmpty();
    }

    [Test]
    public void SnackbarService_CanClearMessages()
    {
        // Arrange
        var configuration = Options.Create(new SnackbarConfiguration());
        var sut = new SnackbarService(configuration);
        sut.Add("Test message");
        sut.ShownSnackbars.Should().NotBeEmpty();

        // Act
        sut.Clear();

        // Assert
        sut.ShownSnackbars.Should().BeEmpty();
    }
}
