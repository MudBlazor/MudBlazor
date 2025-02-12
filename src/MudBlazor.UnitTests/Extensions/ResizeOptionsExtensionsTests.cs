// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable
using System.Collections.Generic;
using FluentAssertions;
using MudBlazor.Extensions;
using MudBlazor.Services;
using NUnit.Framework;

namespace MudBlazor.UnitTests;

[TestFixture]
public class ResizeOptionsExtensionsTests
{
    [Test]
    public void Clone_ShouldCreateNewInstanceWithSamePropertyValues()
    {
        // Arrange
        var originalOptions = new ResizeOptions
        {
            ReportRate = 100,
            EnableLogging = false,
            SuppressInitEvent = true,
            NotifyOnBreakpointOnly = true,
            BreakpointDefinitions = new Dictionary<Breakpoint, int>
            {
                { Breakpoint.Xxl, 2560 },
                { Breakpoint.Xl, 1920 },
                { Breakpoint.Lg, 1280 },
                { Breakpoint.Md, 960 },
                { Breakpoint.Sm, 600 },
                { Breakpoint.Xs, 0 }
            }
        };

        // Act
        var clonedOptions = originalOptions.Clone();

        // Assert
        clonedOptions.Should().NotBeNull();
        clonedOptions.Should().NotBeSameAs(originalOptions);
        clonedOptions.ReportRate.Should().Be(originalOptions.ReportRate);
        clonedOptions.EnableLogging.Should().Be(originalOptions.EnableLogging);
        clonedOptions.SuppressInitEvent.Should().Be(originalOptions.SuppressInitEvent);
        clonedOptions.NotifyOnBreakpointOnly.Should().Be(originalOptions.NotifyOnBreakpointOnly);
        clonedOptions.BreakpointDefinitions.Should().BeEquivalentTo(originalOptions.BreakpointDefinitions);
    }
}
