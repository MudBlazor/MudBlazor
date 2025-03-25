using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Components;
using MudBlazor.UnitTests.Mocks;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

#nullable enable
[TestFixture]
public class ParameterViewExtensionsTests
{
    [Test]
    public void HasParameterChanged_WithComparer_ParameterNotChanged_ReturnsFalse()
    {
        // Arrange
        const string ParameterName = "Parameter";
        var parameters = new Dictionary<string, object?>
        {
            [ParameterName] = 10
        };
        var parameterView = ParameterView.FromDictionary(parameters);

        // Act
        var result = parameterView.HasParameterChanged(ParameterName, 10, new MockIntEqualityComparer());

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void HasParameterChanged_WithComparer_ParameterChanged_ReturnsTrue()
    {
        // Arrange
        const string ParameterName = "Parameter";
        var parameters = new Dictionary<string, object?>
        {
            [ParameterName] = 10
        };
        var parameterView = ParameterView.FromDictionary(parameters);

        // Act
        var result = parameterView.HasParameterChanged(ParameterName, 20, new MockIntEqualityComparer());

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void HasParameterChanged_WithoutComparer_ParameterNotChanged_ReturnsFalse()
    {
        // Arrange
        const string ParameterName = "Parameter";
        var parameters = new Dictionary<string, object?>
        {
            [ParameterName] = 10
        };
        var parameterView = ParameterView.FromDictionary(parameters);

        // Act
        var result = parameterView.HasParameterChanged(ParameterName, 10);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void HasParameterChanged_WithoutComparer_ParameterChanged_ReturnsTrue()
    {
        // Arrange
        const string ParameterName = "Parameter";
        var parameters = new Dictionary<string, object?>
        {
            [ParameterName] = 10
        };
        var parameterView = ParameterView.FromDictionary(parameters);

        // Act
        var result = parameterView.HasParameterChanged(ParameterName, 20);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void HasParameterChanged_WithNewValue_ParameterNotChanged_ReturnsFalse()
    {
        // Arrange
        const string ParameterName = "Parameter";
        var parameters = new Dictionary<string, object?>
        {
            [ParameterName] = 10
        };
        var parameterView = ParameterView.FromDictionary(parameters);

        // Act
        var result = parameterView.HasParameterChanged(ParameterName, 10, out var newValue);

        // Assert
        newValue.Should().Be(10);
        result.Should().BeFalse();
    }

    [Test]
    public void HasParameterChanged_WithNewValue_ParameterChanged_ReturnsTrue()
    {
        // Arrange
        const string ParameterName = "Parameter";
        var parameters = new Dictionary<string, object?>
        {
            [ParameterName] = 10
        };
        var parameterView = ParameterView.FromDictionary(parameters);

        // Act
        var result = parameterView.HasParameterChanged(ParameterName, 20, out var newValue);

        // Assert
        newValue.Should().Be(10);
        result.Should().BeTrue();
    }

    [Test]
    public void HasParameterChanged_WithNewValue_WithComparer_ParameterNotChanged_ReturnsFalse()
    {
        // Arrange
        const string ParameterName = "Parameter";
        var parameters = new Dictionary<string, object?>
        {
            [ParameterName] = 10
        };
        var parameterView = ParameterView.FromDictionary(parameters);

        // Act
        var result = parameterView.HasParameterChanged(ParameterName, 10, out var newValue, new MockIntEqualityComparer());

        // Assert
        newValue.Should().Be(10);
        result.Should().BeFalse();
    }

    [Test]
    public void HasParameterChanged_WithNewValue_WithComparer_ParameterChanged_ReturnsTrue()
    {
        // Arrange
        const string ParameterName = "Parameter";
        var parameters = new Dictionary<string, object?>
        {
            [ParameterName] = 10
        };
        var parameterView = ParameterView.FromDictionary(parameters);

        // Act
        var result = parameterView.HasParameterChanged(ParameterName, 20, out var newValue, new MockIntEqualityComparer());

        // Assert
        newValue.Should().Be(10);
        result.Should().BeTrue();
    }
}
