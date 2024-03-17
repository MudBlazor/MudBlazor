using System;
using FluentAssertions;
using MudBlazor.State;
using MudBlazor.State.Rule;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State.Rule;

#nullable enable
[TestFixture]
public class ParameterMetadataRulesTests
{
    [Test]
    public void Morph_ThrowsException()
    {
        // Arrange
        ParameterMetadata? metadata = null;

        // Act 
        var addSameParameter = () => ParameterMetadataRules.Morph(metadata!);

        // Assert
        addSameParameter.Should().Throw<ArgumentNullException>();
    }

    [TestCase(null, false)]
    [TestCase("", false)]
    [TestCase("OnParameterChanged", false)]
    [TestCase("() => handlerFireCount++", true)]
    public void Morph_ShouldChangeMetadata(string handlerName, bool expectedResult)
    {
        // Arrange
        var metadata = new ParameterMetadata("Parameter", handlerName);

        //Act
        var newMetadata = ParameterMetadataRules.Morph(metadata);
        var isNew = !string.Equals(metadata.HandlerName, newMetadata.HandlerName, StringComparison.Ordinal);

        // Assert
        isNew.Should().Be(expectedResult);
    }
}
