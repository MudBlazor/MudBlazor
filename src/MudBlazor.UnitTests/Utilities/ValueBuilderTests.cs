using AwesomeAssertions;
using MudBlazor.Utilities;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities;

[TestFixture]
public class ValueBuilderTests
{
    [Test]
    public void AddValue_AppendsConditionalValuesAndBuildsTrimmedOutput()
    {
        var builder = new ValueBuilder()
            .AddValue("display:block")
            .AddValue("color:red", when: false)
            .AddValue(() => "margin:8px");

        builder.HasValue.Should().BeTrue();
        builder.ToString().Should().Be("display:block margin:8px");
    }

    [Test]
    public void AddValue_DoesNotInvokeFactoryWhenConditionIsFalse()
    {
        var builder = new ValueBuilder();
        var wasInvoked = false;

        builder.AddValue(() =>
        {
            wasInvoked = true;
            return "padding:4px";
        }, when: false);

        wasInvoked.Should().BeFalse();
        builder.HasValue.Should().BeFalse();
        builder.ToString().Should().BeEmpty();
    }

    [Test]
    public void HasValue_ReturnsFalseWhenOnlyWhitespaceWasAdded()
    {
        var builder = new ValueBuilder()
            .AddValue(" ");

        builder.HasValue.Should().BeFalse();
        builder.ToString().Should().BeEmpty();
    }
}
