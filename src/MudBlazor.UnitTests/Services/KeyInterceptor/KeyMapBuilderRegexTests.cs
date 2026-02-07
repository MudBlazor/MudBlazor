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
public class KeyMapBuilderRegexTests
{
    [Test]
    public async Task OnKeyDown_WithRegexPattern_MatchesAnyKey()
    {
        // Arrange
        var executionCount = 0;
        var builder = KeyMapBuilder.Create();
        builder.OnKeyDown("/./", () =>
        {
            executionCount++;
            return Task.CompletedTask;
        });

        var (keyDown, _) = builder.Build();

        // Act & Assert - Test multiple different keys
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "A" });
        executionCount.Should().Be(1);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "B" });
        executionCount.Should().Be(2);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "1" });
        executionCount.Should().Be(3);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        executionCount.Should().Be(4);
    }

    [Test]
    public async Task OnKeyDown_WithAlphaRegex_MatchesOnlyLetters()
    {
        // Arrange
        var executionCount = 0;
        var builder = KeyMapBuilder.Create();
        builder.OnKeyDown("/[a-z]/", () =>
        {
            executionCount++;
            return Task.CompletedTask;
        });

        var (keyDown, _) = builder.Build();

        // Act & Assert - Test letters (should match)
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "a" });
        executionCount.Should().Be(1);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "z" });
        executionCount.Should().Be(2);

        // Test numbers (should not match)
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "1" });
        executionCount.Should().Be(2); // unchanged

        // Test uppercase (should not match)
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "A" });
        executionCount.Should().Be(2); // unchanged
    }

    [Test]
    public async Task OnKeyDown_WithRegexOrPattern_MatchesMultipleKeys()
    {
        // Arrange
        var executionCount = 0;
        var builder = KeyMapBuilder.Create();
        builder.OnKeyDown("/Enter|Escape|Tab/", () =>
        {
            executionCount++;
            return Task.CompletedTask;
        });

        var (keyDown, _) = builder.Build();

        // Act & Assert
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        executionCount.Should().Be(1);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Escape" });
        executionCount.Should().Be(2);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Tab" });
        executionCount.Should().Be(3);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "A" });
        executionCount.Should().Be(3); // unchanged
    }

    [Test]
    public async Task OnKeyDownAny_WithMixedRegexAndLiteral_MatchesBoth()
    {
        // Arrange
        var executionCount = 0;
        var builder = KeyMapBuilder.Create();
        builder.OnKeyDownAny(["/[0-9]/", "Enter", "Escape"], () =>
        {
            executionCount++;
            return Task.CompletedTask;
        });

        var (keyDown, _) = builder.Build();

        // Act & Assert - Test regex match (digits)
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "1" });
        executionCount.Should().Be(1);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "9" });
        executionCount.Should().Be(2);

        // Test literal matches
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Enter" });
        executionCount.Should().Be(3);

        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "Escape" });
        executionCount.Should().Be(4);

        // Test non-match
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "a" });
        executionCount.Should().Be(4); // unchanged
    }

    [Test]
    public async Task OnKeyDown_WithRegexAndKeyboardEventArgs_PassesArgsCorrectly()
    {
        // Arrange
        string? capturedKey = null;
        var builder = KeyMapBuilder.Create();
        builder.OnKeyDown("/./", (args) =>
        {
            capturedKey = args.Key;
            return Task.CompletedTask;
        });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "TestKey" });

        // Assert
        capturedKey.Should().Be("TestKey");
    }

    [Test]
    public async Task OnKeyDown_WithInvalidRegex_FallsBackToLiteralMatch()
    {
        // Arrange
        var executionCount = 0;
        var builder = KeyMapBuilder.Create();
        // Invalid regex pattern (unclosed bracket)
        builder.OnKeyDown("/[a-z/", () =>
        {
            executionCount++;
            return Task.CompletedTask;
        });

        var (keyDown, _) = builder.Build();

        // Act & Assert - Should match literal string "/[a-z/"
        await keyDown!.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "/[a-z/" });
        executionCount.Should().Be(1);

        // Should not match "a" as regex would
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "a" });
        executionCount.Should().Be(1); // unchanged
    }

    [Test]
    public async Task OnKeyDown_WithRegexInConditionScope_WorksCorrectly()
    {
        // Arrange
        var canHandle = true;
        var executionCount = 0;
        var builder = KeyMapBuilder.Create();
        builder.When(() => canHandle, b => b
            .OnKeyDown("/./", () =>
            {
                executionCount++;
                return Task.CompletedTask;
            }));

        var (keyDown, _) = builder.Build();

        // Act & Assert - When condition is true
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "A" });
        executionCount.Should().Be(1);

        // When condition is false
        canHandle = false;
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "B" });
        executionCount.Should().Be(1); // unchanged
    }

    [Test]
    public async Task OnKeyDown_WithMultipleRegexCommands_ExecutesFirstMatch()
    {
        // Arrange
        var firstExecuted = false;
        var secondExecuted = false;
        var builder = KeyMapBuilder.Create();
        builder
            .OnKeyDown("/[a-z]/", () =>
            {
                firstExecuted = true;
                return Task.CompletedTask;
            })
            .OnKeyDown("/./", () =>
            {
                secondExecuted = true;
                return Task.CompletedTask;
            });

        var (keyDown, _) = builder.Build();

        // Act
        await keyDown.NotifyOnKeyDownAsync(new KeyboardEventArgs { Key = "a" });

        // Assert - Only first matching command should execute (early exit)
        firstExecuted.Should().BeTrue();
        secondExecuted.Should().BeFalse();
    }
}
