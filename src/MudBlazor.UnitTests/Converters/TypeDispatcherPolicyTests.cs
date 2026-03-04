// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using AwesomeAssertions;
using MudBlazor.Resources;
using MudBlazor.Utilities.Converter.Dispatcher;
using MudBlazor.Utilities.Exceptions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Converters;

#nullable enable
[TestFixture]
public class TypeDispatcherPolicyTests
{
    [Test]
    public void TypeDispatcher_DefaultPolicy_LastWins()
    {
        var dispatcher = TypeDispatcher
            .Create<int, string>()
            .Add(new ConstantConverter("first"))
            .Add(new ConstantConverter("second"))
            .Build();

        dispatcher.Convert(123).Should().Be("second");
    }

    [Test]
    public void TypeDispatcher_FirstWinsPolicy_UsesFirstRegistration()
    {
        var dispatcher = TypeDispatcher
            .Create<int, string>(DispatcherRegistrationPolicy.FirstWins)
            .Add(new ConstantConverter("first"))
            .Add(new ConstantConverter("second"))
            .Build();

        dispatcher.Convert(123).Should().Be("first");
    }

    [Test]
    public void TypeDispatcher_ThrowPolicy_ThrowsOnDuplicateRegistration()
    {
        var builder = TypeDispatcher
            .Create<int, string>(DispatcherRegistrationPolicy.Throw)
            .Add(new ConstantConverter("first"));

        Action act = () => builder.Add(new ConstantConverter("second"));

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void TypeDispatcher_AddDynamic_FirstWinsPolicy_UsesFirstRegistration()
    {
        var builder = TypeDispatcher.Create<int, string>(DispatcherRegistrationPolicy.FirstWins);
        builder.AddDynamic(typeof(int), new ConstantConverter("first"));
        builder.AddDynamic(typeof(int), new ConstantConverter("second"));

        var dispatcher = builder.Build();

        dispatcher.Convert(5).Should().Be("first");
    }

    [Test]
    public void TypeDispatcher_AddDynamic_WhenConverterTypeDoesNotImplementTargetInterface_ThrowsInvalidOperationException()
    {
        var builder = TypeDispatcher.Create<int, string>();

        Action act = () => builder.AddDynamic(typeof(int), new ForwardOnlyStringConverter());

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Converter type*does not implement Convert(System.Int32)");
    }

    [Test]
    public void TypeDispatcher_UnsupportedRegistrationPolicy_ThrowsOnAdd()
    {
        var builder = TypeDispatcher.Create<int, string>((DispatcherRegistrationPolicy)999);

        Action act = () => builder.Add(new ConstantConverter("A"));

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Unsupported registration policy:*");
    }

    [Test]
    public void ReversibleTypeDispatcher_DefaultPolicy_LastWins()
    {
        var dispatcher = ReversibleTypeDispatcher
            .Create<int, string>()
            .Add(new PrefixConverter("A"))
            .Add(new PrefixConverter("B"))
            .Build();

        dispatcher.Convert(7).Should().Be("B7");
        dispatcher.ConvertBack("B7").Should().Be(7);
    }

    [Test]
    public void ReversibleTypeDispatcher_FirstWinsPolicy_UsesFirstRegistration()
    {
        var dispatcher = ReversibleTypeDispatcher
            .Create<int, string>(DispatcherRegistrationPolicy.FirstWins)
            .Add(new PrefixConverter("A"))
            .Add(new PrefixConverter("B"))
            .Build();

        dispatcher.Convert(7).Should().Be("A7");
        dispatcher.ConvertBack("A7").Should().Be(7);
    }

    [Test]
    public void ReversibleTypeDispatcher_ThrowPolicy_ThrowsOnDuplicateRegistration()
    {
        var builder = ReversibleTypeDispatcher
            .Create<int, string>(DispatcherRegistrationPolicy.Throw)
            .Add(new PrefixConverter("A"));

        Action act = () => builder.Add(new PrefixConverter("B"));

        act.Should().Throw<InvalidOperationException>();
    }

    [Test]
    public void ReversibleTypeDispatcher_AddDynamic_FirstWinsPolicy_UsesFirstRegistration()
    {
        var builder = ReversibleTypeDispatcher.Create<int, string>(DispatcherRegistrationPolicy.FirstWins);
        builder.AddDynamic(typeof(int), new PrefixConverter("A"));
        builder.AddDynamic(typeof(int), new PrefixConverter("B"));

        var dispatcher = builder.Build();

        dispatcher.Convert(3).Should().Be("A3");
        dispatcher.ConvertBack("A3").Should().Be(3);
    }

    [Test]
    public void ReversibleTypeDispatcher_AddDynamic_WhenConverterTypeDoesNotImplementForwardInterface_ThrowsInvalidOperationException()
    {
        var builder = ReversibleTypeDispatcher.Create<int, string>();

        Action act = () => builder.AddDynamic(typeof(int), new ForwardOnlyStringConverter());

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Converter type*does not implement Convert(System.Int32)");
    }

    [Test]
    public void ReversibleTypeDispatcher_AddDynamic_WhenConverterTypeDoesNotImplementReverseInterface_ThrowsInvalidOperationException()
    {
        var builder = ReversibleTypeDispatcher.Create<int, string>();

        Action act = () => builder.AddDynamic(typeof(int), new ForwardOnlyIntConverter());

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Converter type*does not implement ConvertBack(System.String)");
    }

    [Test]
    public void ReversibleTypeDispatcher_ConvertBack_WhenNoConverterRegistered_ThrowsConversionException()
    {
        var dispatcher = ReversibleTypeDispatcher
            .Create<int, string>()
            .Build();

        Action act = () => dispatcher.ConvertBack("x");

        var exception = act.Should()
            .Throw<ConversionException>()
            .Which;

        exception.ErrorMessageKey.Should().Be(LanguageResource.Converter_ConversionNotImplemented);
        exception.ErrorMessageArgs.Should().ContainSingle().Which.Should().Be(typeof(int));
        exception.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Test]
    public void ReversibleTypeDispatcher_AddDynamic_NullSpecificType_ThrowsArgumentNullException()
    {
        var builder = ReversibleTypeDispatcher.Create<int, string>();

        Action act = () => builder.AddDynamic(null!, new PrefixConverter("A"));

        act.Should()
            .Throw<ArgumentNullException>()
            .Which.ParamName
            .Should()
            .Be("specificType");
    }

    [Test]
    public void ReversibleTypeDispatcher_UnsupportedRegistrationPolicy_ThrowsOnAdd()
    {
        var builder = ReversibleTypeDispatcher.Create<int, string>((DispatcherRegistrationPolicy)999);

        Action act = () => builder.Add(new PrefixConverter("A"));

        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("Unsupported registration policy:*");
    }

    [Test]
    public void TypeDispatcher_Convert_WhenConverterThrowsConversionException_RethrowsInnerException()
    {
        var dispatcher = TypeDispatcher
            .Create<int, string>()
            .Add(new ThrowingForwardConverter())
            .Build();

        Action act = () => dispatcher.Convert(1);

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidType);
    }

