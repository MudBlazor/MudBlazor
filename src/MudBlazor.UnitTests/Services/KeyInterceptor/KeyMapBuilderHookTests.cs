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
public class KeyMapBuilderHookTests
{
    [Test]
    public async Task HookKeyDown_ExecutesForAnyKey()
    {
        // Arrange
        var hookExecutedKeys = new List<string>();
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                hookExecutedKeys.Add(args.Key);
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act - try different keys
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Escape" });
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "a" });

        // Assert
        hookExecutedKeys.Should().HaveCount(3);
        hookExecutedKeys.Should().Contain("Enter");
        hookExecutedKeys.Should().Contain("Escape");
        hookExecutedKeys.Should().Contain("a");
    }

    [Test]
    public async Task HookKeyUp_ExecutesForAnyKey()
    {
        // Arrange
        var hookExecutedKeys = new List<string>();
        var builder = KeyMapBuilder.Create()
            .HookKeyUp(args =>
            {
                hookExecutedKeys.Add(args.Key);
                return Task.CompletedTask;
            });

        var (_, keyUp) = builder.Build();

        // Act - try different keys
        await keyUp.NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "Enter" });
        await keyUp.NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "Escape" });
        await keyUp.NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "a" });

        // Assert
        hookExecutedKeys.Should().HaveCount(3);
        hookExecutedKeys.Should().Contain("Enter");
        hookExecutedKeys.Should().Contain("Escape");
        hookExecutedKeys.Should().Contain("a");
    }

    [Test]
    public async Task HookKeyDown_ExecutesBeforeOtherCommands()
    {
        // Arrange
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook");
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert
        executionOrder.Should().HaveCount(2);
        executionOrder[0].Should().Be("Hook");
        executionOrder[1].Should().Be("Command");
    }

    [Test]
    public async Task HookKeyDown_AfterMatchingCommand_StillExecutes()
    {
        // Arrange
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command");
                return Task.CompletedTask;
            })
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Hook executes first (inserted at index 0), then Command
        executionOrder.Should().HaveCount(2);
        executionOrder[0].Should().Be("Hook");
        executionOrder[1].Should().Be("Command");
    }

    [Test]
    public async Task HookKeyDown_MultipleHooks_ExecuteInOrder()
    {
        // Arrange
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook1");
                return Task.CompletedTask;
            })
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook2");
                return Task.CompletedTask;
            })
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook3");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert
        executionOrder.Should().HaveCount(3);
        executionOrder[0].Should().Be("Hook1");
        executionOrder[1].Should().Be("Hook2");
        executionOrder[2].Should().Be("Hook3");
    }

    [Test]
    public async Task HookKeyDown_DoesNotPreventOtherCommandsFromExecuting()
    {
        // Arrange
        var hookExecuted = false;
        var commandExecuted = false;
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                hookExecuted = true;
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                commandExecuted = true;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert
        hookExecuted.Should().BeTrue();
        commandExecuted.Should().BeTrue();
    }

    [Test]
    public async Task HookKeyDown_ExecutesEvenWhenNoOtherCommandMatches()
    {
        // Arrange
        var hookExecuted = false;
        var commandExecuted = false;
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                hookExecuted = true;
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                commandExecuted = true;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act - press a key that doesn't match any command
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Escape" });

        // Assert
        hookExecuted.Should().BeTrue();
        commandExecuted.Should().BeFalse();
    }

    [Test]
    public async Task HookKeyDown_OutsideWhenScope_ExecutesRegardlessOfCondition()
    {
        // Arrange
        var condition = false; // Condition is false
        var hookExecuted = false;
        var commandExecuted = false;
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                hookExecuted = true;
                return Task.CompletedTask;
            })
            .When(() => condition, b => b
                .OnKeyDown("Enter", () =>
                {
                    commandExecuted = true;
                    return Task.CompletedTask;
                }));

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - hook executes even though When condition is false (hook is outside the When scope)
        hookExecuted.Should().BeTrue();
        commandExecuted.Should().BeFalse();
    }

    [Test]
    public async Task HookKeyDown_InsideWhenScope_ExecutesIfConditionTrue()
    {
        // Arrange
        var condition = true; // Condition is true
        var hookExecuted = false;
        var builder = KeyMapBuilder.Create()
            .When(() => condition, b => b
                .HookKeyDown(args =>
                {
                    hookExecuted = true;
                    return Task.CompletedTask;
                }));

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - hook executes when condition is true
        hookExecuted.Should().BeTrue();

        // Reset and change condition
        hookExecuted = false;
        condition = false;

        // Act again with false condition
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - hook doesn't execute when condition is false (wrapped in ConditionalCommand)
        hookExecuted.Should().BeFalse();
    }

    [Test]
    public async Task HookKeyDown_ReceivesKeyboardEventArgs()
    {
        // Arrange
        KeyboardEventArgs? receivedArgs = null;
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                receivedArgs = args;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();
        var originalArgs = new KeyboardEventArgs
        {
            Key = "Enter",
            CtrlKey = true,
            ShiftKey = false,
            AltKey = true
        };

        // Act
        await keyDown.NotifyOnKeyDownAsync(originalArgs);

        // Assert
        receivedArgs.Should().NotBeNull();
        receivedArgs!.Key.Should().Be("Enter");
        receivedArgs.CtrlKey.Should().BeTrue();
        receivedArgs.ShiftKey.Should().BeFalse();
        receivedArgs.AltKey.Should().BeTrue();
    }

    [Test]
    public async Task HookKeyUp_ReceivesKeyboardEventArgs()
    {
        // Arrange
        KeyboardEventArgs? receivedArgs = null;
        var builder = KeyMapBuilder.Create()
            .HookKeyUp(args =>
            {
                receivedArgs = args;
                return Task.CompletedTask;
            });

        var (_, keyUp) = builder.Build();
        var originalArgs = new KeyboardEventArgs
        {
            Key = "Escape",
            CtrlKey = false,
            ShiftKey = true,
            AltKey = false
        };

        // Act
        await keyUp.NotifyOnKeyUpAsync(originalArgs);

        // Assert
        receivedArgs.Should().NotBeNull();
        receivedArgs!.Key.Should().Be("Escape");
        receivedArgs.CtrlKey.Should().BeFalse();
        receivedArgs.ShiftKey.Should().BeTrue();
        receivedArgs.AltKey.Should().BeFalse();
    }

    [Test]
    public async Task HookKeyDown_AndHookKeyUp_IndependentExecution()
    {
        // Arrange
        var downHookExecuted = false;
        var upHookExecuted = false;
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                downHookExecuted = true;
                return Task.CompletedTask;
            })
            .HookKeyUp(args =>
            {
                upHookExecuted = true;
                return Task.CompletedTask;
            });

        var (keyDown, keyUp) = builder.Build();

        // Act - only trigger keydown
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - only down hook executed
        downHookExecuted.Should().BeTrue();
        upHookExecuted.Should().BeFalse();

        // Reset
        downHookExecuted = false;

        // Act - only trigger keyup
        await keyUp.NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - only up hook executed
        downHookExecuted.Should().BeFalse();
        upHookExecuted.Should().BeTrue();
    }

    [Test]
    public async Task HookKeyDown_WithConditionalCommands_HookAlwaysExecutes()
    {
        // Arrange
        var hookExecutionCount = 0;
        var commandExecutionCount = 0;
        var condition = true;

        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                hookExecutionCount++;
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                commandExecutionCount++;
                return Task.CompletedTask;
            }, when: () => condition);

        var (keyDown, _) = builder.Build();

        // Act - condition is true
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert
        hookExecutionCount.Should().Be(1);
        commandExecutionCount.Should().Be(1);

        // Change condition to false
        condition = false;

        // Act - condition is false
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - hook still executes, command doesn't
        hookExecutionCount.Should().Be(2);
        commandExecutionCount.Should().Be(1);
    }

    [Test]
    public async Task HookKeyDown_WithMultipleCommands_OnlyFirstMatchingCommandExecutesAfterHooks()
    {
        // Arrange
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook");
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command1");
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command2");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - hook executes, then only first matching command
        executionOrder.Should().HaveCount(2);
        executionOrder[0].Should().Be("Hook");
        executionOrder[1].Should().Be("Command1");
    }

    [Test]
    public async Task HookKeyDown_CanAccessAndModifySharedState()
    {
        // Arrange
        var sharedCounter = 0;
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                sharedCounter += 10; // Hook increments by 10
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                sharedCounter += 1; // Command increments by 1
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - both hook and command modified the shared state
        sharedCounter.Should().Be(11);
    }

    [Test]
    public async Task HookKeyDown_WithRegexPattern_StillExecutesForAllKeys()
    {
        // Arrange
        var hookExecuted = false;
        var regexCommandExecuted = false;
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                hookExecuted = true;
                return Task.CompletedTask;
            })
            .OnKeyDown("/[a-z]/", () =>
            {
                regexCommandExecuted = true;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act - press a key that matches the regex
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "a" });

        // Assert
        hookExecuted.Should().BeTrue();
        regexCommandExecuted.Should().BeTrue();

        // Reset
        hookExecuted = false;
        regexCommandExecuted = false;

        // Act - press a key that doesn't match the regex
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "1" });

        // Assert - hook still executes
        hookExecuted.Should().BeTrue();
        regexCommandExecuted.Should().BeFalse();
    }

    [Test]
    public async Task HookKeyDown_DeclaredAtEnd_StillExecutesFirst()
    {
        // Arrange
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command1");
                return Task.CompletedTask;
            })
            .OnKeyDown("Escape", () =>
            {
                executionOrder.Add("Command2");
                return Task.CompletedTask;
            })
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Hook executes first even though declared last
        executionOrder.Should().HaveCount(2);
        executionOrder[0].Should().Be("Hook");
        executionOrder[1].Should().Be("Command1");
    }

    [Test]
    public async Task HookKeyDown_MultipleHooks_DeclaredInDifferentPositions_ExecuteInDeclarationOrder()
    {
        // Arrange - Hooks maintain their declaration order regardless of where they're declared
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook1");
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command");
                return Task.CompletedTask;
            })
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook2");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Hooks execute first in declaration order (Hook1, Hook2), then Command
        executionOrder.Should().HaveCount(3);
        executionOrder[0].Should().Be("Hook1");
        executionOrder[1].Should().Be("Hook2");
        executionOrder[2].Should().Be("Command");
    }

    [Test]
    public async Task HookKeyDown_InsideWhenScope_ExecutesFirst()
    {
        // Arrange
        var condition = true;
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("CommandOutside");
                return Task.CompletedTask;
            })
            .When(() => condition, b => b
                .OnKeyDown("Enter", () =>
                {
                    executionOrder.Add("CommandInside");
                    return Task.CompletedTask;
                })
                .HookKeyDown(args =>
                {
                    executionOrder.Add("HookInside");
                    return Task.CompletedTask;
                }));

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Hook inside When executes first, then CommandOutside (stops chain)
        executionOrder.Should().HaveCount(2);
        executionOrder[0].Should().Be("HookInside");
        executionOrder[1].Should().Be("CommandOutside");
    }

    [Test]
    public async Task HookKeyDown_InsideWhenScope_RespectsCondition()
    {
        // Arrange
        var condition = false;
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command");
                return Task.CompletedTask;
            })
            .When(() => condition, b => b
                .HookKeyDown(args =>
                {
                    executionOrder.Add("HookInside");
                    return Task.CompletedTask;
                }));

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Hook inside When doesn't execute because condition is false
        executionOrder.Should().HaveCount(1);
        executionOrder[0].Should().Be("Command");
    }

    [Test]
    public async Task HookKeyUp_DeclaredAtEnd_StillExecutesFirst()
    {
        // Arrange
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .OnKeyUp("Enter", () =>
            {
                executionOrder.Add("Command1");
                return Task.CompletedTask;
            })
            .OnKeyUp("Escape", () =>
            {
                executionOrder.Add("Command2");
                return Task.CompletedTask;
            })
            .HookKeyUp(args =>
            {
                executionOrder.Add("Hook");
                return Task.CompletedTask;
            });

        var (_, keyUp) = builder.Build();

        // Act
        await keyUp.NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Hook executes first even though declared last
        executionOrder.Should().HaveCount(2);
        executionOrder[0].Should().Be("Hook");
        executionOrder[1].Should().Be("Command1");
    }

    [Test]
    public async Task HookKeyDown_DeclaredAfterWhenScope_StillExecutesFirst()
    {
        // Arrange
        var condition = true;
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .When(() => condition, b => b
                .OnKeyDown("Enter", () =>
                {
                    executionOrder.Add("CommandInWhen");
                    return Task.CompletedTask;
                })
                .OnKeyDownAny(["Escape", "Tab"], () =>
                {
                    executionOrder.Add("CommandAnyInWhen");
                    return Task.CompletedTask;
                }))
            .HookKeyDown(args =>
            {
                executionOrder.Add("HookAfterWhen");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Hook declared after When still executes first
        executionOrder.Should().HaveCount(2);
        executionOrder[0].Should().Be("HookAfterWhen");
        executionOrder[1].Should().Be("CommandInWhen");
    }

    [Test]
    public async Task HookKeyUp_DeclaredAfterWhenScope_StillExecutesFirst()
    {
        // Arrange
        var condition = true;
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .When(() => condition, b => b
                .OnKeyUp("Enter", () =>
                {
                    executionOrder.Add("CommandInWhen");
                    return Task.CompletedTask;
                })
                .OnKeyUpAny(["Escape", "Tab"], () =>
                {
                    executionOrder.Add("CommandAnyInWhen");
                    return Task.CompletedTask;
                }))
            .HookKeyUp(args =>
            {
                executionOrder.Add("HookAfterWhen");
                return Task.CompletedTask;
            });

        var (_, keyUp) = builder.Build();

        // Act
        await keyUp.NotifyOnKeyUpAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - Hook declared after When still executes first
        executionOrder.Should().HaveCount(2);
        executionOrder[0].Should().Be("HookAfterWhen");
        executionOrder[1].Should().Be("CommandInWhen");
    }

    [Test]
    public async Task HookKeyDown_MixedWithWhenScopes_MaintainsCorrectOrder()
    {
        // Arrange
        var condition = true;
        var executionOrder = new List<string>();
        var builder = KeyMapBuilder.Create()
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook1");
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command1");
                return Task.CompletedTask;
            })
            .When(() => condition, b => b
                .HookKeyDown(args =>
                {
                    executionOrder.Add("Hook2InWhen");
                    return Task.CompletedTask;
                })
                .OnKeyDown("Enter", () =>
                {
                    executionOrder.Add("Command2InWhen");
                    return Task.CompletedTask;
                }))
            .HookKeyDown(args =>
            {
                executionOrder.Add("Hook3");
                return Task.CompletedTask;
            })
            .OnKeyDown("Enter", () =>
            {
                executionOrder.Add("Command3");
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        // Assert - All hooks execute first in declaration order, then first matching command
        executionOrder.Should().HaveCount(4);
        executionOrder[0].Should().Be("Hook1");
        executionOrder[1].Should().Be("Hook2InWhen");
        executionOrder[2].Should().Be("Hook3");
        executionOrder[3].Should().Be("Command1");
    }
}
