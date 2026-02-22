// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Extensions;
using MudBlazor.Utilities.Exceptions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Extensions;

#nullable enable
[TestFixture]
public class ConverterExtensionsTests
{
    [Test]
    public void TryConvert_Success_ReturnsValue()
    {
        var conv = new IntToStringConverter();
        var res = conv.TryConvert(5);

        res.Success.Should().BeTrue();
        res.Value.Should().Be("5");
        res.ExceptionError.Should().BeNull();
        res.ErrorMessageKey.Should().BeNull();
    }

    [Test]
    public void TryConvert_ConversionException_IsCapturedWithKeyAndArgs()
    {
        var conv = new ThrowConversionExceptionConverter();
        var res = conv.TryConvert(1);

        res.Success.Should().BeFalse();
        res.ExceptionError.Should().BeOfType<ConversionException>();
        res.ErrorMessageKey.Should().Be("ERR_KEY");
        res.ErrorMessageArgs.Should().Contain("arg1");
    }

    [Test]
    public void TryConvert_WrappedConversionException_IsUnwrappedAndCaptured()
    {
        var conv = new ThrowWrappedConversionExceptionConverter();
        var res = conv.TryConvert(1);

        res.Success.Should().BeFalse();
        res.ExceptionError.Should().BeOfType<ConversionException>();
        res.ErrorMessageKey.Should().Be("INNER_KEY");
        res.ErrorMessageArgs.Should().Contain("a");
    }

    [Test]
    public void TryConvert_AggregateExceptionContainingConversionException_IsCaptured()
    {
        var conv = new ThrowAggregateWithConversionExceptionConverter();
        var res = conv.TryConvert(1);

        res.Success.Should().BeFalse();
        res.ExceptionError.Should().BeOfType<ConversionException>();
        res.ErrorMessageKey.Should().Be("AGG_KEY");
        res.ErrorMessageArgs.Should().Contain("x");
        res.ErrorMessageArgs.Should().Contain(2);
    }

    [Test]
    public void TryConvert_UnknownException_IsCapturedAsGenericFailure()
    {
        var conv = new ThrowUnknownExceptionConverter();
        var res = conv.TryConvert(1);

        res.Success.Should().BeFalse();
        res.ExceptionError.Should().BeOfType<InvalidOperationException>();
        res.ErrorMessageKey.Should().BeNull();
    }

    [Test]
    public void TryConvertBack_OnReversibleConverter_Works()
    {
        var rev = new ReversibleIntString();
        var res = rev.TryConvertBack("201"); // ConvertBack("201") => 201 - 100 = 101

        res.Success.Should().BeTrue();
        res.Value.Should().Be(101);
    }

    [Test]
    public void TryConvertBack_OnIConverter_WithUnderlyingReversible_WrapsResult()
    {
        // treat reversible as IConverter<int,string> and call TryConvertBack extension (it delegates to ConvertBack at runtime)
        IConverter<int, string> asConverter = new ReversibleIntString();
        var res = asConverter.TryConvertBack("201");

        res.Success.Should().BeTrue();
        res.Value.Should().Be(101);
    }

    [Test]
    public void Reverse_ReversibleConverter_ReturnsReversedChain()
    {
        var rev = new ReversibleIntString();
        var reversed = rev.Reverse(); // ReversibleChain<string,int>

        var forwardResult = reversed.Convert("150"); // uses ConvertBack of original -> 150 - 100 = 50
        forwardResult.Should().Be(50);

        var backwardResult = reversed.ConvertBack(2); // uses Convert of original -> 2 -> (2 + 100).ToString() => "102"
        backwardResult.Should().Be("102");
    }

    [Test]
    public void ConvertBack_Extension_OnNonReversible_ThrowsInvalidOperationException()
    {
        IConverter<int, string> conv = new NoBackwardConverter();

        Action act = () => conv.ConvertBack("test");

        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage($"Converter {conv.GetType().Name} does not support ConvertBack. Implement an IReversibleConverter for the converter instead.");
    }

    [Test]
    public void ConvertBack_Extension_OnReversible_InvokesConvertBack()
    {
        IConverter<int, string> conv = new ReversibleIntString();
        var back = conv.ConvertBack("123"); // should call ConvertBack and return 23
        back.Should().Be(23);
    }

    private sealed class IntToStringConverter : IConverter<int, string>
    {
        public string Convert(int input) => input.ToString();
    }

    private sealed class ThrowConversionExceptionConverter : IConverter<int, string>
    {
        public string Convert(int input) => throw new ConversionException("ERR_KEY", ["arg1"]);
    }

    private sealed class ThrowWrappedConversionExceptionConverter : IConverter<int, string>
    {
        public string Convert(int input) => throw new InvalidOperationException("wrapped", new ConversionException("INNER_KEY", ["a"]));
    }

    private sealed class ThrowAggregateWithConversionExceptionConverter : IConverter<int, string>
    {
        public string Convert(int input) => throw new AggregateException(new ConversionException("AGG_KEY", ["x", 2]));
    }

    private sealed class ThrowUnknownExceptionConverter : IConverter<int, string>
    {
        public string Convert(int input) => throw new InvalidOperationException("boom");
    }

    private sealed class ReversibleIntString : IReversibleConverter<int, string>
    {
        public string Convert(int input) => (input + 100).ToString();
        public int ConvertBack(string input) => int.Parse(input) - 100;
    }

    private sealed class NoBackwardConverter : IConverter<int, string>
    {
        public string Convert(int input) => input.ToString();
    }
}
