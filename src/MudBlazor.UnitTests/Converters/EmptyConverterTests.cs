// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

public class EmptyConverterTests
{
    [Test]
    public void Convert_ShouldReturnSameReference_ForReferenceType()
    {
        var conv = new EmptyConverter<Uri>();
        var obj = new Uri("https://mudblazor.com");

        var forward = conv.Convert(obj);
        var backward = conv.ConvertBack(obj);

        // identity must be preserved for reference types
        forward.Should().BeSameAs(obj);
        backward.Should().BeSameAs(obj);
    }

    [Test]
    public void Convert_ShouldReturnEqualValue_ForValueTypes()
    {
        var convInt = new EmptyConverter<int>();
        convInt.Convert(5).Should().Be(5);
        convInt.ConvertBack(5).Should().Be(5);

        var convDate = new EmptyConverter<DateTime>();
        var now = DateTime.UtcNow;
        convDate.Convert(now).Should().Be(now);
        convDate.ConvertBack(now).Should().Be(now);
    }

    [Test]
    public void Convert_ShouldReturnNull_WhenInputIsNull_ForReferenceTypes()
    {
        var conv = new EmptyConverter<object>();

        conv.Convert(null).Should().BeNull();
        conv.ConvertBack(null).Should().BeNull();
    }

    [Test]
    public void ReplacingInstanceDoesNotAffectReturnedValue_UsingDifferentInstances()
    {
        var conv = new EmptyConverter<string>();
        var a = new string(['a']);
        var b = new string(['b']);

        conv.Convert(a).Should().Be(a);
        conv.ConvertBack(a).Should().Be(a);

        // different instance returns itself unchanged
        conv.Convert(b).Should().Be(b);
        conv.ConvertBack(b).Should().Be(b);
    }
}
