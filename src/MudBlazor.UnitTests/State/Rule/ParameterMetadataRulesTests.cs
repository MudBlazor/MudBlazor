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
    public void Morph_HandlerName_ShouldChangeMetadata(string handlerName, bool expectedResult)
    {
        // Arrange
        var metadata = new ParameterMetadata("Parameter", handlerName);

        //Act
        var newMetadata = ParameterMetadataRules.Morph(metadata);
        var isNew = !string.Equals(metadata.HandlerName, newMetadata.HandlerName, StringComparison.Ordinal);

        // Assert
        isNew.Should().Be(expectedResult);
    }

    [TestCase("() => TestComparer", "TestComparer", true)]
    [TestCase("()=>TestComparer", "TestComparer", true)]
    [TestCase("()       =>TestComparer", "TestComparer", true)]
    [TestCase("()=>       TestComparer", "TestComparer", true)]
    [TestCase("()       =>       TestComparer", "TestComparer", true)]
    [TestCase(" TestComparer ", "TestComparer", true)]
    [TestCase("TestComparer", "TestComparer", false)]
    public void Morph_Comparer_ShouldChangeMetadata(string input, string expectedComparerName, bool expectedResult)
    {
        // Arrange
        var metadata = new ParameterMetadata("Parameter", null, input);

        //Act
        var newMetadata = ParameterMetadataRules.Morph(metadata);
        var isNew = !string.Equals(metadata.ComparerParameterName, newMetadata.ComparerParameterName, StringComparison.Ordinal);

        // Assert
        isNew.Should().Be(expectedResult);
        newMetadata.ComparerParameterName.Should().Be(expectedComparerName);
    }
}
