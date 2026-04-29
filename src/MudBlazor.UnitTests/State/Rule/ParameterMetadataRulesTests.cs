using AwesomeAssertions;
using MudBlazor.State;
using MudBlazor.State.Rule;
using ParameterMetadata = MudBlazor.State.ParameterMetadata;

namespace MudBlazor.UnitTests.State.Rule;
#nullable enable
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

    [Test]
    [Arguments(null, false)]
    [Arguments("", false)]
    [Arguments("OnParameterChanged", false)]
    [Arguments("() => handlerFireCount++", true)]
    public void Morph_HandlerName_ShouldChangeMetadata(string? handlerName, bool expectedResult)
    {
        // Arrange
        var metadata = new ParameterMetadata("Parameter", handlerName);

        //Act
        var newMetadata = ParameterMetadataRules.Morph(metadata);
        var isNew = !string.Equals(metadata.HandlerName, newMetadata.HandlerName, StringComparison.Ordinal);

        // Assert
        isNew.Should().Be(expectedResult);
    }

    [Test]
    [Arguments("() => TestComparer", "TestComparer", true)]
    [Arguments("()=>TestComparer", "TestComparer", true)]
    [Arguments("()       =>TestComparer", "TestComparer", true)]
    [Arguments("()=>       TestComparer", "TestComparer", true)]
    [Arguments("()       =>       TestComparer", "TestComparer", true)]
    [Arguments(" TestComparer ", "TestComparer", true)]
    [Arguments("TestComparer", "TestComparer", false)]
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
