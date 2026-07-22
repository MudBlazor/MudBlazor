
using FluentAssertions;
using MudBlazor.Docs.Extensions;
using NUnit.Framework;

namespace MudBlazor.UnitTests.Utilities
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void ToKebabCaseTest()
        {
            default(string).ToKebabCase().Should().Be(null);
            "".ToKebabCase().Should().Be("");
            "I".ToKebabCase().Should().Be("i");
            "IO".ToKebabCase().Should().Be("io");
            "FileIO".ToKebabCase().Should().Be("file-io");
            "SignalR".ToKebabCase().Should().Be("signal-r");
            "IOStream".ToKebabCase().Should().Be("io-stream");
            "COMObject".ToKebabCase().Should().Be("com-object");
            "WebAPI".ToKebabCase().Should().Be("web-api");
            "awesome".ToKebabCase().Should().Be("awesome");
            "kebab-case".ToKebabCase().Should().Be("kebab-case");
        }

        [Test]
        public void ToPascalCaseTest()
        {
            default(string).ToPascalCase().Should().Be(null);
            "".ToPascalCase().Should().Be("");
            "i".ToPascalCase().Should().Be("I");
            "I".ToPascalCase().Should().Be("I");
            "io".ToPascalCase().Should().Be("Io");
            "IO".ToPascalCase().Should().Be("IO");
            "file-io".ToPascalCase().Should().Be("FileIo");
            "FileIO".ToPascalCase().Should().Be("FileIO");
            "signal-r".ToPascalCase().Should().Be("SignalR");
            "SignalR".ToPascalCase().Should().Be("SignalR");
            "COMObject".ToPascalCase().Should().Be("COMObject");
            "WebAPI".ToPascalCase().Should().Be("WebAPI");
            "awesome".ToPascalCase().Should().Be("Awesome");
            "kebab-case".ToPascalCase().Should().Be("KebabCase");
            "snake_case".ToPascalCase().Should().Be("SnakeCase");
        }
    }
}