    [Test]
    public void ReversibleTypeDispatcher_ConvertBack_WhenConverterThrowsConversionException_RethrowsInnerException()
    {
        var dispatcher = ReversibleTypeDispatcher
            .Create<int, string>()
            .Add(new ThrowingReverseConverter())
            .Build();

        Action act = () => dispatcher.ConvertBack("x");

        act.Should()
            .Throw<ConversionException>()
            .Which.ErrorMessageKey
            .Should()
            .Be(LanguageResource.Converter_InvalidType);
    }

    private sealed class ConstantConverter(string output) : IConverter<int, string>
    {
        public string Convert(int input) => output;
    }

    private sealed class PrefixConverter(string prefix) : IReversibleConverter<int, string>
    {
        public string Convert(int input) => $"{prefix}{input}";

        public int ConvertBack(string input)
        {
            if (!input.StartsWith(prefix, StringComparison.Ordinal))
            {
                throw new FormatException($"Input must start with '{prefix}'.");
            }

            return int.Parse(input.AsSpan(prefix.Length));
        }
    }

    private sealed class ThrowingForwardConverter : IConverter<int, string>
    {
        public string Convert(int input) => throw new ConversionException(LanguageResource.Converter_InvalidType, [nameof(Int32)]);
    }

    private sealed class ThrowingReverseConverter : IReversibleConverter<int, string>
    {
        public string Convert(int input) => input.ToString();

        public int ConvertBack(string input) => throw new ConversionException(LanguageResource.Converter_InvalidType, [nameof(Int32)]);
    }

    private sealed class ForwardOnlyStringConverter : IConverter<string, string>
    {
        public string Convert(string input) => input;
    }

    private sealed class ForwardOnlyIntConverter : IConverter<int, string>
    {
        public string Convert(int input) => input.ToString();
    }
}
