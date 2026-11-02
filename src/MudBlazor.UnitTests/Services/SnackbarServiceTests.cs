// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services;

[TestFixture]
public class SnackbarServiceTests : BunitTest
{
    private BunitNavigationManager _navigationManager;

    public override void Setup()
    {
        base.Setup();
        _navigationManager = Context.Services.GetRequiredService<BunitNavigationManager>();
    }

    [Test]
    public void NavigationManager_LocationChanged_ClearsSnackbarsWhenClearAfterNavigationIsTrue()
    {
        // Arrange
        var configuration = Options.Create(new SnackbarConfiguration { ClearAfterNavigation = true });
        var timeProvider = new FakeTimeProvider();
        var sut = new SnackbarService(_navigationManager, timeProvider, configuration);
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
        var timeProvider = new FakeTimeProvider();
        var sut = new SnackbarService(_navigationManager, timeProvider, configuration);
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
        var timeProvider = new FakeTimeProvider();
        var sut = new SnackbarService(_navigationManager, timeProvider, configuration);
        sut.Add("Test message", configure: options => options.CloseAfterNavigation = true);
        sut.Add("Another message", configure: options => options.CloseAfterNavigation = false);

        // Act
        _navigationManager.NavigateTo("/new-location");

        // Assert
        sut.ShownSnackbars.Should().ContainSingle().Which.SnackbarMessage.Text.Should().Be("Another message");
    }

    [Test]
    public void ShownSnackbars_ReturnsSnapshotWhenCollectionChanges()
    {
        // Arrange
        var timeProvider = new FakeTimeProvider();
        var sut = new SnackbarService(_navigationManager, timeProvider);
        sut.Add("First message");

        // Act
        var snapshot = sut.ShownSnackbars;
        sut.Add("Second message");

        // Assert
        snapshot.Select(x => x.SnackbarMessage.Text).Should().Equal("First message");
    }

    [Test]
    public void Remove_SnackbarAlreadyRemoved_IsNoOp()
    {
        // Arrange
        var sut = new SnackbarService(_navigationManager, new FakeTimeProvider());
        var snackbar = sut.Add("Test message");

        // Act
        sut.Remove(snackbar);
        sut.Remove(snackbar); // Second removal: the snackbar is no longer in the list.

        // Assert
        sut.ShownSnackbars.Should().BeEmpty();
    }

    [Test]
    public void RemoveByKey_NoMatchingKey_IsNoOp()
    {
        // Arrange
        var sut = new SnackbarService(_navigationManager, new FakeTimeProvider());
        sut.Add("Test message", key: "keep");

        // Act
        sut.RemoveByKey("does-not-exist");

        // Assert
        sut.ShownSnackbars.Should().ContainSingle().Which.SnackbarMessage.Key.Should().Be("keep");
    }

    [Test]
    public void ShownSnackbars_RespectsMaxDisplayedSnackbars()
    {
        // Arrange
        var configuration = Options.Create(new SnackbarConfiguration { MaxDisplayedSnackbars = 2, PreventDuplicates = false });
        var sut = new SnackbarService(_navigationManager, new FakeTimeProvider(), configuration);
        sut.Add("First");
        sut.Add("Second");
        sut.Add("Third");

        // Act & Assert: only the first two are exposed even though three were added.
        sut.ShownSnackbars.Select(x => x.SnackbarMessage.Text).Should().Equal("First", "Second");
    }

    [Test]
    public void Dispose_UnsubscribesFromNavigationAndConfiguration()
    {
        // Arrange
        var configuration = Options.Create(new SnackbarConfiguration { ClearAfterNavigation = true });
        var sut = new SnackbarService(_navigationManager, new FakeTimeProvider(), configuration);
        var raised = 0;
        sut.OnSnackbarsUpdated += () => raised++;

        // Act
        sut.Dispose();

        // Assert: post-dispose navigation and config changes must no longer notify the disposed service.
        _navigationManager.NavigateTo("/new-location");
        sut.Configuration.NewestOnTop = !sut.Configuration.NewestOnTop;
        raised.Should().Be(0);
    }
}
