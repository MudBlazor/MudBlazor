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
public class KeyMapBuilderTests
{
    [Test]
    public void Build_EmptyBuilder_ReturnsEmptyObservers()
    {
        // Arrange
        var builder = KeyMapBuilder.Create();

        // Act
        var (keyDown, keyUp) = builder.Build();

        // Assert
        var typeKeyUpIgnore = KeyObserver.KeyUpIgnore().GetType();
        var typeKeyDownIgnore = KeyObserver.KeyDownIgnore().GetType();
        keyDown.Should().BeOfType(typeKeyDownIgnore);
        keyUp.Should().BeOfType(typeKeyUpIgnore);
    }

    [Test]
    public async Task OnKeyDown_SimpleCommand_ExecutesAction()
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
    public async Task OnKeyDown_WrongKey_DoesNotExecute()
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
        var args = new KeyboardEventArgs { Key = "Escape" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);

        // Assert
        executed.Should().BeFalse();
    }

    [Test]
    public async Task OnKeyDown_ConditionalCommand_ExecutesWhenConditionTrue()
    {
        // Arrange
        var condition = true;
        var executed = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                executed = true;
                return Task.CompletedTask;
            }, when: () => condition);

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);

        // Assert
        executed.Should().BeTrue();
    }

    [Test]
    public async Task OnKeyDown_ConditionalCommand_DoesNotExecuteWhenConditionFalse()
    {
        // Arrange
        var condition = false;
        var executed = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                executed = true;
                return Task.CompletedTask;
            }, when: () => condition);

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);

        // Assert
        executed.Should().BeFalse();
    }

    [Test]
    public async Task OnKeyDownAny_MultipleKeys_ExecutesForAnyKey()
    {
        // Arrange
        var executedCount = 0;
        var builder = KeyMapBuilder.Create()
            .OnKeyDownAny(["Enter", "NumpadEnter", "Space"], () =>
            {
                executedCount++;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "NumpadEnter" });
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Space" });
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Escape" }); // Should not execute

        // Assert
        executedCount.Should().Be(3);
    }

    [Test]
    public async Task OnKeyDownAny_WithCondition_ExecutesWhenConditionTrue()
    {
        // Arrange
        var condition = true;
        var executed = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDownAny(["Enter", "NumpadEnter"], () =>
            {
                executed = true;
                return Task.CompletedTask;
            }, when: () => condition);

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);

        // Assert
        executed.Should().BeTrue();
    }

    [Test]
    public async Task OnKeyDownAny_WithCondition_DoesNotExecuteWhenConditionFalse()
    {
        // Arrange
        var condition = false;
        var executed = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDownAny(["Enter", "NumpadEnter"], () =>
            {
                executed = true;
                return Task.CompletedTask;
            }, when: () => condition);

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);

        // Assert
        executed.Should().BeFalse();
    }

    [Test]
    public async Task When_ConditionalScope_AppliesConditionToAllCommands()
    {
        // Arrange
        var condition = true;
        var enterExecuted = false;
        var spaceExecuted = false;
        var escapeExecuted = false;

        var builder = KeyMapBuilder.Create()
            .When(() => condition, b => b
                .OnKeyDown("Enter", () =>
                {
                    enterExecuted = true;
                    return Task.CompletedTask;
                })
                .OnKeyDown("Space", () =>
                {
                    spaceExecuted = true;
                    return Task.CompletedTask;
                })
                .OnKeyDown("Escape", () =>
                {
                    escapeExecuted = true;
                    return Task.CompletedTask;
                }));

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Space" });
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Escape" });

        // Assert
        enterExecuted.Should().BeTrue();
        spaceExecuted.Should().BeTrue();
        escapeExecuted.Should().BeTrue();
    }

    [Test]
    public async Task When_ConditionalScope_DoesNotExecuteWhenConditionFalse()
    {
        // Arrange
        var condition = false;
        var enterExecuted = false;
        var spaceExecuted = false;

        var builder = KeyMapBuilder.Create()
            .When(() => condition, b => b
                .OnKeyDown("Enter", () =>
                {
                    enterExecuted = true;
                    return Task.CompletedTask;
                })
                .OnKeyDown("Space", () =>
                {
                    spaceExecuted = true;
                    return Task.CompletedTask;
                }));

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Space" });

        // Assert
        enterExecuted.Should().BeFalse();
        spaceExecuted.Should().BeFalse();
    }

    [Test]
    public async Task OnKeyUp_SimpleCommand_ExecutesAction()
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
    public async Task Build_MixedDownAndUpCommands_ReturnsCorrectObservers()
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

        var (keyDown, keyUp) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyDown.NotifyOnKeyDownAsync(args);
        await keyUp.NotifyOnKeyUpAsync(args);

        // Assert
        downExecuted.Should().BeTrue();
        upExecuted.Should().BeTrue();
    }

    [Test]
    public async Task Build_OnlyKeyDownCommands_KeyUpDoesNothing()
    {
        // Arrange
        var executed = false;
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                executed = true;
                return Task.CompletedTask;
            });

        var (_, keyUp) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act
        await keyUp.NotifyOnKeyUpAsync(args); // Should not execute the keydown command

        // Assert
        executed.Should().BeFalse();
    }

    [Test]
    public async Task MultipleCommands_ExecutesFirstMatchingCommand()
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
        secondExecuted.Should().BeFalse(); // Only first matching command should execute
    }

    [Test]
    public async Task When_NestedScopes_AppliesConditionsCorrectly()
    {
        // Arrange
        var outerCondition = true;
        var executed = false;

        var builder = KeyMapBuilder.Create()
            .When(() => outerCondition, b => b
                .OnKeyDown("Enter", () =>
                {
                    executed = true;
                    return Task.CompletedTask;
                }));

        var (keyDown, _) = builder.Build();
        var args = new KeyboardEventArgs { Key = "Enter" };

        // Act - condition true
        await keyDown.NotifyOnKeyDownAsync(args);
        var executedWhenTrue = executed;

        // Change condition
        executed = false;
        outerCondition = false;
        await keyDown.NotifyOnKeyDownAsync(args);
        var executedWhenFalse = executed;

        // Assert
        executedWhenTrue.Should().BeTrue();
        executedWhenFalse.Should().BeFalse();
    }
}
