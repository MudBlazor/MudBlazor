using MudBlazor.Docs.Extensions;

namespace MudBlazor.UnitTests.Docs.Extensions;

public class StringExtensionsTests
{
    [Test]
    public async Task ToKebabCaseTest()
    {
        await Assert.That(default(string).ToKebabCase()).IsNull();
        await Assert.That("".ToKebabCase()).IsEqualTo("");
        await Assert.That("I".ToKebabCase()).IsEqualTo("i");
        await Assert.That("IO".ToKebabCase()).IsEqualTo("io");
        await Assert.That("FileIO".ToKebabCase()).IsEqualTo("file-io");
        await Assert.That("SignalR".ToKebabCase()).IsEqualTo("signal-r");
        await Assert.That("IOStream".ToKebabCase()).IsEqualTo("io-stream");
        await Assert.That("COMObject".ToKebabCase()).IsEqualTo("com-object");
        await Assert.That("WebAPI".ToKebabCase()).IsEqualTo("web-api");
        await Assert.That("awesome".ToKebabCase()).IsEqualTo("awesome");
        await Assert.That("kebab-case".ToKebabCase()).IsEqualTo("kebab-case");
    }

    [Test]
    public async Task ToPascalCaseTest()
    {
        await Assert.That(default(string).ToPascalCase()).IsNull();
        await Assert.That("".ToPascalCase()).IsEqualTo("");
        await Assert.That("i".ToPascalCase()).IsEqualTo("I");
        await Assert.That("I".ToPascalCase()).IsEqualTo("I");
        await Assert.That("io".ToPascalCase()).IsEqualTo("Io");
        await Assert.That("IO".ToPascalCase()).IsEqualTo("IO");
        await Assert.That("file-io".ToPascalCase()).IsEqualTo("FileIo");
        await Assert.That("FileIO".ToPascalCase()).IsEqualTo("FileIO");
        await Assert.That("signal-r".ToPascalCase()).IsEqualTo("SignalR");
        await Assert.That("SignalR".ToPascalCase()).IsEqualTo("SignalR");
        await Assert.That("COMObject".ToPascalCase()).IsEqualTo("COMObject");
        await Assert.That("WebAPI".ToPascalCase()).IsEqualTo("WebAPI");
        await Assert.That("awesome".ToPascalCase()).IsEqualTo("Awesome");
        await Assert.That("kebab-case".ToPascalCase()).IsEqualTo("KebabCase");
        await Assert.That("snake_case".ToPascalCase()).IsEqualTo("SnakeCase");
    }
}
