// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.State;
using NUnit.Framework;

namespace MudBlazor.UnitTests.State;

#nullable enable
[TestFixture]
public class ParameterStateCollectionTests
{
    private ParameterStateCollection _collectionWithData;

    [SetUp]
    public void SetUp()
    {
        // Sample data collection
        var dictionary = new Dictionary<string, ParameterStateValue>
        {
            { "param1", new ParameterStateValue("param1", "lastValue1", "value1") },
            { "param2", new ParameterStateValue("param2", "lastValue2", "value2") }
        };
        _collectionWithData = new ParameterStateCollection(dictionary);
    }

    [Test]
    public void Count_ShouldReturnZero_WhenEmpty()
    {
        var emptyCollection = ParameterStateCollection.Empty;
        ParameterStateCollection defaultCollection = default;
        emptyCollection.Count.Should().Be(0);
        defaultCollection.Count.Should().Be(0);
    }

    [Test]
    public void Count_ShouldReturnCorrectCount_WhenDataIsPresent()
    {
        _collectionWithData.Count.Should().Be(2);
    }

    [Test]
    public void TryGetValue_ShouldReturnTrueAndValue_WhenParameterExists()
    {
        var result = _collectionWithData.TryGetValue("param1", out var value);

        result.Should().BeTrue();
        value.Should().NotBeNull();
        value.Value.Should().Be("value1");
        value.LastValue.Should().Be("lastValue1");
    }

    [Test]
    public void TryGetValue_ShouldReturnFalse_WhenParameterDoesNotExist()
    {
        var result = _collectionWithData.TryGetValue("param3", out var value);

        result.Should().BeFalse();
        value.Should().Be(default(ParameterStateValue));
    }

    [Test]
    public void Indexer_ShouldReturnCorrectValue_WhenParameterExists()
    {
        var value = _collectionWithData["param1"];

        value.Value.Should().Be("value1");
        value.LastValue.Should().Be("lastValue1");
    }

    [Test]
    public void Indexer_ShouldThrowKeyNotFoundException_WhenParameterDoesNotExist()
    {
        var act = () => _collectionWithData["param3"];

        act.Should().Throw<KeyNotFoundException>()
            .WithMessage("The parameter 'param3' was not found.");
    }

    [Test]
    public void TryGetValueWithType_ShouldReturnTrueAndCorrectValues_WhenParameterExists()
    {
        var result = _collectionWithData.TryGetValue<string>("param1", out var value, out var lastValue);

        result.Should().BeTrue();
        value.Should().Be("value1");
        lastValue.Should().Be("lastValue1");
    }

    [Test]
    public void TryGetValueWithType_ShouldReturnFalse_WhenParameterDoesNotExist()
    {
        var result = _collectionWithData.TryGetValue<string>("param3", out var value, out var lastValue);

        result.Should().BeFalse();
        value.Should().BeNull();
        lastValue.Should().BeNull();
    }
}
