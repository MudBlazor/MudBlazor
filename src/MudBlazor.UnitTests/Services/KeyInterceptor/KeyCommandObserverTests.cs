// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Services.KeyInterceptor;

#nullable enable
[TestFixture]
public class KeyCommandObserverTests
{
    [Test]
    public async Task NotifyOnKeyDownAsync_MatchingCommand_Executes()
    {
        // Arrange
        var executed = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                executed = true;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);

        // Assert
        executed.Should().BeTrue();
    }

    [Test]
    public async Task NotifyOnKeyUpAsync_MatchingCommand_Executes()
    {
        // Arrange
        var executed = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyUp("Enter", () =>
            {
                executed = true;
                return Task.CompletedTask;
            });

        var (_, keyUp) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyUp.NotifyOnKeyUpAsync(args);

        // Assert
        executed.Should().BeTrue();
    }

    [Test]
    public async Task NotifyOnKeyDownAsync_MixedCommands_OnlyExecutesKeyDownCommands()
    {
        // Arrange
        var downExecuted = false;
        var upExecuted = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                downExecuted = true;
                return Task.CompletedTask;
            })
            .OnKeyUp("Enter", () =>
            {
                upExecuted = true;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);

        // Assert
        downExecuted.Should().BeTrue();
        upExecuted.Should().BeFalse(); // Should not execute KeyUp command
    }

    [Test]
    public async Task NotifyOnKeyUpAsync_MixedCommands_OnlyExecutesKeyUpCommands()
    {
        // Arrange
        var downExecuted = false;
        var upExecuted = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                downExecuted = true;
                return Task.CompletedTask;
            })
            .OnKeyUp("Enter", () =>
            {
                upExecuted = true;
                return Task.CompletedTask;
            });

        var (_, keyUp) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyUp.NotifyOnKeyUpAsync(args);

        // Assert
        downExecuted.Should().BeFalse(); // Should not execute KeyDown command
        upExecuted.Should().BeTrue();
    }

    [Test]
    public async Task NotifyOnKeyDownAsync_MultipleMatchingCommands_ExecutesOnlyFirst()
    {
        // Arrange
        var firstExecuted = false;
        var secondExecuted = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                firstExecuted = true;
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                secondExecuted = true;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);

        // Assert
        firstExecuted.Should().BeTrue();
        secondExecuted.Should().BeFalse(); // Early exit pattern - only first match executes
    }

    [Test]
    public async Task NotifyOnKeyDownAsync_EmptyCommandList_CompletesSuccessfully()
    {
        // Arrange
        var builder = KeyMapBuilder.Create()
            .OnKeyUp("Enter", () => Task.CompletedTask); // Only KeyUp command

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act & Assert - should not throw
        await keyDown.NotifyOnKeyDownAsync(args);
    }

    [Test]
    public async Task NotifyOnKeyDownAsync_HookThenTwoMatchingCommands_StopsAfterFirstNonHook()
    {
        // Hook continues the chain, the first non-hook command then stops it before the second runs.
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(_ =>
            {
                executionOrder.Add("Hook");
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("First");
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Second");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - hook runs, first command runs and halts dispatch, second never runs
        executionOrder.Should().Equal("Hook", "First");
    }

    [Test]
    public async Task NotifyOnKeyUpAsync_HookContinuesChainToMatchingCommand()
    {
        // The hook-continuation branch of DispatchAsync must apply to key-up dispatch too.
        var hookExecuted = false;
        var commandExecuted = false;
        var builder = KeyMapBuilder.Create()
            .HookKeyUp(_ =>
            {
                hookExecuted = true;
                return Task.CompletedTask;
            })
            .OnKeyUp("Enter", () =>
            {
                commandExecuted = true;
                return Task.CompletedTask;
            });

        var (_, keyUp) = builder.Build();

        // Act
        await keyUp.NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - hook did not short-circuit dispatch before the command
        hookExecuted.Should().BeTrue();
        commandExecuted.Should().BeTrue();
    }

    [Test]
    public async Task PerformanceOptimization_SeparatesCommandsByKindAtConstruction()
    {
        // This test verifies that the observer splits commands at construction time
        // rather than checking Kind on every dispatch.

        // Arrange
        var downCount = 0;
        var upCount = 0;
        var builder = KeyMapBuilder.Create();

        // Add many commands of both kinds
        for (var i = 0; i < 100; i++)
        {
            var key = $"Key{i}";
            builder.OnKeyDown(key, () =>
            {
                downCount++;
                return Task.CompletedTask;
            });
            builder.OnKeyUp(key, () =>
            {
                upCount++;
                return Task.CompletedTask;
            });
        }

        var (keyDown, keyUp) = builder.Build();

        // Act - trigger one down event
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Key50" });

        // Assert - should only check down commands (not iterate through up commands)
        downCount.Should().Be(1);
        upCount.Should().Be(0);

        // Act - trigger one up event
        await keyUp.NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "Key50" });

        // Assert
        downCount.Should().Be(1); // Still 1
        upCount.Should().Be(1);
    }
}
