// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

#nullable enable
[TestFixture]
internal class ConversionsTests
{
    [Test]
    public void From_Function_CreatesConverterChain_And_Then_Composes()
    {
        var chain = Conversions.From<int, string>(i => (i * 2).ToString());
        chain.Convert(3).Should().Be("6");

        // Compose: parse and add 1 -> final int
        var composed = chain.Then(s => int.Parse(s) + 1);
        composed.Convert(3).Should().Be(7);
    }

    [Test]
    public void From_IConverter_WrapsAndConverts()
    {
        var mul3 = new MulConverter(3);
        var chain = Conversions.From(mul3);
        chain.Convert(4).Should().Be("12");
    }

    [Test]
    public void From_Delegates_CreateReversibleChain_And_Reverse_Works()
    {
        var rev = Conversions.From<int, string>(
            forward: i => (i + 5).ToString(),
            backward: s => int.Parse(s) - 5);

        rev.Convert(2).Should().Be("7");
        rev.ConvertBack("10").Should().Be(5);

        var reversed = rev.Reverse();
        // reversed.Convert uses original backward delegate
        reversed.Convert("10").Should().Be(5);
        // reversed.ConvertBack uses original forward delegate (string <- int)
        reversed.ConvertBack(3).Should().Be("8");
    }

    [Test]
    public void From_IReversibleConverter_WrapsAndSupportsConvertAndConvertBack()
    {
        var revConv = new IntStringReversible(offset: 4);
        var chain = Conversions.From(revConv);

        chain.Convert(1).Should().Be("5");      // 1 + 4
        chain.ConvertBack("9").Should().Be(5);  // 9 - 4 = 5

        // Compose reversible steps: add one in forward/backward directions
        var composed = chain.Then(
            forward: s => int.Parse(s) * 2,
            backward: v => (v / 2).ToString()
        );

        // sanity: forward path works
        composed.Convert(2).Should().Be((2 + 4) * 2);
        // ensure backward conversion composes correctly
        composed.ConvertBack(18).Should().Be((18 / 2) - 4);
    }

    [Test]
    public void From_IReversibleConverter_Then_IReversibleConverter_ComposesBothDirections()
    {
        var chain = Conversions
            .From(new IntStringReversible(offset: 4))   // forward: i -> (i+4); backward: s -> int.Parse - 4
            .Then(new ParseTimesTwoReversible());        // forward: s -> int.Parse * 2; backward: v -> (v/2).ToString()

        chain.Convert(2).Should().Be((2 + 4) * 2);          // 12
        chain.ConvertBack(12).Should().Be((12 / 2) - 4);    // 2
    }

    [Test]
    public void ReversibleChain_Then_ForwardOnlyDelegate_DropsReversibility()
    {
        // Documented behavior: the inherited single-delegate Then loses backward conversion.
        var chain = Conversions
            .From<int, string>(i => i.ToString(), s => int.Parse(s))
            .Then(s => int.Parse(s) + 1);

        chain.Convert(5).Should().Be(6);
        chain.Should().NotBeAssignableTo<IReversibleConverter<int, int>>();
    }

    private sealed class MulConverter(int mul) : IConverter<int, string>
    {
        public string Convert(int input) => (input * mul).ToString();
    }

    private sealed class IntStringReversible(int offset) : IReversibleConverter<int, string>
    {
        public string Convert(int input) => (input + offset).ToString();

        public int ConvertBack(string input) => int.Parse(input) - offset;
    }

    private sealed class ParseTimesTwoReversible : IReversibleConverter<string, int>
    {
        public int Convert(string input) => int.Parse(input) * 2;

        public string ConvertBack(int input) => (input / 2).ToString();
    }
}
